using JwtDemoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var keys = new JwtKeys(builder.Configuration["Jwt:Key"]!);
builder.Services.AddSingleton(keys);
builder.Services.AddSingleton<TokenService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; 

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,              
            ValidateAudience = true,           
            ValidateLifetime = true,            
            ValidateIssuerSigningKey = true,    
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKeys = new SecurityKey[] { keys.HmacKey, keys.RsaSigningKey },

            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256 },

            TokenDecryptionKey = keys.RsaEncryptionKey,

            NameClaimType = "name",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.Zero         
        };

       
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (!ctx.Request.Headers.ContainsKey("Authorization") &&
                    ctx.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                {
                    ctx.Token = cookieToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
}

app.UseDefaultFiles();   
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();