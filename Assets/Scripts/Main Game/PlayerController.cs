using System;
using Fusion;
using PhotonCourse.Scripts.Helper.Constants;
using PhotonCourse.Scripts.MainGame.Managers;
using TMPro;
using UnityEngine;


namespace PhotonCourse.Scripts.MainGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerWeaponController))]
    [RequireComponent(typeof(PlayerAnimatorController))]
    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {
        public enum PlayerInputButtons
        {
            None,
            Jump,
            Fire1
        }


        // Handles setting the player nickname
        [SerializeField]
        private TextMeshProUGUI _nameDisplayLabel;

        [Networked(OnChanged = nameof(OnNickNameChanged))]
        public NetworkString<_16> PlayerName { get; private set; }

        [Networked]
        public NetworkBool IsPlayerAlive { get; private set; }

        [Networked]
        private Vector2 _NextSpawnPosition { get; set; }


        private PlayerWeaponController _playerWeaponController;
        private PlayerAnimatorController _playerAnimController;
        private PlayerHealthController _playerHealthController;


        [SerializeField]
        private GameObject _cameraGroup;

        [SerializeField]
        private float _moveSpeed = 6.0f;

        [SerializeField]
        private float _jumpForce = 10.0f;

        [Networked]
        public NetworkButtons ButtonPrev { get; set; }

        [Networked]
        public TickTimer RespawnTimer { get; private set; }

        private Rigidbody2D _rigidBody;
        private float _horizontalInput;


        [SerializeField, Header("Grounded variables")]
        private LayerMask _groundLayerMask;

        [SerializeField]
        private Transform _groundDetector;

        [Networked]
        private NetworkBool _IsGrounded { get; set; }

        [Networked]
        private TickTimer _TimerToChangePosition { get; set; }



        // Network Methods-------------------------------------------------------------------------

        public override void Spawned()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerWeaponController = GetComponent<PlayerWeaponController>();
            _playerAnimController = GetComponent<PlayerAnimatorController>();
            _playerHealthController = GetComponent<PlayerHealthController>();

            SetLocalObject();

            IsPlayerAlive = true;
        }

        public override void FixedUpdateNetwork()
        {
            CheckRespawnTimer();

            if (Runner.TryGetInputForPlayer<PlayerInputData>(Object.InputAuthority, out var input) && AcceptInput)
            {
                _rigidBody.linearVelocity = new Vector2(input.HorizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
                CheckJumpInput(input);
            }

            _playerAnimController.UpdatePlayerLocalScale(_rigidBody.linearVelocity);
        }

        public override void Render()
        {
            // _playerAnimController.UpdateCharacterAnimations(_rigidBody.linearVelocity);
            if (!AcceptInput)
            {
                _playerAnimController.UpdateCharacterAnimations(Vector3.zero, false);
                return;
            }

            _playerAnimController.UpdateCharacterAnimations(_rigidBody.linearVelocity, _playerWeaponController.IsHoldingFireButton);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            GlobalsManager.Instance.ObjectPoolManagerInstance.RemoveGameobjectFromDictionary(Object);
            Destroy(gameObject);
        }

        // Member Methods--------------------------------------------------------------------------

        private void SetLocalObject()
        {
            if (Runner.LocalPlayer == Object.InputAuthority)
            {
                _cameraGroup.transform.SetParent(null);
                _cameraGroup.SetActive(true);
                var userName = GlobalsManager.Instance.NetwrokRunnerControllerInstance.PlayerName;
                RpcSetLocalPlayerName(userName);
            }
            else
            {
                // set other proxies to snap shots instead of predicted
                GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
            }
        }

        private void CheckJumpInput(PlayerInputData input)
        {
            // var lastPressed = input.NetworkButton.GetPressed(ButtonPrev);

            // Debug.Log("I'm Before Jumping!!!", this);

            // if (lastPressed.WasPressed(ButtonPrev, PlayerInputButtons.Jump))
            _IsGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox(_groundDetector.position, _groundDetector.localScale,0.0f, _groundLayerMask);


            if (_IsGrounded)
            {
                if (input.networkButton.WasPressed(ButtonPrev, PlayerInputButtons.Jump))
                {
                    _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                    Debug.Log("I'm Jumping!!!", this);
                }
            }

            ButtonPrev = input.networkButton;
        }

        public PlayerInputData GetPlayerNetworkData()
        {
            PlayerInputData playerInputData = new PlayerInputData
            {
                HorizontalInput = _horizontalInput,
                GunPivotRotation = _playerWeaponController.LocalPivotRotation
            };
            
            // playerInputData.HorizontalInput = _horizontalInput;
            playerInputData.networkButton.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
            playerInputData.networkButton.Set(PlayerInputButtons.Fire1, Input.GetButton(CS_Input.FIRE_1));

            return playerInputData;
        }

        private void SetLocalPlayerNameLabel(NetworkString<_16> playerName)
        {
            _nameDisplayLabel.text = $"{playerName} {Object.InputAuthority.PlayerId}";
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RpcSetLocalPlayerName(NetworkString<_16> playerName)
        {
            PlayerName = playerName;
        }

        public void KillPlayer()
        {
            const float TIME_TO_SPAWN = 5.0f;
            if (Runner.IsServer)
            {
                var playerSpawner = GlobalsManager.Instance.PlayerSpawnerControllerInstance;

                _NextSpawnPosition = playerSpawner.GetRandomSpawnPoint();
                _TimerToChangePosition = TickTimer.CreateFromSeconds(Runner, TIME_TO_SPAWN - 1.0f);
            }

            _rigidBody.simulated = false;
            _playerAnimController.UpdateDeathAnimations();
            IsPlayerAlive = false;
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, TIME_TO_SPAWN);
        }

        private void RespawnPlayer()
        {
            IsPlayerAlive = true;
            _rigidBody.simulated = true;
            // _rigidBody.position = _NextSpawnPosition;
            
            _playerHealthController.ResetHealthOnRespawn();
            _playerAnimController.UpdateRespawnAnimations();
        }

        private void CheckRespawnTimer()
        {
            if (AcceptInput)
            {
                return;
            }

            if (_TimerToChangePosition.Expired(Runner))
            {
                _TimerToChangePosition = TickTimer.None;
                GetComponent<NetworkRigidbody2D>().TeleportToPosition(_NextSpawnPosition);
            }
            if (RespawnTimer.Expired(Runner))
            {
                RespawnTimer = TickTimer.None;
                RespawnPlayer();
            }
        }

        // Signal Methods--------------------------------------------------------------------------

        private static void OnNickNameChanged(Changed<PlayerController> changed)
        {
            changed.Behaviour.SetLocalPlayerNameLabel(changed.Behaviour.PlayerName);
        }

        // Interface Methods-----------------------------------------------------------------------
        public void BeforeUpdate()
        {
            if (Runner.LocalPlayer == Object.InputAuthority && AcceptInput)
            {
                _horizontalInput = Input.GetAxis(CS_Input.HORIZONTAL);
            }
        }

        // Getters & Setters-----------------------------------------------------------------------

        public bool AcceptInput => IsPlayerAlive && !GameManager.IsMatchOver;
    }
}