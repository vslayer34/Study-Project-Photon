using PhotonCourse.Scripts.Helper.Constants;
using Unity.VisualScripting;
using UnityEngine;


namespace PhotonCourse.Scripts.MainGame
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        [SerializeField, Header("Transforms to flip its scale")]
        private Transform _gunPivot;

        [SerializeField]
        private Transform _playerInfoCanvas;

        [SerializeField, Header("Player Visuals Animator")]
        private Animator _animator;

        private readonly int _isWalkingHash = Animator.StringToHash(Animations.Player.IS_WALKING);
        private readonly int _isShootingHash = Animator.StringToHash(Animations.Player.IS_SHOOTING);

        private int _dieTriggerHash = Animator.StringToHash(Animations.Player.DIE);
        private int _respawnTriggerHash = Animator.StringToHash(Animations.Player.RESPAWN);

        private Vector3 _originalScale;
        private Vector3 _gunPivotOriginalScale;
        private Vector3 _playerInfoCanvasOriginalScale;

        private bool _facingRight = true;
        private bool _scriptInitialized;



        // Game Loop Methods---------------------------------------------------------------------------

        private void Start()
        {
            _originalScale = transform.localScale;
            _gunPivotOriginalScale = _gunPivot.localScale;
            _playerInfoCanvasOriginalScale = _playerInfoCanvas.localScale;
            
            const int SHOOTING_LAYER_INDEX = 1;
            _animator.SetLayerWeight(SHOOTING_LAYER_INDEX, 1.0f);
            
            _scriptInitialized = true;
        }

        // Memeber Methods-----------------------------------------------------------------------------

        public void UpdateCharacterAnimations(Vector2 velocity)
        {
            if (!_scriptInitialized)
            {
                return;
            }

            bool isWalikng = velocity.x > 0.1f || velocity.x < -0.1f;

            _animator.SetBool(_isWalkingHash, isWalikng);
        }

        public void UpdateCharacterAnimations(Vector2 velocity, bool isShooting)
        {
            if (!_scriptInitialized)
            {
                return;
            }

            bool isWalikng = velocity.x > 0.1f || velocity.x < -0.1f;

            _animator.SetBool(_isWalkingHash, isWalikng);
            _animator.SetBool(_isShootingHash, isShooting);
        }

        public void UpdatePlayerLocalScale(Vector2 velocity)
        {
            if (!_scriptInitialized)
            {
                return;
            }

            if (velocity.x != 0.0f)
            {
                _facingRight = velocity.x > 0.1f;
            }

            

            SetGameObjectLocalScale(transform, _originalScale);
            SetGameObjectLocalScale(_gunPivot, _gunPivotOriginalScale);
            SetGameObjectLocalScale(_playerInfoCanvas, _playerInfoCanvasOriginalScale);
        }

        private void SetGameObjectLocalScale(Transform targetTransform, Vector3 originalScale)
        {
            var correctedXValue = _facingRight ? originalScale.x : -1.0f * originalScale.x;
            targetTransform.localScale = new Vector3(correctedXValue, originalScale.y, originalScale.z);
        }

        public void UpdateDeathAnimations() => _animator.SetTrigger(_dieTriggerHash);
        public void UpdateRespawnAnimations() => _animator.SetTrigger(_respawnTriggerHash);
    }
}