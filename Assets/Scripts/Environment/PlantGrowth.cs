using Core;
using Farming;
using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

namespace Environment
{

    public class PlantGrowth : MonoBehaviour, IHarvestable
    {
        [Header("Events")]
        // Drag and drop functions here in the Inspector
        public UnityEvent<Vector3> OnSizeChanged;

        [Header("Object References")] [SerializeField]
        DayController dayController;

        private float dayDivisionSeconds;
        
        [SerializeField] private int growthDays = 3;
        private int currentGrowth = 0;

        [SerializeField] private Transform visual;
        

        [SerializeField] private int cropValue = 10;
        public int GetCropValue() { return cropValue; }
        
        // Mesh size
        [Header("Size Constraints")] [SerializeField]
        private Vector3 minSize = new Vector3(0.1f, 0.1f, 0.1f);

        [SerializeField] private Vector3 maxSize = new Vector3(1f, 1f, 1f);
        [SerializeField] private int stepsPerDay = 10;

        [SerializeField] private Vector2 temperatureRange = new Vector2(0f, 30f); //  temperature range for growth
        [SerializeField] private Vector2 moistureRange = new Vector2(0.2f, 0.8f); //  moisture range for growth
        [SerializeField] private Vector2 sunlightRange = new Vector2(0.5f, 1f); //  sunlight range for growth
        
        [Header("Quality Settings")]
        [SerializeField] private float minQualityMultiplier = 0.3f; // worst case: 30% value

        private float accumulatedQuality = 0f;
        private int qualitySamples = 0;

        
        private bool fullyGrown = false;
        private bool isLoaded = false;
        
        bool firstHarvest = true;
        private SeasonManager seasonManager;
        private float currentTemp;
        private float currentMoisture;
        private float currentSunlight;

        void Start()
        {
            if (visual == null)
            {
                if (transform.childCount > 0)
                {
                    visual = transform.GetChild(0);
                    Debug.Log($"{gameObject.name}: visual was naturally null, but was automatically assigned to first child '{visual.name}'");
                }
                else
                {
                    visual = transform;
                    Debug.Log($"{gameObject.name}: visual was naturally null, but was automatically assigned to self");
                }
            }
            
            if (visual == null)
            {
                Debug.LogError($"{gameObject.name}: visual NOT assigned and no children found!");
                enabled = false;
                return;
            }

            if (dayController == null)
                dayController = FindFirstObjectByType<DayController>();
            
            if (dayController == null)
            {
                Debug.LogError("DayController not found");
                enabled = false;
                return;
            }


            // Subscribe to the dayPassedEvent
            dayController.dayPassedEvent.AddListener(OnDayPassed);

            dayDivisionSeconds = dayController.DayLengthSeconds / stepsPerDay;
            
            if (!isLoaded)
            {
                visual.localScale = minSize;
            }
            
            seasonManager = FindFirstObjectByType<SeasonManager>();
            if (!seasonManager)
            {
                Debug.LogError("SeasonManager not found");
                enabled = false;
            }
            
            currentTemp = seasonManager.GetCurrentTemperature();
            currentMoisture = seasonManager.GetCurrentMoisture();
            currentSunlight = seasonManager.GetCurrentSunlight();
        }

        void OnDestroy()
        {
            if(dayController!=null)
                dayController.dayPassedEvent.RemoveListener(OnDayPassed);
        }
        
        void OnDayPassed()
        {
            currentGrowth++;
            currentTemp = seasonManager.GetCurrentTemperature();
            currentMoisture = seasonManager.GetCurrentMoisture();
            currentSunlight = seasonManager.GetCurrentSunlight();
            SampleQuality(currentTemp, currentMoisture, currentSunlight);
        }

        void Update()
        {

            dayDivisionSeconds += Time.deltaTime;
            
            CheckForScaleChange();
        }

        private void CheckForScaleChange()
        {
            // Check if the current scale is different from the last recorded scale
            if (dayDivisionSeconds >= dayController.DayLengthSeconds / stepsPerDay)
            {

                // 1. Call your specific internal function
                MyFunctionWhenGrown();

                // 2. Invoke the event for external listeners (UI, Sound, Particles)
                OnSizeChanged?.Invoke(transform.localScale);
                dayDivisionSeconds = 0f;
            }
        }

        // This is the function you wanted to call
        // modified to cap growth when fully mature plant, written with help from chatGPT
        private void MyFunctionWhenGrown()
        {
            if (fullyGrown) return;

            Vector3 growthStep = (maxSize - minSize) / (growthDays * stepsPerDay);
            visual.localScale += growthStep;

            visual.localScale = new Vector3(
                Mathf.Min(visual.localScale.x, maxSize.x),
                Mathf.Min(visual.localScale.y, maxSize.y),
                Mathf.Min(visual.localScale.z, maxSize.z)
            );

            // clamp + set fully grown
            if (visual.localScale.x >= maxSize.x &&
                visual.localScale.y >= maxSize.y &&
                visual.localScale.z >= maxSize.z)
            {
                visual.localScale = maxSize;
                fullyGrown = true;
                Debug.Log("Plant fully grown");
            }
            
            // Debug the quality multiplier for testing
            Debug.Log($"Accumulated Quality: {accumulatedQuality}, Samples: {qualitySamples}, Multiplier: {GetQualityMultiplier()}");
        }

        public bool CanHarvest()
        {
            return fullyGrown;
        }

        public int Harvest()
        {
            if (!fullyGrown) return 0;
            fullyGrown = false;
            return Mathf.RoundToInt(cropValue * GetQualityMultiplier());
        }

        public void SetGrowthState(Vector3 savedScale, bool isFullyGrown)
        {
            if (visual == null)
            {
                if (transform.childCount > 0) visual = transform.GetChild(0);
                else visual = transform;
            }
            
            if (visual != null)
            {
                isLoaded = true;
                visual.localScale = savedScale;
                fullyGrown = isFullyGrown;
                if (fullyGrown)
                {
                    visual.localScale = maxSize;
                }
            }
        }

        public Vector3 GetScale()
        {
            return visual != null ? visual.localScale : maxSize;
        }

        public int HarvestValue()
        {
            return 5; // temp hard coded plant value for testing
        }
        
        private void SampleQuality(float temperature, float moisture, float sunlight)
        {
            float tempScore  = ScoreFactor(temperature, temperatureRange);
            float moistScore = ScoreFactor(moisture, moistureRange);
            float sunScore   = ScoreFactor(sunlight, sunlightRange);

            float stepQuality = tempScore * moistScore * sunScore;
            accumulatedQuality += stepQuality;
            qualitySamples++;
        }

        // Returns 1.0 at the midpoint of the range, falls off toward the edges
        private float ScoreFactor(float value, Vector2 range)
        {
            float mid = (range.x + range.y) * 0.5f;
            float halfSpan = (range.y - range.x) * 0.5f;

            if (halfSpan <= 0f) return 1f;

            float distance = Mathf.Abs(value - mid) / halfSpan; // 0 at center, 1 at edge
            float score = 1f - distance;                         // 1 at center, 0 at edge
            return Mathf.Clamp(score, 0f, 1f);
        }

        // Call this instead of returning raw cropValue
        public float GetQualityMultiplier()
        {
            if (qualitySamples == 0) return 1f;
            float avgQuality = accumulatedQuality / qualitySamples;
            // Remap so worst case is minQualityMultiplier, best is 1.0
            return Mathf.Lerp(minQualityMultiplier, 1f, avgQuality);
        }
    }
    
    
}