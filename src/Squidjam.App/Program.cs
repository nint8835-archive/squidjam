using System.Text.Json.Serialization;
using Squidjam.Game;

var games = new Dictionary<Guid, Game>();
Guid testGameGuid = Guid.NewGuid();

games.Add(testGameGuid, new Game(testGameGuid, GameState.PlayerRegistration, Array.Empty<Player>()));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonFSharpConverter()));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/api/games", () => games);

app.Run();
