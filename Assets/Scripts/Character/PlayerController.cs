using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Farming;
using System.Collections.Generic;
namespace Character 
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerFarming))] // Ensure the farming script is attached
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TileSelector tileSelector;
        private MovementController moveController;
        private AnimatedController animatedController;
        private PlayerFarming playerFarming;

        public IInteractable currentInteractable;
        public bool canMove = true;

        void Start()
        {
            moveController = GetComponent<MovementController>();
            animatedController = GetComponent<AnimatedController>();
            playerFarming = GetComponent<PlayerFarming>();

            Debug.Assert(moveController, "PlayerController requires a MovementController");
            Debug.Assert(playerFarming, "PlayerController requires PlayerFarming script");
            Debug.Assert(tileSelector, "PlayerController requires a TileSelector.");
        }

        public void OnMove(InputValue inputValue)
        {
            if(!canMove) {return;}; // disables player movement for inside UI menus
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            if(!canMove) return;   // disable player movement for inside UI menus
            moveController.Jump();
            if(animatedController) animatedController.Jump();
        }
        
        private void OnInteract(InputValue value)
        {
            // Delegate the logic to the farming script
            FarmTile tile = tileSelector.GetSelectedTile(); //you HAVE to make sure the tile selector is set to player (raycaster) in the inspector!
            //Debug.Log(tileSelector.GetSelectedTile().gameObject.name + " recognized for OnInteract");
            List<IWaterable> waterable = tileSelector.GetSelectionOfType<IWaterable>();
            //Debug.Log(waterable);
            playerFarming.AttemptInteraction(tile);
        }

        private void OnOpen(InputValue value)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                return;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();

            if (interactable != null && interactable == currentInteractable)
            {
                currentInteractable = null;
            }            
        }
    }
}
