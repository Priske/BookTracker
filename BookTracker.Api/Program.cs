using BookTracker.Api.Application;
using BookTracker.Api.Storage;
using BookTracker.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.UpdateBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

builder.Services.AddScoped<IBookRepository, EfBookRepository>();

builder.Services.AddScoped<GetBookListQuery>();
builder.Services.AddScoped<GetBookByIdQuery>();

builder.Services.AddScoped<CreateBookCommandHandler>();
builder.Services.AddScoped<UpdateBookCommandHandler>();
builder.Services.AddScoped<DeleteBookCommandHandler>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }
}
app.MapBookEndpoints();


app.Run();

public partial class Program;