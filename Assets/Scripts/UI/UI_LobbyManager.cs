using PhotonCourse.Scripts.Helper.Enums;
using UnityEngine;

namespace PhotonCourse.Scripts.UI
{
    public class UI_LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private LobbyPanalBase[] _lobbyPanals;

        [SerializeField]
        private GameObject _loadingCanvasPrefab;



        // Game Loop Methods-----------------------------------------------------------------------

        private void Start()
        {
            Instantiate(_loadingCanvasPrefab);
            
            foreach (var panal in _lobbyPanals)
            {
                panal.InitPanal(this);
            }
        }

        // Memeber Methods-------------------------------------------------------------------------

        public void ShowPanal(LobbyPanalType type)
        {
            foreach (var panal in _lobbyPanals)
            {
                panal.ShowPanal();
            }
        }
    }
}