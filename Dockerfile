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
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish /app/publish .

# Expose port (Railway sets PORT environment variable)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "BenchLibrary.Web.dll"]
