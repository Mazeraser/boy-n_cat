using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Infrastructure
{
    public class CharacterAnimationCommand : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings")]
        [LabelText("Animator")]
        private Animator animator;

        public bool SetAnimation(string animationName)
        {
            int hash = Animator.StringToHash(animationName);
            // Используем кэшированную ссылку
            if (animator.HasState(0, hash))
            {
                animator.Play(hash);
                return true;
            }
            Debug.LogError($"Animation is doesn't exist.\n Layer: {0}. ID: {hash}");
            return false;
        }
    }
}