using Environment;
using TMPro;
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

    struct Date
    {
        public int day;
        public DayOfWeek dayOfWeek;
        public Season season;
    }
    
    private Date currentDate;
    
    [SerializeField] private DayController dayController;
    [SerializeField] private TMP_Text dateLabel;    
    
    void OnEnable()
    {
        dayController.dayPassedSystem += AdvanceDate;
    }

    void OnDisable()
    {
        dayController.dayPassedSystem -= AdvanceDate;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LogDate();
        if (dateLabel)
        {
            dateLabel.SetText($"{currentDate.dayOfWeek}, {currentDate.season}\nDay: {currentDate.day}");
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
        currentDate.dayOfWeek = (DayOfWeek)(((int)currentDate.dayOfWeek + 1) % 7);

        if (currentDate.day > 6)
        {
            currentDate.day = 1;
            currentDate.season = (Season)(((int)currentDate.season + 1) % 4);
        }

        LogDate();
        if (dateLabel)
        {
            dateLabel.SetText($"{currentDate.dayOfWeek}, {currentDate.season}\nDay: {currentDate.day}");
        }
    }

    
    void LogDate()
    {
        Debug.Log($"Day: {currentDate.day}, Day of Week: {currentDate.dayOfWeek}, Season: {currentDate.season}");
    }
}
