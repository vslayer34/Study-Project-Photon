using Fusion;
using PhotonCourse.Scripts.MainGame;
using TMPro;
using UnityEngine;

namespace PhotonCourse.Scripts.UI
{
    public class UI_RespawnPanal : SimulationBehaviour
    {
        [SerializeField, Header("Required Components")]
        private PlayerController _playerController;

        [SerializeField]
        private GameObject _panalObject;

        [SerializeField, Header("UI Components")]
        private TextMeshProUGUI _remainingTimeLabel;




        // Simulation Loop Methods-----------------------------------------------------------------

        public override void FixedUpdateNetwork()
        {
            if (_playerController.Object.InputAuthority == Runner.LocalPlayer)
            {
                var isRespawnTimerRunning = _playerController.RespawnTimer.IsRunning;
                _panalObject.SetActive(isRespawnTimerRunning);


                if (isRespawnTimerRunning && _playerController.RespawnTimer.RemainingTime(Runner).HasValue)
                {
                    var remainingTime = _playerController.RespawnTimer.RemainingTime(Runner).Value;

                    var _remaingTimeInt = Mathf.FloorToInt(remainingTime);

                    _remainingTimeLabel.text = _remaingTimeInt.ToString();
                }
            }
        }

        // Member Methods--------------------------------------------------------------------------
    }
}
