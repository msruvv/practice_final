using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TimeTrackingWebAPI;
using TimeTrackingWebAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

/// <summary>
/// Настройка подключения к базе данных.
/// </summary>
builder.Services.AddDbContext<TimeTrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Регистрация репозиториев в DI контейнере.
/// </summary>
builder.Services.AddScoped<IProjectRepository, EFProjectRepository>();
builder.Services.AddScoped<ITaskRepository, EFTaskRepository>();
builder.Services.AddScoped<ITimeEntryRepository, EFTimeEntryRepository>();

var app = builder.Build();

app.UseCors("AllowAll");

/// <summary>
/// Настройка конвейера обработки HTTP запросов.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();