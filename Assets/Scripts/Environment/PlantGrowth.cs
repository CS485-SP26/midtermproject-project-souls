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

        private bool fullyGrown = false;

        void Start()
        {
            if (visual == null)
            {
                Debug.LogError($"{gameObject.name}: visual NOT assigned!");
                enabled = false;
                return;
            }
            else
            {
                Debug.Log($"{gameObject.name}: visual assigned as {visual.name}");
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
            visual.localScale = minSize;
            
        }

        void OnDestroy()
        {
            if(dayController!=null)
                dayController.dayPassedEvent.RemoveListener(OnDayPassed);
        }
        void OnDayPassed()
        {
            currentGrowth++;
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

            if (visual.localScale.x >= maxSize.x &&
                visual.localScale.y >= maxSize.y &&
                visual.localScale.z >= maxSize.z)
            {
                visual.localScale = maxSize;
                fullyGrown = true;
                Debug.Log("Plant fully grown");
            }

        }

        public bool CanHarvest()
        {
            return fullyGrown;
        }

        public int Harvest()
        {
            if (!fullyGrown) return 0;

            fullyGrown = false;
            return cropValue;
        }

        public int HarvestValue()
        {
            return 5; // temp hard coded plant value for testing
        }
    }
}