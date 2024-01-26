using System.Text.Json.Serialization;
using Microsoft.FSharp.Core;
using Squidjam.Game;

var games = new Dictionary<Guid, Game>();
Guid testGameGuid = Guid.NewGuid();

games.Add(testGameGuid,
	new Game(testGameGuid, GameState.NewPlayerTurn(0), [new Player(Guid.NewGuid(), false, FSharpOption<Class>.Some(Class.Grack))]));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
	options.SerializerOptions.Converters.Add(
		new JsonFSharpConverter(
			JsonFSharpOptions.Default().WithUnionTagName("type").WithUnionNamedFields().WithUnionInternalTag()
		)
	));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/api/games", () => games);

app.Run();
