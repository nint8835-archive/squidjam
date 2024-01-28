using Microsoft.AspNetCore.SignalR;

namespace Squidjam.App;

public class GameHub(PlayerConnectionManager manager, GameManager gameManager) : Hub {
	private Guid? GetPlayerId() {
		string? playerId = Context.GetHttpContext().Request.Query["playerId"].First();
		if (playerId == null) {
			return null;
		}

		if (!Guid.TryParse(playerId, out Guid playerIdGuid)) {
			return null;
		}

		return playerIdGuid;
	}

	public override Task OnConnectedAsync() {
		var playerId = GetPlayerId();
		if (!playerId.HasValue) {
			Context.Abort();
			return Task.CompletedTask;
		}

		manager.AddPlayer((Guid)playerId, Context.ConnectionId);

		var playerGames = gameManager.GetPlayerGames((Guid)playerId);

		foreach (Game.Game game in playerGames) {
			Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
		}

		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception) {
		var playerId = GetPlayerId();
		if (!playerId.HasValue) {
			Context.Abort();
			return Task.CompletedTask;
		}

		manager.RemovePlayer((Guid)playerId, Context.ConnectionId);

		return base.OnDisconnectedAsync(exception);
	}
}
