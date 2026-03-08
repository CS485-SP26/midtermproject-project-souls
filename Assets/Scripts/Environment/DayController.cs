using UnityEngine;
using TMPro; // Important for TextMeshPro
using UnityEngine.Events;
using System;
using Farming;
using UnityEditor.Toolbars;
using UnityEditor.Experimental.GraphView;

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

        struct tellTime { public int hour; public int minute; public int second; public string ampm;} //struct to display the time
        private tellTime ToD; //ToD = Time of Day

        // Properties
        public float DayProgressPercent => Mathf.Clamp01(dayProgressSeconds / dayLengthSeconds);
        public int CurrentDay { get { return currentDay; } } 
        public float DayLengthSeconds => dayLengthSeconds;
        
        public UnityEvent dayPassedEvent = new UnityEvent(); // Invoke() at end of day

        public event Action dayPassedSystem;
        

        void Start()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.Seasons != null)
            {
                Core.GameManager.Instance.Seasons.SetDayController(this);
            }
        }

        void OnDestroy()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.Seasons != null)
            {
                Core.GameManager.Instance.Seasons.SaveTime(dayProgressSeconds, currentDay);
            }
        }

        public void RestoreTime(float savedSeconds, int savedDay)
        {
            dayProgressSeconds = savedSeconds;
            currentDay = savedDay;
            UpdateVisuals();
        }

        public string TimeToString()
        {
            //am:pm logic, theres probably a better way of doing it
            if(ToD.hour == 0)
                { ToD.ampm = "AM"; ToD.hour = 12; } //make this 12:00AM
            else if(ToD.hour < 12)
                ToD.ampm = "AM"; //make this 1:00-11:00AM, the hour after is noon (12:00PM)
            else if(ToD.hour == 12)
                ToD.ampm = "PM"; //make this noon (12:00PM)
            else if(ToD.hour > 12) 
                { ToD.ampm = "PM"; ToD.hour -= 12; } //make this 1:00-11:00PM, reset to 12:00AM afterwards


            string finalHour = ToD.hour.ToString().PadLeft(2, '0');
            string finalMinute = ToD.minute.ToString().PadLeft(2, '0');
            string finalSecond = ToD.second.ToString().PadLeft(2, '0');
            return $"{finalHour}:{finalMinute}:{finalSecond} {ToD.ampm}";
        }

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

            ToD.hour = (int)GetHour();
            ToD.minute = 0; //todo
            ToD.second = 0; //todo

            if (dayProgressSeconds >= dayLengthSeconds)
            {
                AdvanceDay();
            }

            UpdateVisuals();
        }

        private float GetHour()
        {
            //linear mapping using Isak's formula
            float sunRot = Mathf.Lerp(0f, 360f, DayProgressPercent);
            float minA = 0; float minB = 0; //A is parameters for the angle of the sun
            float maxA = 360; float maxB = 24; //B is parameters for hours in a day
            float value = ((sunRot - minA) * (maxB - minB) / (maxA - minA) + (minB));

            return value;
        }

        
    }
}