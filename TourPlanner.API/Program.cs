using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.DAL.Repositories;
using TourPlanner.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddControllers(); 
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ITourLogService, TourLogService>();
builder.Services.AddScoped<IOpenRouteService, OpenRouteService>();
//for requesting stuff from openroute
builder.Services.AddHttpClient<IOpenRouteService, OpenRouteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<TourPlannerDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITourLogRepository, TourLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    
    var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        
    };
});

// Add CORS services
//for some reason we need CORS because our server doenst like how angular handles stuff?
// https://towardsdev.com/fixing-cors-issues-with-credentials-in-angular-17-a-guide-for-net-core-backend-integration-6e397f7549c5
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Specify allowed origins
            .AllowAnyMethod()  // Allow GET, POST, PUT, DELETE methods
            .AllowAnyHeader()  // Allow custom headers
            .AllowCredentials();  // Allow cookies or credentials
    });
});
var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
// Apply CORS policy globally
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

// app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();