using Art.UI.ProgressBar;
using UnityEngine;
using Farming;
using TMPro;
using Core;

namespace Character
{
    public class PlayerFarming : MonoBehaviour
    {
        [Header("Dependencies")]
        //[SerializeField] private TileSelector tileSelector;
        [SerializeField] private FarmTileManager farmTileManager;
        private AnimatedController animatedController;

        [Header("Tools & Visuals")]
        [SerializeField] private GameObject waterCan;
        [SerializeField] private GameObject gardenHoe;

        [Header("Water Stats")]
        [SerializeField] private SimpleProgressBar waterLevelBar;
        // [SerializeField] private int maxWaterLevel = 10;
        // [SerializeField] private int currentWaterLevel = 10;

        [Header("Funds")]
        [SerializeField] private TMP_Text fundsText;

        private GameObject currentToolInstance;
        [SerializeField] private DepletingBar waterLevelUI; //eventually refactor this to water can
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.2f;

        private void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            
            // Initialize UI
            UpdateWaterUI();
            fundsText.text = "Funds: $" + GameManager.Instance.GetFunds();

            SetTool("None");
            waterLevelUI.SetFill(waterLevel);
        }

        public void AttemptInteraction(FarmTile tile)
        {
            if (tile == null) return;

            switch (tile.GetCondition)
            {
                case FarmTile.Condition.Grass: 
                    animatedController.SetTrigger("Till"); 
                    tile.Interact();
                    break;
                case FarmTile.Condition.Tilled: 
                    if(waterLevel >= waterPerUse)
                    {
                        animatedController.SetTrigger("Water"); 
                        tile.Interact();
                        waterLevel -= waterPerUse;
                        waterLevelUI.SetFill(waterLevel);
                    }
                    break;

                default: break;
            }
        }

        public void SetTool(string tool)
        {
            //want to make sure our tools are inactive by default
            waterCan.SetActive(false);
            gardenHoe.SetActive(false);

            switch(tool)
            {
                case "GardenHoe": gardenHoe.gameObject.SetActive(true); break;
                case "WaterCan": waterCan.gameObject.SetActive(true); break;
            }

        }    
    }
}