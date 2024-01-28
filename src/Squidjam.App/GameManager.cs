using System.Collections.Concurrent;

namespace Squidjam.App;

public class GameManager {
	private readonly ConcurrentDictionary<Guid, Game.Game> _games = new ConcurrentDictionary<Guid, Game.Game>();

	public ConcurrentDictionary<Guid, Game.Game> GetGames() {
		return _games;
	}

	public void Store(Game.Game game) {
		_games[game.Id] = game;
	}

	public bool TryGetGame(Guid gameId, out Game.Game? game) {
		return _games.TryGetValue(gameId, out game);
	}

	public IEnumerable<Game.Game> GetPlayerGames(Guid playerId) {
		return _games.Values.Where(game => game.Players.Where(player => player.Id == playerId).ToArray().Length > 0).ToArray();
	}
}
