using PhotonCourse.Scripts.MainGame.Managers;
using PhotonCourse.Scripts.Network;
using UnityEngine;

public class GlobalsManager : MonoBehaviour
{
    public static GlobalsManager Instance { get; private set; }

    [SerializeField, Header("DDOL Parent Object")]
    private GameObject _parentObj;

    [field: SerializeField, Header("Instances")]
    public NetworkRunnerController NetwrokRunnerControllerInstance { get; private set; }
    public PlayerSpawnerController PlayerSpawnerControllerInstance { get; set; }
    public ObjectPoolManager ObjectPoolManagerInstance { get; set; }
    public GameManager GameManagerInstance { get; set; }




    // Game Loop Methods---------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(_parentObj);
        }
    }

    // Memeber Methods-----------------------------------------------------------------------------
}
