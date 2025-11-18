using Clima.Cep.Application.Extensions;
using Clima.Cep.Domain.Repositories;
using Clima.Cep.Infrastructure.Repositories;
using ServiceIntegration.BrasilAPICEP.Extensions;
using ServiceIntegration.OpenMeteoForecast.Extensions;
using ServiceIntegration.OpenMeteoGeocoding.Extensions;
using ServiceIntegration.ViaCEP.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HTTP clients from ServiceIntegration projects
builder.Services.AddViaCepClient();
builder.Services.AddBrasilAPICEPClient();
builder.Services.AddOpenMeteoWeatherClient();
builder.Services.AddOpenMeteoGeocodingClient();

// Register repositories
builder.Services.AddSingleton<IZipCodeLookupRepository, InMemoryZipCodeLookupRepository>();

// Register application layer
builder.Services.AddApplicationLayer();

// Add memory cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
