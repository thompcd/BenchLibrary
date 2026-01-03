using BenchLibrary.Data.Extensions;
using BenchLibrary.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// Add BenchLibrary services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=benchlibrary.db";
builder.Services.AddBenchLibrarySqlite(connectionString);

// Add application services
builder.Services.AddScoped<SpcDataService>();
builder.Services.AddScoped<SimulatedDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BenchLibrary.Data.BenchLibraryDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
