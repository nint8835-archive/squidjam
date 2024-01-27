using System.Text.Json.Serialization;
using FSharp.SystemTextJson.Swagger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.FSharp.Core;
using Squidjam.App;
using Squidjam.Game;
using Swashbuckle.AspNetCore.SwaggerGen;

var games = new Dictionary<Guid, Game>();
Guid testGameGuid = Guid.NewGuid();

games.Add(testGameGuid,
	new Game(testGameGuid, GameState.PlayerRegistration, [new Player(Guid.NewGuid(), false, FSharpOption<Class>.Some(Class.Grack))]));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JsonFSharpOptions? fsOptions = JsonFSharpOptions.Default().WithUnionTagName("type").WithUnionNamedFields().WithUnionInternalTag();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForSystemTextJson(fsOptions, FSharpOption<FSharpFunc<SwaggerGenOptions, Unit>>.None);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonFSharpConverter(fsOptions)));
builder.Services.AddSignalR();
builder.Services.AddSingleton<PlayerConnectionManager>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapHub<GameHub>("/api/realtime");

app.MapGet("/api/games", () => games).WithName("ListGames");
app.MapPost("/api/games", () => {
	Game newGame = new Game(Guid.NewGuid(), GameState.PlayerRegistration, []);
	games.Add(newGame.Id, newGame);

	return newGame;
}).WithName("CreateGame");

app.MapPost("/api/games/{gameId:guid}/action",
	Results<Ok<Game>, NotFound<string>, BadRequest<string>> (Guid gameId, Actions.Action action) => {
		if (!games.TryGetValue(gameId, out Game? value)) {
			return TypedResults.NotFound("Game not found");
		}

		Console.WriteLine($"Performing action {action} on game {gameId}");

		var newGame = Actions.Apply(value, action);

		if (newGame.IsError) {
			return TypedResults.BadRequest(newGame.ErrorValue);
		}

		games[gameId] = newGame.ResultValue;

		return TypedResults.Ok(newGame.ResultValue);
	}).WithName("PerformAction");

app.Run();
