using System.Text;
using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Middleware;
using BankingApi.Repositories;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt settings are missing.");
if (jwtSettings.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

string connectionString = builder.Configuration.GetConnectionString("BankingDb")
    ?? throw new InvalidOperationException("Connection string 'BankingDb' is missing.");


builder.Services.AddDbContext<BankingDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IDisbursementRepository, DisbursementRepository>();
builder.Services.AddScoped<IRepaymentRepository, RepaymentRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IDisbursementService, DisbursementService>();
builder.Services.AddScoped<IRepaymentService, RepaymentService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

            string firstMessage = errors.Values.SelectMany(v => v).FirstOrDefault()
                                  ?? "The request is not valid.";

            return new BadRequestObjectResult(new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = firstMessage,
                Errors = errors
            });
        };
    });

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;   
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            NameClaimType = AppClaims.Name,
            RoleClaimType = AppClaims.Role,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "You need to log in to do this."
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You don't have permission to do this."
                });
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banking & Loan Management API",
        Version = "v1",
        Description = "Customers apply for loans and track them; admins manage products, approvals, disbursements and payments."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Log in with /api/auth/login, then paste the token here."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BankingDbContext>();

    if (!db.Database.GetMigrations().Any())
        throw new InvalidOperationException(
            "No EF Core migrations found. In the BankingApi folder run: dotnet ef migrations add InitialCreate");

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();   
app.UseMiddleware<RequestLoggingMiddleware>();      

app.UseSwagger();
app.UseSwaggerUI(options => options.DocumentTitle = "Banking & Loan API");

app.UseCors("Frontend");
app.UseAuthentication();                             
app.UseAuthorization();                              

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.RunAsync();
