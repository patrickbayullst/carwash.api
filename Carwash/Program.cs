using Carwash.Repositories;
using Carwash.Services;
using Carwash.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Repositories
builder.Services.AddSingleton<UserRepository>();

//services
builder.Services.AddSingleton<AuthService>();

//utilities
builder.Services.AddSingleton<PasswordUtility>();
builder.Services.AddSingleton<TokenUtility>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
