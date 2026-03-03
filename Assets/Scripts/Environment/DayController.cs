using UnityEngine;
using TMPro; // Important for TextMeshPro
using UnityEngine.Events;
using System;
using Farming;

namespace Environment 
{
    public class DayController : MonoBehaviour //this has changes to match the professors lecture from 2/24/26
    {
        [Header("Object References")]
        [SerializeField] private Light sunLight;
        
        [Header("Time Constraints")]
        [SerializeField] private float dayLengthSeconds = 60f;
        [SerializeField] private float dayProgressSeconds = 0f; // good for debugging from the editor
        [SerializeField] private int currentDay = 1; // Good for debugging from the editor

        // Properties
        public float DayProgressPercent => Mathf.Clamp01(dayProgressSeconds / dayLengthSeconds);
        public int CurrentDay { get { return currentDay; } } 
        public float DayLengthSeconds => dayLengthSeconds;
        
        public UnityEvent dayPassedEvent = new UnityEvent(); // Invoke() at end of day

        public event Action dayPassedSystem;

        public void AdvanceDay()
        {
            Debug.Assert(sunLight, "DayController requires a 'Sun'");

            dayProgressSeconds = 0f; // Reset to start a new day
            currentDay++;
            
            dayPassedSystem?.Invoke(); //? modifier means only do if not null.
            dayPassedEvent?.Invoke(); //make announcement to all listeners
        }

        public void UpdateVisuals()
        {
            // Calculate sun's rotation based on time of day
            // 0 degrees for sunrise, 180 for sunset, 360 for next sunrise
            float sunRotationX = Mathf.Lerp(0f, 360f, DayProgressPercent);

            // Apply rotation to the sun light
            sunLight.transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);

            // Optional: Adjust other elements, like skybox, light source intensity, and so on
            // sunLight.intensity = 
            // RenderSettings.fogColor = 
            // RenderSettings.skybox.SetFloat
        }

        void Update()
        {
            dayProgressSeconds += Time.deltaTime;

            if (dayProgressSeconds >= dayLengthSeconds)
            {
                AdvanceDay();
            }

            UpdateVisuals();
        }
        
    }
}