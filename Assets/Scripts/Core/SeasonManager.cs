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
    private TMP_Text dateLabel;
    
    public float savedDayProgressSeconds = 0f;
    public int savedCurrentDay = 1;
    
    public void SetDayController(DayController dc)
    {
        if (dayController != null)
        {
            dayController.dayPassedSystem -= AdvanceDate;
        }
        dayController = dc;
        if (dayController != null)
        {
            dayController.dayPassedSystem += AdvanceDate;
            dayController.RestoreTime(savedDayProgressSeconds, savedCurrentDay);
        }
        
        // Dynamically find the date label in the scene to avoid Inspector setup
        var labelObj = GameObject.Find("DayLabel");
        if (labelObj != null)
        {
            dateLabel = labelObj.GetComponent<TMP_Text>();
            dateLabel.SetText(DateToString());
        }
    }

    public void SaveTime(float progress, int day)
    {
        savedDayProgressSeconds = progress;
        savedCurrentDay = day;
    }
    
    [Header("Environment Curves")]
    [Header("Temperature")]
    [SDCurve(-20f, 40f, "Season", "Temperature (°C)")]
    [SerializeField] private EnvironmentCurve temperatureCurve;
    [Header("Moisture")]
    [SDCurve(0f, 100f, "Season", "Soil Moisture")]
    [SerializeField] private EnvironmentCurve moistureCurve;
    [Header("Sunlight")]
    [SDCurve(0f, 100f, "Season", "Percent Sunlight")]
    [SerializeField] private EnvironmentCurve sunlightCurve;
    
    public float GetCurrentTemperature() => temperatureCurve.SampleValue(percentYearPassed);
    public float GetCurrentMoisture() => moistureCurve.SampleValue(percentYearPassed);
    public float GetCurrentSunlight() => sunlightCurve.SampleValue(percentYearPassed);
    
    float currentTemperature;
    float currentMoisture;
    float currentSunlight;
    
    void OnEnable()
    {
        // Handled by SetDayController now
    }

    void OnDisable()
    {
        // Handled by SetDayController now
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Handled by SetDayController now
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
        
        currentTemperature = GetCurrentTemperature();
        currentMoisture = GetCurrentMoisture();
        currentSunlight = GetCurrentSunlight();
    }
    
    private string DateToString()
    {
        return $"{currentDate.dayOfWeek}, {currentDate.season} Day {currentDate.day}";
    }
}
