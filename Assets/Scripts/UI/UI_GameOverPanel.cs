using System;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonCourse.Scripts.UI
{
    public class UI_GameOverPanel : MonoBehaviour
    {
        [SerializeField]
        private Button _backToLobbyBtn;

        [SerializeField]
        private GameObject _childObject;



        // Game Loop Methods-----------------------------------------------------------------------

        private void Start()
        {
            GlobalsManager.Instance.GameManagerInstance.OnMatchFinished += ShowSelf;

            _backToLobbyBtn.onClick.AddListener(() =>
            {
                GlobalsManager.Instance.NetwrokRunnerControllerInstance.ShutDownConnection();
            });
        }

        private void OnDestroy()
        {
            GlobalsManager.Instance.GameManagerInstance.OnMatchFinished -= ShowSelf;
        }

        // Signal Methods--------------------------------------------------------------------------

        private void ShowSelf()
        {
            _childObject.SetActive(true);
        }
    }
}