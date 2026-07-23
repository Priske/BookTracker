using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationServices();
var frontendOrigin = builder.Configuration["FrontendOrigin"]
    ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseBookTracker();
app.UseCors();
app.Run();

public partial class Program;