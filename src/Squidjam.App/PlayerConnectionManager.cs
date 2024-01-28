namespace Squidjam.App;

public class PlayerConnectionManager {
	private readonly Dictionary<string, Guid> _connectionPlayers = new Dictionary<string, Guid>();
	private readonly Dictionary<Guid, HashSet<string>> _playerConnections = new Dictionary<Guid, HashSet<string>>();

	public void AddPlayer(Guid playerId, string connectionId) {
		if (!_playerConnections.ContainsKey(playerId)) {
			_playerConnections.Add(playerId, []);
		}

		_playerConnections[playerId].Add(connectionId);
		_connectionPlayers.Add(connectionId, playerId);

		Console.WriteLine($"Player {playerId} connected with connection {connectionId}");
	}

	public void RemovePlayer(Guid playerId, string connectionId) {
		_playerConnections[playerId].Remove(connectionId);

		if (_playerConnections[playerId].Count == 0) {
			_playerConnections.Remove(playerId);
		}

		_connectionPlayers.Remove(connectionId);

		Console.WriteLine($"Player {playerId} disconnected with connection {connectionId}");
	}

	public Guid GetPlayerId(string connectionId) {
		return _connectionPlayers[connectionId];
	}

	public HashSet<string> GetConnectionIds(Guid playerId) {
		return _playerConnections.GetValueOrDefault(playerId, []);
	}
}
