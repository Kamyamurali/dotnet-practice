using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Middleware;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services (Dependency Injection container) ----------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database: Azure SQL / SQL Server when a connection string is set,
// otherwise an in-memory database so the site runs with zero setup.
var connectionString = builder.Configuration.GetConnectionString("PortfolioDb");
builder.Services.AddDbContext<PortfolioDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
        options.UseInMemoryDatabase("PortfolioDb");
    else
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()); // retries help with Azure SQL
});

// Scoped = one instance per HTTP request (matches DbContext lifetime)
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

// CORS: lets the React dev server (Vite, port 5173) call the API while developing
builder.Services.AddCors(options =>
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ---------- Seed the database ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Using {Provider}", string.IsNullOrWhiteSpace(connectionString) ? "in-memory database" : "SQL Server");
    try
    {
        await SeedData.InitializeAsync(db, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Could not create or seed the database. Check the connection string and the Azure SQL firewall.");
    }
}

// ---------- HTTP pipeline (order matters) ----------
app.UseGlobalExceptionHandling();
app.UseRequestLogging();

// Swagger stays on in production too, so recruiters can see the API at /swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("ReactDev");

// Serve the built React app from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));

// Any non-API route goes to React (single-page app)
app.MapFallbackToFile("index.html");

app.Run();
