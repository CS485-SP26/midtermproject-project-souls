using Art.UI.ProgressBar;
using UnityEngine;
using Farming;

namespace Character
{
    public class PlayerFarming : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private FarmTileManager farmTileManager;
        private AnimatedController animatedController;

        [Header("Tools & Visuals")]
        [SerializeField] private GameObject wateringCanMesh;
        [SerializeField] private GameObject handAttachmentPoint;
        [SerializeField] private GameObject hoeMesh;
        [SerializeField] private GameObject hoeHandAttachmentPoint;

        [Header("Water Stats")]
        [SerializeField] private SimpleProgressBar waterLevelBar;
        [SerializeField] private int maxWaterLevel = 10;
        [SerializeField] private int currentWaterLevel = 10;

        private GameObject currentToolInstance;

        private void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            
            // Initialize UI
            UpdateWaterUI();
        }

        public void AttemptInteraction()
        {
            FarmTile tile = tileSelector.GetSelectedTile();
            if (tile == null) return;

            if (tile.GetCondition == FarmTile.Condition.Grass)
            {
                EquipTool(hoeMesh, hoeHandAttachmentPoint);
                animatedController.SetTrigger("Till");
            }
            else if (tile.GetCondition == FarmTile.Condition.Tilled)
            {
                if (currentWaterLevel <= 0)
                {
                    Debug.Log("Not enough water!");
                    return;
                }

                EquipTool(wateringCanMesh, handAttachmentPoint);
                animatedController.SetTrigger("Water");
            }

            // Update global manager (if needed immediately on click)
            if(farmTileManager) farmTileManager.UpdateProgressBar();
        }

        private void EquipTool(GameObject toolPrefab, GameObject attachPoint)
        {
            // Clean up old tool
            if (currentToolInstance != null) Destroy(currentToolInstance);

            // Create new tool
            currentToolInstance = Instantiate(toolPrefab, attachPoint.transform);
            currentToolInstance.transform.localPosition = Vector3.zero;
            currentToolInstance.transform.localRotation = Quaternion.identity;
        }

        private void UpdateWaterUI()
        {
            if (waterLevelBar != null)
            {
                waterLevelBar.SetProgress(currentWaterLevel, 0, maxWaterLevel);
            }
        }

        // ---------------------------------------------------------
        // ANIMATION EVENT RECEIVERS
        // These function names match the strings called by your Animation Events
        // ---------------------------------------------------------

        public void WaterTile()
        {
            FarmTile tile = tileSelector.GetSelectedTile();
            if (tile != null)
            {
                tile.Water();
                currentWaterLevel--;
                UpdateWaterUI();
            }
        }

        public void TillTile()
        {
            FarmTile tile = tileSelector.GetSelectedTile();
            if (tile != null)
            {
                tile.Till();
            }
        }
    }
}