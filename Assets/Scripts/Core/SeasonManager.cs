using Environment;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    
    private int numberOfSeasons = System.Enum.GetNames(typeof(Season)).Length;

    enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
    
    private int daysPerSeason = System.Enum.GetNames(typeof(DayOfWeek)).Length;

    struct Date
    {
        public int day;
        public DayOfWeek dayOfWeek;
        public Season season;
    }
    
    private Date currentDate;
    
    private float percentYearPassed => ((int)currentDate.season * daysPerSeason + currentDate.day) / 
                                       (float)(numberOfSeasons * daysPerSeason);
    
    [SerializeField] private DayController dayController;
    [SerializeField] private TMP_Text dateLabel;    
    
    [Header("Temperature")]
    [SDCurve(-20f, 40f, "Season", "Temperature (°C)")]
    [SerializeField] private EnvironmentCurve temperatureCurve;

    public float GetCurrentTemperature()
    {
        if (temperatureCurve == null) return 0f;
        return temperatureCurve.SampleMean(percentYearPassed);
    }

    public float GetCurrentTemperatureSD()
    {
        if (temperatureCurve == null) return 0f;
        return temperatureCurve.SampleSD(percentYearPassed);
    }
    
    void OnEnable()
    {
        if (dayController != null)
        {
            dayController.dayPassedSystem += AdvanceDate;
        }
    }

    void OnDisable()
    {
        if (dayController != null)
        {
            dayController.dayPassedSystem -= AdvanceDate;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dateLabel)
        {
            dateLabel.SetText(DateToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Initialize()
    {
        currentDate.day = 1;
        currentDate.dayOfWeek = DayOfWeek.Monday;
        currentDate.season = Season.Spring;
    }

    public void AdvanceDate()
    {
        currentDate.day++;
        currentDate.dayOfWeek = (DayOfWeek)(((int)currentDate.dayOfWeek + 1) % daysPerSeason);
        if (currentDate.day > daysPerSeason)
        {
            currentDate.day = 1;
            currentDate.season = (Season)(((int)currentDate.season + 1) % numberOfSeasons);
        }
       
        if (dateLabel)        
        {
            dateLabel.SetText(DateToString());
        }
    }
    
    private string DateToString()
    {
        return $"{currentDate.dayOfWeek}, {currentDate.season} Day {currentDate.day}";
    }
}
