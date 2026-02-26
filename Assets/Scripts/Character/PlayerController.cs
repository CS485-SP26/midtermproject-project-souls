using UnityEngine;
using UnityEngine.InputSystem;
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
            FarmTile tile = tileSelector.GetSelectedTile(); //you HAVE to make sure the tile selector is set to player (raycaster) in the inspector!
            //Debug.Log(tileSelector.GetSelectedTile().gameObject.name + " recognized for OnInteract");
            List<IWaterable> waterable = tileSelector.GetSelectionOfType<IWaterable>();
            Debug.Log(waterable);
            playerFarming.AttemptInteraction(tile);
        }
    }
}