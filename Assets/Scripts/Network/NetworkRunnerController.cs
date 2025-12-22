using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using PhotonCourse.Scripts.Helper.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PhotonCourse.Scripts.Network
{
    public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
    {
        public Action OnConnectionStablishedSuccessfully;
        public Action OnPlayerJoinedSuccessfully;

        [SerializeField]
        private NetworkRunner _networkRunnerPrefab;

        private NetworkRunner _networkRunnerInstance;

        [SerializeField]
        public string PlayerName { get; private set; }



        // Game Loop Methods-----------------------------------------------------------------------
        // Member Methods--------------------------------------------------------------------------

        public void SetPlayerNickName(string newName) => PlayerName = newName;

        public void ShutDownConnection() => _networkRunnerInstance.Shutdown();

        public async void StartGame(GameMode mode, string roomName)
        {
            if (_networkRunnerInstance == null)
            {
                _networkRunnerInstance = Instantiate(_networkRunnerPrefab);
            }

            OnConnectionStablishedSuccessfully?.Invoke();
            

            // Add interface methods to the network instance
            _networkRunnerInstance.AddCallbacks(this);

            _networkRunnerInstance.ProvideInput = true;

            var startGameArgs = new StartGameArgs
            {
                GameMode = mode,
                SessionName = roomName,
                PlayerCount = 4,
                SceneManager = _networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>(),
                // SceneManager = _networkRunnerInstance.GetComponent<INetworkSceneManager>(),
                ObjectPool = _networkRunnerInstance.GetComponent<INetworkObjectPool>()
            };

            var result = await _networkRunnerInstance.StartGame(args: startGameArgs);

            if (result.Ok)
            {
                //TODO - Connection Success

                _networkRunnerInstance.SetActiveScene(CS_SceneNames.MAIN_GAME);
            }
            else
            {
                Debug.LogError("Something terrbily wrong happened");
                Debug.LogError($"{result.ShutdownReason}");
            }
        }

        // Interface Methods-----------------------------------------------------------------------
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer Called");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed Called");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("OnCustomAuthenticationResponse Called");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("OnDisconnectedFromServer Called");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("OnHostMigration Called");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log("OnInputMissing Called");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerJoined Called");
            OnPlayerJoinedSuccessfully?.Invoke();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerLeft Called");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            Debug.Log("OnReliableDataReceived Called");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadDone Called");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadStart Called");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("OnSessionListUpdated Called");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutdown Called");
            SceneManager.LoadScene(CS_SceneNames.MAIN_LOOBY);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("OnUserSimulationMessage Called");
        }
    }
}