using System.Text.Json;
using System.Text.Json.Nodes;
using Containers.Application;
using Containers.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

builder.Services.AddTransient<IContainerService, ContainerService>(
    _ => new ContainerService(connectionString));

// Add services to the container.
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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/api/containers", (IContainerService containerService) =>
{
    try
    {
        return Results.Ok(containerService.GetAllContainers());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("api/containers", (IContainerService containerService, Container container) =>
{
    try
    {
        var result = containerService.Create(container);
        if (result is true) return Results.Created();
        else return Results.BadRequest();

    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/containers", (IContainerService containerService, HttpRequest request) =>
{
    using (var reader = new StreamReader(request.Body))
    {
        string rowJson = await reader.ReadToEndAsync();

        var json = JsonNode.Parse(rowJson);
        var specifiedType = json["type"];
        if (specifiedType != null && specifiedType.ToString() == "Standart")
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            
            var containerInfo = JsonSerializer.Deserialize<ShortContainerInfos>(json["typeValues"], options);
            containerService.Create(new Container
            {
                ContainerTypeId = 1;
                IsHazardious = containerInfo.IsHazardious,
                
            })
                return Results.Created();
        }
    }
    return Results.BadRequest();
});

app.Run();

class ShortContainerInfos
{
    public string IsHazardious { get; set; }
    public String Name { get; set; }
}