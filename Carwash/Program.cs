using Carwash.BackgroundServices;
using Carwash.Models.Settings;
using Carwash.RabbitMqEventHandlers;
using Carwash.Repositories;
using Carwash.Services;
using Carwash.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Repositories
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<PaymentHistoryRepository>();
builder.Services.AddSingleton<CarwashRepository>();
builder.Services.AddSingleton<TokenRepository>();

//services
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CarwashService>();

//utilities
builder.Services.AddSingleton<PasswordUtility>();
builder.Services.AddSingleton<TokenUtility>();

//backgroundservices
builder.Services.AddHostedService<Consumer>();

//eventHandlers
builder.Services.AddScoped<SyncEventHandler>();
builder.Services.AddScoped<AnyEventHandler>();

//config fíles
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("ConnectionString"));
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitConfig"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("AppSettings:Issuer").Value,
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetSection("AppSettings:Audience").Value,
            ValidateLifetime = true
        };
    });

builder.Services.AddCors();

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser().Build();
});

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

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
