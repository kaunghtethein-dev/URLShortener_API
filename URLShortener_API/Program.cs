using URLShortener_Application.Interfaces.Services;
using URLShortener_Application.Services;
using URLShortener_Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Register services related to Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);

//Register services related to Application
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
