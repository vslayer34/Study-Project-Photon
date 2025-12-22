using Fusion;
using UnityEngine;


namespace PhotonCourse.Scripts.MainGame.Managers
{
    public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField]
        private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

        [SerializeField]
        private Transform[] _spawnPoints;


        // Memeber Methods-------------------------------------------------------------------------

        public override void Spawned()
        {
            base.Spawned();

            GlobalsManager.Instance.PlayerSpawnerControllerInstance = this;

            if (Runner.IsServer)
            {
                foreach (var player in Runner.ActivePlayers)
                {
                    SpawnPlayer(player);
                }
            }
        }

        private void SpawnPlayer(PlayerRef playerRef)
        {
            if (Runner.IsServer)
            {
                var index = playerRef % _spawnPoints.Length;
                Vector3 spawnPoint = _spawnPoints[playerRef].transform.position;
                var spawnedObj = Runner.Spawn(_playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

                Runner.SetPlayerObject(playerRef, spawnedObj);
            }
        }

        private void DespawnPlayer(PlayerRef playerRef)
        {
            if (Runner.IsServer)
            {
                if (Runner.TryGetPlayerObject(playerRef, out var networkObject))
                {
                    Runner.Despawn(networkObject);
                }

                // Reset To Defaults
                Runner.SetPlayerObject(playerRef, null);
            }
        }

        public Vector2 GetRandomSpawnPoint()
        {
            int index = Random.Range(0, _spawnPoints.Length);

            return _spawnPoints[index].position;
        }

        // Interface Methods-----------------------------------------------------------------------

        public void PlayerJoined(PlayerRef playerRef)
        {
            SpawnPlayer(playerRef);
        }

        public void PlayerLeft(PlayerRef playerRef)
        {
            DespawnPlayer(playerRef);
        }
    }
}