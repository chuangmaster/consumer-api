using System.Security.Cryptography;
using ConsumerApi.Infrastructure;
using ConsumerApi.Repositories;
using ConsumerApi.Repositories.Interfaces;
using ConsumerApi.Services;
using ConsumerApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure ──
builder.Services.AddSingleton<DbConnectionFactory>();

// ── Repositories ──
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IPreferenceRepository, PreferenceRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();

// ── Services ──
builder.Services.AddScoped<IRegistryService, RegistryService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

// ── JWT Bearer Authentication ──
// 啟動時從 rbac-api 取得 RSA 公鑰 (JWK)，用於離線驗證 JWT
var rbacPublicKeyUrl =
    builder.Configuration["JWT:PublicKeyUrl"] ?? "http://localhost:5100/api/auth/public-key";

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // 動態從 rbac-api 取得公鑰
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                using var http = new HttpClient();
                var jwkJson = http.GetStringAsync(rbacPublicKeyUrl).Result;
                var jwk = new JsonWebKey(jwkJson);
                return [jwk];
            },
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:3000", // demo-consumer
                    "http://localhost:3001", // demo-provider
                    "http://localhost:3002"
                ) // common-provider
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// ── Controllers ──
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    });

var app = builder.Build();

// ── Middleware Pipeline ──
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
