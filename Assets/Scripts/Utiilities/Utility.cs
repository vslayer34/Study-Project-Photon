using System.Collections;
using UnityEngine;

namespace PhotonCourse.Scripts.Utilities
{
    public static class Utility
    {
        public static IEnumerator SetUIPanalStateAfterAnimations(GameObject gameObject, Animator anim, string animationClip, bool targetState = true)
        {
            anim.Play(animationClip);
            var waitingTime = anim.GetCurrentAnimatorClipInfo(0).Length;

            yield return new WaitForSeconds(waitingTime);

            gameObject.SetActive(targetState);
        }
    }
}
