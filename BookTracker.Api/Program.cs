using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);
builder.AddBookTrackerServices();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseBookTracker();
app.UseMemberTracker();
app.UseCors();
app.Run();

public partial class Program;