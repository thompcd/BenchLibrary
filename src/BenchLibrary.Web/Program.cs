using BenchLibrary.Data.Extensions;
using BenchLibrary.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Railway (uses PORT environment variable)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHealthChecks();

// Add BenchLibrary services
// Use PostgreSQL if DATABASE_URL is set (Railway), otherwise SQLite
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Railway PostgreSQL connection string
    builder.Services.AddBenchLibraryPostgreSql(databaseUrl);
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=data/benchlibrary.db";
    builder.Services.AddBenchLibrarySqlite(connectionString);
}

// Add application services
builder.Services.AddScoped<SpcDataService>();
builder.Services.AddScoped<SimulatedDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Don't use HTTPS redirection when running behind Railway's proxy
    if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") == null)
    {
        app.UseHsts();
    }
}

// Only redirect to HTTPS in non-Railway environments
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") == null)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Ensure data directory and database are created
Directory.CreateDirectory("data");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BenchLibrary.Data.BenchLibraryDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
