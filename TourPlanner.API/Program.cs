using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddScoped<ITourService, TourService>();


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
// Apply CORS policy globally
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
