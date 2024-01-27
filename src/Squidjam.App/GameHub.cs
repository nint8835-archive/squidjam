using Microsoft.AspNetCore.SignalR;

namespace Squidjam.App;

public class GameHub(PlayerConnectionManager manager) : Hub {
	public override Task OnConnectedAsync() {
		string? playerId = Context.GetHttpContext().Request.Query["playerId"].First();
		if (playerId == null) {
			Context.Abort();
			return Task.CompletedTask;
		}

		if (!Guid.TryParse(playerId, out Guid playerIdGuid)) {
			Context.Abort();
			return Task.CompletedTask;
		}

		manager.AddPlayer(playerIdGuid, Context.ConnectionId);

		return base.OnConnectedAsync();
	}

	public async Task Example() {
		// Log the user making the call
		Console.WriteLine("Got Example");
		Console.WriteLine(manager.GetPlayerId(Context.ConnectionId));
	}
}
