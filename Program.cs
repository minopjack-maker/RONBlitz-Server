using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RONBlitz.Server.Data;
using RONBlitz.Server.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// --- JWT Secret Key (⚠️ consider moving to environment variable later)
// --- JWT Secret Key (⚠️ Must be at least 256 bits)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "A9$kd02!fJz83nLpVxR5qT7uHmYcWbZ4";

// --- Add Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ronblitz.db"));
builder.Services.AddScoped<PlayerScoresService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.WithOrigins(
            "https://naturesbunker.netlify.app",
            "http://naturesbunker.netlify.app",
            "http://localhost:5173",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- Middleware ---
app.UseCors("AllowClient");
// app.UseHttpsRedirection(); // ❌ disable for Render
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapGet("/", () => Results.Ok("✅ RONBlitz Server is running online"));

app.Run();
