using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Farming;

namespace Character
{
    public class BoxSelector : TileSelector
    {
        [Header("Settings")]
        [SerializeField] private LayerMask tileLayer; // Assign this to the layer your tiles are on!
        
        private Vector2 startMousePos;
        private bool isDragging;
        
        // Create a mathematical plane at Y=0 (or your ground height)
        // This ensures the raycast always hits "ground" even if you drag off the map.
        private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        private void Update()
        {
            // 1. Detect Mouse Down
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                startMousePos = Mouse.current.position.ReadValue();
                isDragging = true;
            }
            
            // 2. Detect Mouse Up (Perform Selection)
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (isDragging)
                {
                    PerformBoxSelection();
                    isDragging = false;
                }
            }
        }

        private void PerformBoxSelection()
        {
            Vector2 endMousePos = Mouse.current.position.ReadValue();
            
            // Convert the 2D screen points (Start and End) to 3D world points
            Vector3 worldStart = GetWorldPositionOnPlane(startMousePos);
            Vector3 worldEnd = GetWorldPositionOnPlane(endMousePos);

            // Calculate the Center and Size of the detection box
            Vector3 center = (worldStart + worldEnd) / 2f;
            
            // Calculate size (absolute difference). 
            // We set Y to 10f to ensure the box is tall enough to hit the tiles.
            Vector3 size = new Vector3(
                Mathf.Abs(worldStart.x - worldEnd.x), 
                10f, 
                Mathf.Abs(worldStart.z - worldEnd.z)
            );
            
            Vector3 halfExtents = size / 2f;

            // Use OverlapBox to find all colliders inside that volume
            Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, tileLayer);
            
            List<FarmTile> selectedTiles = new List<FarmTile>();

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<FarmTile>(out FarmTile tile))
                {
                    selectedTiles.Add(tile);
                }
            }

            Debug.Log($"Box Selected {selectedTiles.Count} tiles.");
            
            // TODO: You will need to update your base class or controller to handle a LIST of tiles.
            // For example: SetSelectedTiles(selectedTiles);
        }

        // Helper: Casts a ray from camera to the mathematical ground plane
        private Vector3 GetWorldPositionOnPlane(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            
            // Raycast against the math plane, not physics colliders (more robust for dragging)
            if (groundPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

        // 3. Draw the Visual Box (Using OnGUI for simplicity)
        private void OnGUI()
        {
            if (isDragging)
            {
                Vector2 endMousePos = Mouse.current.position.ReadValue();
                Rect rect = GetScreenRect(startMousePos, endMousePos);
                
                // Draw a simple box. You can replace this with a UI Image later.
                GUI.Box(rect, ""); 
            }
        }

        // Helper: Converts two mouse points into a valid Rect (handles dragging backwards)
        private Rect GetScreenRect(Vector2 pos1, Vector2 pos2)
        {
            // Screen space origin is Bottom-Left, but GUI is Top-Left. Flip Y.
            pos1.y = Screen.height - pos1.y;
            pos2.y = Screen.height - pos2.y;

            Vector2 topLeft = Vector2.Min(pos1, pos2);
            Vector2 bottomRight = Vector2.Max(pos1, pos2);

            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
    }
}