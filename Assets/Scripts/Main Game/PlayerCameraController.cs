using PhotonCourse.Scripts.MainGame.Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace PhotonCourse.Scripts.MainGame
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineImpulseSource _cameraImpulseSource;

        [SerializeField]
        private CinemachineConfiner2D _confinerComponent;


        // Game Loop Methods-----------------------------------------------------------------------

        private void Start()
        {
            _confinerComponent.BoundingShape2D = GlobalsManager.Instance.GameManagerInstance.CameraBoundsCollider;
        }

        // Member Methods------------------------------------------------------------------------------

        public void ShakeCamera()
        {
            float force = Random.Range(0.2f, 1.0f);
            _cameraImpulseSource.GenerateImpulseWithForce(force);
        }
        public void ShakeCamera(Vector3 shakeAmount) => _cameraImpulseSource.GenerateImpulse(shakeAmount);
        public void ShakeCamera(float force)
        {
            _cameraImpulseSource.GenerateImpulse(force);
        }
    }
}
