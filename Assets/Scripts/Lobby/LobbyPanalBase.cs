using PhotonCourse.Scripts.Helper.Constants;
using PhotonCourse.Scripts.Helper.Enums;
using PhotonCourse.Scripts.UI;
using PhotonCourse.Scripts.Utilities;
using UnityEngine;

public abstract class LobbyPanalBase : MonoBehaviour
{
    [field: SerializeField, Header("Base Variables")]
    public LobbyPanalType PanalType { get; protected set; }

    [SerializeField]
    private Animator _animator;

    protected UI_LobbyManager _uiManager;



    // Member Methods------------------------------------------------------------------------------

    public virtual void InitPanal(UI_LobbyManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void ShowPanal()
    {
        gameObject.SetActive(true);
        _animator.Play(Animations.UI.POP_IN);
    }

    protected void DisablePanal()
    {
        StartCoroutine(Utility.SetUIPanalStateAfterAnimations(gameObject, _animator, Animations.UI.POP_OUT, false));
    }
}
