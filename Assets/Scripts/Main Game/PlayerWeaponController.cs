using Fusion;
using PhotonCourse.Scripts.MainGame.Managers;
using UnityEngine;
using static PhotonCourse.Scripts.MainGame.PlayerController;


namespace PhotonCourse.Scripts.MainGame
{
    public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
    {
        [SerializeField]
        private Camera _localCamera;

        [SerializeField]
        private Transform _gunPivot;

        [SerializeField]
        private float _delayBetweenShots = 0.18f;

        [SerializeField]
        private ParticleSystem _muzzleVFX;


        public Quaternion LocalPivotRotation { get; private set; } 

        [Networked]
        private Quaternion CurrentPlayerPivotRotation { get; set; }

        [Networked]
        private NetworkButtons ButtonPrev { get; set; }

        [Networked(OnChanged = nameof(OnPlayMuzzleVFXChanged))]
        private NetworkBool ShouldPlayMuzzleVFX { get; set; }

        [Networked, HideInInspector]
        public NetworkBool IsHoldingFireButton { get; set; }

        private TickTimer _shootCoolDown;


        [SerializeField, Header("Bullet Prefab")]
        private NetworkPrefabRef _bulletPrefab;

        [SerializeField]
        private Transform _bulletSpawnPoint;

        private PlayerController _playerController;



        // Network Methods-----------------------------------------------------------------------------

        public override void Spawned()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input))
            {
                CheckShootInput(input);
                CurrentPlayerPivotRotation = input.GunPivotRotation;

                ButtonPrev = input.networkButton;
            }

            _gunPivot.rotation = CurrentPlayerPivotRotation;
        }

        // Member Methods--------------------------------------------------------------------------

        private void CheckShootInput(PlayerInputData input)
        {
            var currentButtons = input.networkButton.GetPressed(ButtonPrev);
            // if (input.networkButton.WasPressed(ButtonPrev, PlayerInputButtons))
            // if (currentButtons.WasReleased(ButtonPrev, PlayerInputButtons.Fire1) && 
            //     _shootCoolDown.ExpiredOrNotRunning(Runner))
            
            IsHoldingFireButton = input.networkButton.IsSet(PlayerInputButtons.Fire1);

            if (input.networkButton.IsSet(PlayerInputButtons.Fire1) && 
                _shootCoolDown.ExpiredOrNotRunning(Runner)  && _playerController.AcceptInput)
            {
                _shootCoolDown = TickTimer.CreateFromSeconds(Runner, _delayBetweenShots);
                //TODO - Shoot

                ShouldPlayMuzzleVFX = true;
                // Debug.Log("Shoot");


                Runner.Spawn(_bulletPrefab, _bulletSpawnPoint.transform.position, _bulletSpawnPoint.transform.rotation, Object.InputAuthority);
            }
            else
            {
                //TODO - Stop Shooting
                ShouldPlayMuzzleVFX = false;

                // Debug.Log("Stop Shooting");
            }
        }

        private void SetMuzzleEffect(bool active)
        {
            if (active)
            {
                _muzzleVFX.Play();
            }
            else
            {
                _muzzleVFX.Stop();
            }
        }

        // Interface Methods-----------------------------------------------------------------------

        public void BeforeUpdate()
        {
            if (Runner.LocalPlayer == Object.InputAuthority && _playerController.AcceptInput)
            {
                var direction = _localCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;

                var angleInDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                LocalPivotRotation = Quaternion.AngleAxis(angleInDegrees, Vector3.forward);
            }
        }

        // Signal Methods--------------------------------------------------------------------------

        private static void OnPlayMuzzleVFXChanged(Changed<PlayerWeaponController> changed)
        {
            changed.LoadOld();
            var oldState = changed.Behaviour.ShouldPlayMuzzleVFX;

            changed.LoadNew();
            var currentState = changed.Behaviour.ShouldPlayMuzzleVFX;

            if (currentState != oldState)
            {
                changed.Behaviour.SetMuzzleEffect(changed.Behaviour.ShouldPlayMuzzleVFX);
            }
        }
    }
}