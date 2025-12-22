using Fusion;
using UnityEngine;


namespace PhotonCourse.Scripts.MainGame.Managers
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;



        // Game Loop Methods---------------------------------------------------------------------------

        public override void Spawned()
        {
            _mainCamera.gameObject.SetActive(false);
        }
    }
}