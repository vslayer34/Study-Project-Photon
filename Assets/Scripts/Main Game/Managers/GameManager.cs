using System;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


namespace PhotonCourse.Scripts.MainGame.Managers
{
    public class GameManager : NetworkBehaviour
    {
        public event Action OnMatchFinished;
        public static bool IsMatchOver;

        [SerializeField]
        private Camera _mainCamera;

        [SerializeField, Header("Match Timer Configs")]
        private TextMeshProUGUI _timerTextField;

        [SerializeField]
        private float _matchTimerInSeconds = 60.0f;

        [Networked]
        private TickTimer _MatchTimer { get; set; }



        // Game Loop Methods---------------------------------------------------------------------------

        private void Awake()
        {
            if (GlobalsManager.Instance != null)
            {
                GlobalsManager.Instance.GameManagerInstance = this;
            }
        }

        public override void Spawned()
        {
            IsMatchOver = false;
            _mainCamera.gameObject.SetActive(false);

            _MatchTimer = TickTimer.CreateFromSeconds(Runner, _matchTimerInSeconds);
        }

        public override void FixedUpdateNetwork()
        {
            if (!_MatchTimer.Expired(Runner) && _MatchTimer.RemainingTime(Runner).HasValue)
            {
                var timeSpan = TimeSpan.FromSeconds(_MatchTimer.RemainingTime(Runner).Value);
                _timerTextField.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            else if (_MatchTimer.Expired(Runner))
            {
                IsMatchOver = true;
                _MatchTimer = TickTimer.None;

                OnMatchFinished?.Invoke();
                Debug.Log("The round is over");
            }
        }
    }
}