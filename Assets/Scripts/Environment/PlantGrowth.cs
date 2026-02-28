using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

namespace Environment
{

    public class PlantGrowth : MonoBehaviour
    {
        [Header("Events")]
        // Drag and drop functions here in the Inspector
        public UnityEvent<Vector3> OnSizeChanged;

        [Header("Object References")] [SerializeField]
        DayController dayController;

        private float dayDivisionSeconds;
        
        [SerializeField] private int growthDays = 3;
        private int currentGrowth = 0;
        

        [SerializeField] private int cropValue = 10;
        public int GetCropValue() { return cropValue; }
        
        // Mesh size
        [Header("Size Constraints")] [SerializeField]
        private Vector3 minSize = new Vector3(0.1f, 0.1f, 0.1f);

        [SerializeField] private Vector3 maxSize = new Vector3(1f, 1f, 1f);
        [SerializeField] private int stepsPerDay = 10;

        void Start()
        {
            
            // Subscribe to the dayPassedEvent
            dayController.dayPassedEvent.AddListener(OnDayPassed);

            dayDivisionSeconds = dayController.DayLengthSeconds / stepsPerDay;
            transform.localScale = minSize;
            
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
        private void MyFunctionWhenGrown()
        {
            transform.localScale += (maxSize - minSize) / (growthDays * stepsPerDay);
        }
    }
}