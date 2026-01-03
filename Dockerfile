# BenchLibrary Web - Dockerfile
# Multi-stage build for .NET 8 Blazor Server application

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["BenchLibrary.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/BenchLibrary.Core/BenchLibrary.Core.csproj", "src/BenchLibrary.Core/"]
COPY ["src/BenchLibrary.SixSigma/BenchLibrary.SixSigma.csproj", "src/BenchLibrary.SixSigma/"]
COPY ["src/BenchLibrary.Data/BenchLibrary.Data.csproj", "src/BenchLibrary.Data/"]
COPY ["src/BenchLibrary.Web/BenchLibrary.Web.csproj", "src/BenchLibrary.Web/"]

# Restore dependencies
RUN dotnet restore "src/BenchLibrary.Web/BenchLibrary.Web.csproj"

# Copy source code
COPY ["src/", "src/"]

# Build the application
RUN dotnet build "src/BenchLibrary.Web/BenchLibrary.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/BenchLibrary.Web/BenchLibrary.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# Copy published application with correct ownership
COPY --from=publish --chown=appuser:appuser /app/publish .

# Create data directory for SQLite (if no DATABASE_URL is set)
RUN mkdir -p /app/data && chown -R appuser:appuser /app/data

# Switch to non-root user
USER appuser

# Expose port (Railway sets PORT environment variable dynamically)
EXPOSE 8080

# Set environment variables
# Note: Do NOT set ASPNETCORE_URLS here - Program.cs handles PORT from Railway
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Note: Railway handles health checks via railway.toml healthcheckPath
# Docker HEALTHCHECK removed as aspnet:8.0 image doesn't include curl

# Start the application
ENTRYPOINT ["dotnet", "BenchLibrary.Web.dll"]
