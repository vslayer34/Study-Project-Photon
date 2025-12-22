using Fusion;
using UnityEngine;

namespace PhotonCourse.Scripts.MainGame.Managers
{
    public struct PlayerInputData : INetworkInput
    {
        public float HorizontalInput { get; set; }
        public Quaternion GunPivotRotation { get; set; }
        public NetworkButtons networkButton;
    }
}