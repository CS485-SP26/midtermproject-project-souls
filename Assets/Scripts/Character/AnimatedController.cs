using UnityEngine;

namespace Character {
    public class AnimatedController : MonoBehaviour
    {
        MovementController moveController;
        Animator animator;

        // Cache hashes once instead of string lookup every frame
        private static readonly int InputX = Animator.StringToHash("InputX");
        private static readonly int InputY = Animator.StringToHash("InputY");
        private static readonly int Speed  = Animator.StringToHash("Speed");

        [SerializeField] private float smoothing = 8f;
        private Vector2 smoothedInput;

        protected Animator Animator { get { return animator; } }

        void Start()
        {
            animator = GetComponent<Animator>();
            Debug.Assert(animator, "AnimatedController requires an Animator component");
            moveController = GetComponent<MovementController>();
            Debug.Assert(moveController, "AnimatedController requires a MovementController component");
        }

        public void Jump()
        {
            animator.SetTrigger("Jump");
        }

        public void SetTrigger(string name)
        {
            animator.SetTrigger(name);
        }

        void Update()
        {
            Vector2 rawInput = moveController.GetMovementVector();

            // Smooth the input so animations blend instead of snapping
            smoothedInput = Vector2.Lerp(smoothedInput, rawInput, Time.deltaTime * smoothing);

            animator.SetFloat(InputX, smoothedInput.x);
            animator.SetFloat(InputY, smoothedInput.y);
            animator.SetFloat(Speed, smoothedInput.sqrMagnitude); // useful for blend trees
        }

        public bool IsPlayingState(string stateName)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}