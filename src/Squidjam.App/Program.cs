using System.Text.Json.Serialization;
using FSharp.SystemTextJson.Swagger;
using Microsoft.FSharp.Core;
using Squidjam.Game;
using Swashbuckle.AspNetCore.SwaggerGen;

var games = new Dictionary<Guid, Game>();
Guid testGameGuid = Guid.NewGuid();

games.Add(testGameGuid,
	new Game(testGameGuid, GameState.NewPlayerTurn(0), [new Player(Guid.NewGuid(), false, FSharpOption<Class>.Some(Class.Grack))]));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JsonFSharpOptions? fsOptions = JsonFSharpOptions.Default().WithUnionTagName("type").WithUnionNamedFields().WithUnionInternalTag();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForSystemTextJson(fsOptions, FSharpOption<FSharpFunc<SwaggerGenOptions, Unit>>.None);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonFSharpConverter(fsOptions)));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/api/games", () => games);

app.Run();
