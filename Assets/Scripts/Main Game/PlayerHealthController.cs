using Fusion;
using PhotonCourse.Scripts.Helper.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonCourse.Scripts.MainGame
{
    public class PlayerHealthController : NetworkBehaviour
    {
        [SerializeField]
        private PlayerCameraController _playerCameraController;

        [SerializeField]
        private PlayerController _playerController;

        [SerializeField]
        private Animator _bloodScreenAnimator;

        [SerializeField]
        private Image _fillImage;

        [SerializeField]
        private TextMeshProUGUI _healthAmountLabel;

        [Networked(OnChanged = nameof(OnHealthAmountChanged))]
        private int _CurrentHealthAmount { get; set; }

        private const int MAX_HEALTH_AMOUNT = 100;

        [SerializeField]
        private Collider2D _collider;

        [SerializeField]
        private LayerMask _fallDetectorLayerMask;



        // Network Loop Methods--------------------------------------------------------------------

        public override void Spawned()
        {
            _collider = GetComponent<Collider2D>();
            _CurrentHealthAmount = MAX_HEALTH_AMOUNT;
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer && _playerController.IsPlayerAlive)
            {
                var hit = Runner.GetPhysicsScene2D().OverlapBox(transform.position, _collider.bounds.size, 0.0f, _fallDetectorLayerMask);

                if (hit != default)
                {
                    Debug.Log("Sensie HIT FASH5!!!");
                    Rpc_TakeDamage(MAX_HEALTH_AMOUNT);
                }
            }
        }

        // Member Methods--------------------------------------------------------------------------

        private void UpdateHealthUI(int newAmount)
        {
            newAmount = Mathf.Clamp(newAmount, 0, MAX_HEALTH_AMOUNT);
            _fillImage.fillAmount = Mathf.Lerp(0.0f, 1.0f,(float)newAmount / MAX_HEALTH_AMOUNT);

            _healthAmountLabel.text = newAmount.ToString();

            if (_CurrentHealthAmount <= 0)
            {
                _playerController.KillPlayer();
            }
        }

        private void TransferDamageToHealth(int currentHealth)
        {
            var localPlayer = Runner.LocalPlayer == Object.InputAuthority;

            if (localPlayer)
            {
                // TODO - Do blood, camera shake,.... etc
                Debug.Log("Player got hit ouch");

                _playerCameraController.ShakeCamera();
                _bloodScreenAnimator.Play(Animations.BloodScreen.BLOOD_SCREEN_ANIME_NAME);

                if (currentHealth <= 0)
                {
                    Debug.Log($"Sensie Current Health Amount {_CurrentHealthAmount}");
                    // TODO - Kill the player
                    _playerCameraController.ShakeCamera(2.0f);
                    // _playerController.KillPlayer();

                    Debug.Log("Oooh Nooo I'm dead");
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
        public void Rpc_TakeDamage(int damage)
        {
            _CurrentHealthAmount -= damage;
        }

        public void ResetHealthOnRespawn() => _CurrentHealthAmount = MAX_HEALTH_AMOUNT;

        // Signal Methods--------------------------------------------------------------------------

        private static void OnHealthAmountChanged(Changed<PlayerHealthController> changed)
        {
            changed.LoadOld();
            var oldHealthState = changed.Behaviour._CurrentHealthAmount;

            changed.LoadNew();
            var newHealthState = changed.Behaviour._CurrentHealthAmount;

            if (oldHealthState != newHealthState)
            {
                changed.Behaviour.UpdateHealthUI(newHealthState);

                // Did not respawn or just spawned
                if (newHealthState != MAX_HEALTH_AMOUNT)
                {
                    changed.Behaviour.TransferDamageToHealth(newHealthState);
                }
            }
        }
    }
}
