namespace Squidjam.App;

public class PlayerConnectionManager {
	private readonly Dictionary<string, Guid> _connectionPlayers = new Dictionary<string, Guid>();
	private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
	private readonly Dictionary<Guid, HashSet<string>> _playerConnections = new Dictionary<Guid, HashSet<string>>();

	public void AddPlayer(Guid playerId, string connectionId) {
		try {
			_lock.EnterWriteLock();

			if (!_playerConnections.ContainsKey(playerId)) {
				_playerConnections.Add(playerId, []);
			}

			_playerConnections[playerId].Add(connectionId);
			_connectionPlayers.Add(connectionId, playerId);

			Console.WriteLine($"Player {playerId} connected with connection {connectionId}");
		}
		finally {
			_lock.ExitWriteLock();
		}
	}

	public void RemovePlayer(Guid playerId, string connectionId) {
		try {
			_lock.EnterWriteLock();

			_playerConnections[playerId].Remove(connectionId);

			if (_playerConnections[playerId].Count == 0) {
				_playerConnections.Remove(playerId);
			}

			_connectionPlayers.Remove(connectionId);

			Console.WriteLine($"Player {playerId} disconnected with connection {connectionId}");
		}
		finally {
			_lock.ExitWriteLock();
		}
	}

	public Guid GetPlayerId(string connectionId) {
		try {
			_lock.EnterReadLock();
			return _connectionPlayers[connectionId];
		}
		finally {
			_lock.ExitReadLock();
		}
	}

	public HashSet<string> GetConnectionIds(Guid playerId) {
		try {
			_lock.EnterReadLock();
			return _playerConnections.GetValueOrDefault(playerId, []);
		}
		finally {
			_lock.ExitReadLock();
		}
	}
}
