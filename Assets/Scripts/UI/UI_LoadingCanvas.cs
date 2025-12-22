using System;
using PhotonCourse.Scripts.Helper.Constants;
using PhotonCourse.Scripts.Network;
using PhotonCourse.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;


namespace PhotonCourse.Scripts.UI
{
    public class UI_LoadingCanvas : MonoBehaviour
    {
        [SerializeField]
        private Button _cancelBtn;

        [SerializeField]
        private Animator _animator;

        private NetworkRunnerController _networkRunnerController;



        // Game Loop Methods-----------------------------------------------------------------------
        
        private void Start()
        {
            _networkRunnerController = GlobalsManager.Instance.NetwrokRunnerControllerInstance;

            _networkRunnerController.OnConnectionStablishedSuccessfully += ShowLoadingCanvas;
            _networkRunnerController.OnPlayerJoinedSuccessfully += HideLoadingCanvas;
            _cancelBtn.onClick.AddListener(_networkRunnerController.ShutDownConnection);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _networkRunnerController.OnConnectionStablishedSuccessfully -= ShowLoadingCanvas;
            _networkRunnerController.OnPlayerJoinedSuccessfully -= HideLoadingCanvas;
            _cancelBtn.onClick.RemoveListener(_networkRunnerController.ShutDownConnection);
        }

        // Memeber Methods-------------------------------------------------------------------------

        private void ShutDownConnection()
        {
            _networkRunnerController.ShutDownConnection();
        }

        // Signal Methods--------------------------------------------------------------------------

        private void ShowLoadingCanvas()
        {
            gameObject.SetActive(true);
            _animator.Play(Animations.UI.POP_IN);
        }

        private void HideLoadingCanvas()
        {
            StartCoroutine(Utility.SetUIPanalStateAfterAnimations(gameObject, _animator, Animations.UI.POP_OUT, false));
        }
    }
}