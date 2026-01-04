using System;
using Fusion;
using PhotonCourse.Scripts.Helper.Constants;
using TMPro;
using UnityEngine;

namespace PhotonCourse.Scripts.MainGame.Managers
{
    public class PlayerChatController : NetworkBehaviour
    {
        public static bool IsTyping { get; private set; }
        [SerializeField]
        private TMP_InputField _chatInputField;

        [SerializeField]
        private TextMeshProUGUI _bubbleChatText;

        [SerializeField]
        private Animator _ChatBubbleAnimator;

        private int _popInHash = Animator.StringToHash(Animations.ChatBubble.POP_IN);



        // Network Methods-------------------------------------------------------------------------

        public override void Spawned()
        {
            // IsTyping = false;
            bool isLoclaPlayer = Object.InputAuthority == Runner.LocalPlayer;
            gameObject.SetActive(isLoclaPlayer);

            if (isLoclaPlayer)
            {
                _chatInputField.onSelect.AddListener(x =>
                {
                    IsTyping = true;
                });
                _chatInputField.onDeselect.AddListener(x =>
                {
                    IsTyping = false;
                });
                _chatInputField.onSubmit.AddListener(OnTextSubmitted);
            }
        }

        // Member Methods--------------------------------------------------------------------------

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RpcSetBubbleSpeech(NetworkString<_64> text)
        {
            _bubbleChatText.text = text.Value;
            // _chatInputField.text = string.Empty;

            _ChatBubbleAnimator.SetTrigger(_popInHash);
        }

        // Signal Methods--------------------------------------------------------------------------

        private void OnTextSubmitted(string arg0)
        {
            if (!string.IsNullOrEmpty(arg0))
            {
                RpcSetBubbleSpeech(arg0);
            }
        }
    }
}
