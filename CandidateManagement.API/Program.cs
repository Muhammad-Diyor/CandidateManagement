using CandidateManagement.API.Middleware;
using CandidateManagement.API.Validators;
using CandidateManagement.Application.Caching;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Application.Services;
using CandidateManagement.Infrastructure.Caching;
using CandidateManagement.Infrastructure.Data;
using CandidateManagement.Infrastructure.Repositories;
using CandidateManagement.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CandidateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICandidateService, CandidateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
