using UnityEngine;


namespace PhotonCourse.Scripts.Utilities
{
    public class DDOL : MonoBehaviour
    {
        // Game Loop Methods-----------------------------------------------------------------------
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}