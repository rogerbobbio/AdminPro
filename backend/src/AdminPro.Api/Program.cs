using AdminPro.Api.Middleware;
using AdminPro.Application;
using AdminPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Routes Serilog events to any ILoggerProvider registered via the normal DI/ILoggingBuilder
// pipeline too (not just Serilog's own sinks) - otherwise UseSerilog() replaces the provider
// chain outright and providers like a test log-capture provider never see anything.
var loggerProviders = new LoggerProviderCollection();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/adminpro-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Providers(loggerProviders)
    .CreateLogger();

builder.Host.UseSerilog(dispose: true, providers: loggerProviders);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
