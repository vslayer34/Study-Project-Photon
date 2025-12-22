using System;
using Fusion;
using PhotonCourse.Scripts.Helper.Enums;
using PhotonCourse.Scripts.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonCourse.Scripts.Lobby
{
    public class CreateNickNamePanal : LobbyPanalBase
    {
        [SerializeField, Header("Panal Variabiles")]
        private Button _createBtn;

        [SerializeField]
        private TMP_InputField _inputField;

        private const int MIN_NUMBER_OF_CHARACTERS = 3;



        // Game Loop Methods---------------------------------------------------------------------------

        private void OnDestroy()
        {
            _createBtn.onClick.RemoveListener(SubmitName);
            _inputField.onValueChanged.RemoveListener(ActivateCreateButton);
        }

        private void ActivateCreateButton(string inputText)
        {
            if (_inputField.text.Length >= MIN_NUMBER_OF_CHARACTERS)
            {
                _createBtn.interactable = true;
            }
        }

        // Memeber Methods-----------------------------------------------------------------------------

        public override void InitPanal(UI_LobbyManager uiManager)
        {
            base.InitPanal(uiManager);

            _createBtn.interactable = false;
            _createBtn.onClick.AddListener(SubmitName);
            _inputField.onValueChanged.AddListener(ActivateCreateButton);
        }

        // Signal Methods------------------------------------------------------------------------------

        private void SubmitName()
        {
            var nickName = _inputField.text;

            GlobalsManager.Instance.NetwrokRunnerControllerInstance.SetPlayerNickName(nickName);
            DisablePanal();
            _uiManager.ShowPanal(LobbyPanalType.JoinRoom);
        }
    }
}
