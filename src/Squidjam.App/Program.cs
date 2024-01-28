using System.Text.Json.Serialization;
using FSharp.SystemTextJson.Swagger;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.FSharp.Core;
using Squidjam.App;
using Squidjam.Game;
using Swashbuckle.AspNetCore.SwaggerGen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JsonFSharpOptions? fsOptions = JsonFSharpOptions.Default().WithUnionTagName("type").WithUnionNamedFields().WithUnionInternalTag();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForSystemTextJson(fsOptions, FSharpOption<FSharpFunc<SwaggerGenOptions, Unit>>.None);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonFSharpConverter(fsOptions)));
builder.Services.AddSignalR().AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new JsonFSharpConverter(fsOptions)));

builder.Services.AddSingleton<PlayerConnectionManager>();
builder.Services.AddSingleton<GameManager>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapHub<GameHub>("/api/realtime");

app.MapGet("/api/games", (GameManager manager) => manager.GetGames()).WithName("ListGames");
app.MapPost("/api/games", (GameManager manager) => {
	Game newGame = new Game(Guid.NewGuid(), GameState.PlayerRegistration, []);
	manager.Store(newGame);

	return newGame;
}).WithName("CreateGame");

app.MapGet("/api/games/{gameId:guid}", Results<Ok<Game>, NotFound<string>> (GameManager manager, Guid gameId) => {
	if (!manager.TryGetGame(gameId, out Game? value)) {
		return TypedResults.NotFound("Game not found");
	}

	return TypedResults.Ok(value);
}).WithName("GetGame");

app.MapPost("/api/games/{gameId:guid}/action",
	Results<Ok<Game>, NotFound<string>, BadRequest<string>> (IHubContext<GameHub> hub, PlayerConnectionManager manager, GameManager gameManager,
		Guid gameId,
		Actions.Action action) => {
		if (!gameManager.TryGetGame(gameId, out Game? value)) {
			return TypedResults.NotFound("Game not found");
		}

		Console.WriteLine($"Performing action {action} on game {gameId}");

		var newGame = Actions.Apply(value, action);

		if (newGame.IsError) {
			return TypedResults.BadRequest(newGame.ErrorValue);
		}

		gameManager.Store(newGame.ResultValue);

		if (action.IsAddPlayer) {
			Actions.Action.AddPlayer addPlayer = (Actions.Action.AddPlayer)action;

			var connectionIds = manager.GetConnectionIds(addPlayer.Player);

			foreach (string connectionId in connectionIds) {
				hub.Groups.AddToGroupAsync(connectionId, gameId.ToString());
			}
		}
		else if (action.IsRemovePlayer) {
			Actions.Action.RemovePlayer removePlayer = (Actions.Action.RemovePlayer)action;

			var connectionIds = manager.GetConnectionIds(removePlayer.Player);

			foreach (string connectionId in connectionIds) {
				hub.Groups.RemoveFromGroupAsync(connectionId, gameId.ToString());
			}
		}

		hub.Clients.Groups(gameId.ToString()).SendAsync("GameUpdated", newGame.ResultValue);

		return TypedResults.Ok(newGame.ResultValue);
	}).WithName("PerformAction");

app.Run();
