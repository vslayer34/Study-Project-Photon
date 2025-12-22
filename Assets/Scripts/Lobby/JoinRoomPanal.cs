using System;
using Fusion;
using PhotonCourse.Scripts.Network;
using PhotonCourse.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonCourse.Scripts.Lobby
{
    public class JoinRoomPanal : LobbyPanalBase
    {
        private const int ROOM_MINIMUM_LENGTH = 3;
        private NetworkRunnerController _networkRunnerController;

        [SerializeField, Header("Join Room Variables")]
        private Button _joinRoomBtn;

        [SerializeField]
        private Button _joinRoomByArgsBtn;

        [SerializeField]
        private Button _createRoomByArgsBtn;

        [SerializeField, Header("Input Fields")]
        private TMP_InputField _createRoomInputField;

        [SerializeField]
        private TMP_InputField _joinRoomInputField;



        // Game Loop- Methods----------------------------------------------------------------------

        // Member Methods--------------------------------------------------------------------------

        public override void InitPanal(UI_LobbyManager uiManager)
        {
            base.InitPanal(uiManager);

            _networkRunnerController = GlobalsManager.Instance.NetwrokRunnerControllerInstance;
            _joinRoomBtn.onClick.AddListener(OnClick_JoinRoom);
            _joinRoomByArgsBtn.onClick.AddListener(OnClick_JoinRoomByArgs);
            _createRoomByArgsBtn.onClick.AddListener(OnClick_CreateRoomByArgs);

            _createRoomByArgsBtn.interactable = false;
            _joinRoomByArgsBtn.interactable = false;

            _createRoomInputField.onValueChanged.AddListener((args) =>
            {
                _createRoomByArgsBtn.interactable = _createRoomInputField.text.Length >= ROOM_MINIMUM_LENGTH;
            });

            _joinRoomInputField.onValueChanged.AddListener((args) =>
            {
                _joinRoomByArgsBtn.interactable = _joinRoomInputField.text.Length >= ROOM_MINIMUM_LENGTH;
            });
        }

        private void LogIntoRoom(GameMode mode, string roomName)
        {
            if (roomName.Length >= ROOM_MINIMUM_LENGTH)
            {
                _networkRunnerController.StartGame(mode, roomName);
            }
        }

        // Signal Methods--------------------------------------------------------------------------

        private void OnClick_JoinRoom()
        {
            _networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
        }

        private void OnClick_JoinRoomByArgs()
        {
            LogIntoRoom(GameMode.Client, _joinRoomInputField.text);
        }

        private void OnClick_CreateRoomByArgs()
        {
            LogIntoRoom(GameMode.Host, _createRoomInputField.text);
        }
    }
}