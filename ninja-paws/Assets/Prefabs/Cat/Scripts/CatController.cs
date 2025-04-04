using UnityEngine;

namespace Assets.Prefabs.Cat.Scripts
{
    public class CatController : MonoBehaviour
    {
        public enum AnimationLayer {
            BASE = 0,
            FIGHT = 1
        }
        private Animator animator;

        public void ChooseAnimationLayer(AnimationLayer layer)
        {
            animator.SetLayerWeight((int)AnimationLayer.BASE, 0);
            animator.SetLayerWeight((int)AnimationLayer.FIGHT, 0);
            animator.SetLayerWeight((int)layer, 1);
        }

        public void Stand()
        {
            animator.SetTrigger("stand");
            animator.SetFloat("speed", 0.0f);
        }

        public void Walk()
        {
            animator.SetFloat("speed", 0.1f);
        }

        /** LifeCycle **/
        void Awake()
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Cat Animator not found");
            }
        }
    }
}

