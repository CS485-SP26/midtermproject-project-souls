using UnityEngine;
using Farming;
using UnityEngine.InputSystem;

namespace Character
{
    public class RaycastSelector : TileSelector
    {
        private void OnInteract()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<FarmTile>(out FarmTile tile))
                {
                    SetActiveTile(tile);
                }
            }
        }
    }
    
}
