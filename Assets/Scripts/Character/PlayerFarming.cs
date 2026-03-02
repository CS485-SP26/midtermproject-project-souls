using Art.UI.ProgressBar;
using UnityEngine;
using Farming;
using TMPro;
using Core;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using System.Collections;
using System;
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

        [Header("Funds")]
        [SerializeField] private TMP_Text fundsText;

        private GameObject currentToolInstance;


        [Header("Events")] //added for stamina meter and quest checking
        public UnityEvent interacting = new UnityEvent();
        public UnityEvent staminaDrain = new UnityEvent();

        bool busy = false; //determine when the player is currently doing something or not

        private void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            originalTimer = staminaRegenTimer;
            staminaCap = staminaLevel;
            waterCap = waterLevel;
            
            // Initialize UI
            // UpdateWaterUI();
            fundsText.text = "Funds: $" + GameManager.Instance.GetFunds();

            SetTool("None");
            waterLevelUI.SetFill(waterLevel);
        }

        public void AttemptInteraction(FarmTile tile)
        {
            if (tile == null) return;

            if(staminaLevel >= staminaDepletion)
            {
                switch (tile.GetCondition)
                {
                    case FarmTile.Condition.Grass:
                        busy = true; 
                        animatedController.SetTrigger("Till"); 
                        tile.Interact();
                        staminaLevel -= staminaDepletion;
                        StartCoroutine(BarRolldown(staminaUI, staminaLevel, staminaLevel -= staminaDepletion));
                        break;
                    case FarmTile.Condition.Tilled: 
                        if(waterLevel >= waterPerUse)
                        { 
                            busy = true;
                            animatedController.SetTrigger("Water"); 
                            tile.Interact();
                            waterLevel -= waterPerUse;
                            StartCoroutine(BarRolldown(waterLevelUI, waterLevel, waterLevel -= waterPerUse));
                            staminaLevel -= staminaDepletion;
                            StartCoroutine(BarRolldown(staminaUI, staminaLevel, staminaLevel -= staminaDepletion));
                            interacting?.Invoke(); //added for tileevent checking
                        }
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

        private IEnumerator RegenStamina() //does NOT use a copy of the values passed into the function, changing stamina levels directly
        {
            float speed = 5f;

            while(staminaLevel != staminaCap)
            {
                staminaLevel = Mathf.MoveTowards(staminaLevel, staminaCap, Time.fixedDeltaTime / speed);
                staminaUI.SetFill(staminaLevel);
                yield return null;
            }
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

        }

    }
}