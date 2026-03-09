using UnityEngine;
using UnityEngine.InputSystem;
using Farming;
using System.Collections.Generic;
using Quest;
namespace Character 
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerFarming))] // Ensure the farming script is attached
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TileSelector tileSelector;
        public QuestUI questUI;
        private MovementController moveController;
        private AnimatedController animatedController;
        private PlayerFarming playerFarming;
        private PlayerFishing playerFishing;
        public IInteractable currentInteractable;
        public bool canMove = true;

        [SerializeField] public GameObject fishingRod;

        void Start()
        {
            moveController = GetComponent<MovementController>();
            animatedController = GetComponent<AnimatedController>();
            playerFarming = GetComponent<PlayerFarming>();
            playerFishing = GetComponent<PlayerFishing>();

            //hide fishing rod on start
            fishingRod.SetActive(false);

            Debug.Assert(moveController, "PlayerController requires a MovementController");
            Debug.Assert(playerFarming, "PlayerController requires PlayerFarming script");
            Debug.Assert(tileSelector, "PlayerController requires a TileSelector.");
        }

        public void OnMove(InputValue inputValue)
        {
            if(!canMove)
            {
                moveController.Move(Vector2.zero);
                return;
            }; // disables player movement for inside UI menus --> modified to still allow interaction, specifically for fishing

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

            // if interacting with a tile, try farming
            if (tile != null)
            {
                playerFarming.AttemptInteraction(tile);
                return;
            }

            // otherwise, try fishing
            if (playerFishing != null)
            {
                playerFishing.TryFish();
            }

        }


        // For opening UI's
        private void OnOpen(InputValue value)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
                return;
            }

            questUI.Open();
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
