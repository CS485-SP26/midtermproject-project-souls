using UnityEngine;
using Farming;
using TMPro;
using Core;
using UnityEngine.Events;
using System.Collections;

//using System.Diagnostics;

namespace Character
{
    public class PlayerFarming : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private FarmTileManager farmTileManager;
        private AnimatedController animatedController;

        [Header("Tools & Visuals")]
        [SerializeField] private GameObject waterCan;
        [SerializeField] private GameObject gardenHoe;
        [SerializeField] private ToolDurability hoeDurability;
        [SerializeField] private ToolDurability wateringCanDurability;
        [SerializeField] private TMP_Text toolBrokenMessage;

        [Header("Player Meter Stats")] //!!renamed to include stamina bar!!
        //[SerializeField] private SimpleProgressBar waterLevelBar; //!!potentially destructive removal of simpleprogressbar which we dont use anymore!!
        // [SerializeField] private int maxWaterLevel = 10;
        // [SerializeField] private int currentWaterLevel = 10;
        [SerializeField] private DepletingBar waterLevelUI; //eventually refactor this to water can, for water bar
        private float waterLevel = 1f;
        private float waterCap;
        [SerializeField] private float waterPerUse = 0.1f; //if we wanted to make upgrades, we can simply reduce this value even further
        [SerializeField] private DepletingBar staminaUI; //for stamina bar
        private float staminaLevel = 1f;
        [SerializeField] private float staminaDepletion = 0.1f; //worded differently but same idea as waterPerUse
        [SerializeField] float staminaRegenTimer = 2f;
        private float originalTimer;
        private float staminaCap;
        private bool isRegenerating = false;

        [Header("Funds")]
        [SerializeField] private TMP_Text fundsText;
        
        [Header("Seeds")]
        [SerializeField] private TMP_Text seedsText;
        [SerializeField] private int startingSeeds = 5;
        
        private GameObject currentToolInstance;
       
        [Header("Seeds")]
        [SerializeField] private GameObject[] plantPrefabs;
        public GameObject[] PlantPrefabs => plantPrefabs;
        private int selectedPlantIndex = 0;

        [Header("Events")] //added for stamina meter and quest checking
        public UnityEvent interacting = new UnityEvent();
        public UnityEvent staminaDrain = new UnityEvent();

        [Header("Audio")]
        [SerializeField] private AudioSource toolBreakAudio;

        bool busy = false; //determine when the player is currently doing something or not

        private void Start()
        {
            
            animatedController = GetComponent<AnimatedController>();
            originalTimer = staminaRegenTimer;
            staminaCap = staminaLevel;
            waterCap = 1f;
            
            if (GameManager.Instance != null)
            {
                waterLevel = GameManager.Instance.GetWaterLevel();
            }

            SetTool("None");
            waterLevelUI.SetFill(waterLevel);
            
            
            if (fundsText != null && GameManager.Instance != null)
            {
                fundsText.SetText("Funds: $" + GameManager.Instance.GetFunds());
            }
            
            if (seedsText != null && GameManager.Instance != null && GameManager.Instance.Seeds != null)
            {
                seedsText.SetText("Seeds: " + GameManager.Instance.Seeds.Get());
            }
            
        }

        public void AttemptInteraction(FarmTile tile)
        {
            if (tile == null) return;

            if(staminaLevel >= staminaDepletion)
            {
                switch (tile.GetCondition)
                {
                    case FarmTile.Condition.Grass:
                        if (!hoeDurability.UseTool())
                        {
                            ShowToolBroken();
                            toolBreakAudio?.Play();
                            return;
                        }
                        busy = true;
                        animatedController.SetTrigger("Till");
                        tile.Interact();
                        StartCoroutine(ClearBusyAfterAnimation("Till"));
                        break;
                    case FarmTile.Condition.Tilled: 
                        if (!wateringCanDurability.UseTool())
                        {
                            ShowToolBroken();
                            toolBreakAudio?.Play();
                            return;
                        }
                        if(waterLevel >= waterPerUse)
                        { 
                            busy = true;
                            animatedController.SetTrigger("Water"); 
                            tile.Interact();
                            waterLevel -= waterPerUse;
                            StartCoroutine(BarRolldown(waterLevelUI, waterLevel, waterLevel -= waterPerUse));
                            if (GameManager.Instance != null) GameManager.Instance.SetWaterLevel(waterLevel);
                            StartCoroutine(BarRolldown(staminaUI, staminaLevel, staminaLevel -= staminaDepletion));
                            interacting?.Invoke(); //added for tileevent checking
                        }
                        break;
                    // used chatGPT and stackoverflow for formatting/debugging help
                    case FarmTile.Condition.Watered:
                        if (GameManager.Instance == null || GameManager.Instance.Seeds == null)
                        {
                            Debug.LogError("SeedsManager missing");
                            return;
                        }
                        if (GameManager.Instance.Seeds.Get() > 0)
                        {
                            GameManager.Instance.Seeds.Set(GameManager.Instance.Seeds.Get() - 1);
                            animatedController.SetTrigger("Plant");
                            seedsText.SetText("Seeds: " + GameManager.Instance.Seeds.Get());
                            tile.Plant(plantPrefabs[selectedPlantIndex]);
                            staminaLevel -= 0.05f;
                            StartCoroutine(BarRolldown(staminaUI, staminaLevel, staminaLevel -= 0.05f));
                        }
                        break;
                    
                    case FarmTile.Condition.Planted:
                        animatedController.SetTrigger("Harvest");
                        tile.Interact();
                        break;

                    default: break;
                }
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
        
        private void FixedUpdate() //stamina regeneration logic
        {
            if(busy) //if busy, reset the timer
            { staminaRegenTimer = originalTimer; busy = false; }
            else if(!busy && staminaLevel < staminaCap && staminaRegenTimer > 0f) //else if not busy and stamina is below the max and the timer isnt already at 0
            { staminaRegenTimer -= Time.fixedDeltaTime; }
            if(staminaRegenTimer <= 0f)
            { StartCoroutine(RegenStamina()); }   
        }

        private IEnumerator RegenStamina()
        {
            if (isRegenerating) yield break;
            isRegenerating = true;

            while (staminaLevel < staminaCap)
            {
                staminaLevel = Mathf.MoveTowards(staminaLevel, staminaCap, Time.fixedDeltaTime / 2f);
                staminaUI.SetFill(staminaLevel);
                yield return null;
            }
            isRegenerating = false;
        }

        private IEnumerator BarRolldown(DepletingBar bar, float originalValue, float changeToValue) //visually show bars depleting or increasing gradually instead of all at once, look up coroutines unity to figure out how the updating works
        {
            float speed = 5f;

                while(originalValue != changeToValue)
                {
                    originalValue = Mathf.MoveTowards(originalValue, changeToValue, Time.fixedDeltaTime / speed);
                    bar.SetFill(originalValue);
                    yield return null;
                }
        }

        public IEnumerator RefillWater() //refills water, called by water pick up script
        {
            float speed = 2f;

            while(waterLevel != waterCap)
            {
                waterLevel = Mathf.MoveTowards(waterLevel, waterCap, Time.fixedDeltaTime * speed);
                waterLevelUI.SetFill(waterLevel);
                yield return null;
            }

            if (GameManager.Instance != null) GameManager.Instance.SetWaterLevel(waterLevel);

        }
        
        private IEnumerator ClearBusyAfterAnimation(string stateName)
        {
            // Wait for animator to enter the state
            yield return null;
            while (animatedController.IsPlayingState(stateName))
                yield return null;
            busy = false;
        }
        
        public void SelectNextPlant()
        {
            selectedPlantIndex = (selectedPlantIndex + 1) % plantPrefabs.Length;
            Debug.Log($"Selected: {plantPrefabs[selectedPlantIndex].name}");
        }

        public void SelectPlant(int index)
        {
            if (index >= 0 && index < plantPrefabs.Length)
                selectedPlantIndex = index;
        }

        void ShowToolBroken()
        {
            if (toolBrokenMessage == null) return;

            toolBrokenMessage.gameObject.SetActive(true);
            StartCoroutine(HideToolBroken());
        }

        IEnumerator HideToolBroken()
        {
            yield return new WaitForSeconds(2f);
            toolBrokenMessage.gameObject.SetActive(false);

        }
    }
}