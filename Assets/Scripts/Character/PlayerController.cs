using UnityEngine;
using UnityEngine.InputSystem;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerFarming))] // Ensure the farming script is attached
    public class PlayerController : MonoBehaviour
    {
        private MovementController moveController;
        private AnimatedController animatedController;
        private PlayerFarming playerFarming;

        void Start()
        {
            moveController = GetComponent<MovementController>();
            animatedController = GetComponent<AnimatedController>();
            playerFarming = GetComponent<PlayerFarming>();

            Debug.Assert(moveController, "PlayerController requires a MovementController");
            Debug.Assert(playerFarming, "PlayerController requires PlayerFarming script");
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
            if(animatedController) animatedController.Jump();
        }
        
        public void OnInteract(InputValue value)
        {
            // Delegate the logic to the farming script
            playerFarming.AttemptInteraction();
        }
    }
}