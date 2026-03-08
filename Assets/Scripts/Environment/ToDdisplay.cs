using UnityEngine;
using TMPro;
using Environment;
using System.Collections;

public class ToDdisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeOfDay;
    [SerializeField] private DayController dayController;

    // Update is called once per frame
    void Update()
    {
        displayToD();
    }

    public void displayToD()
    {
        if(dayController)
        timeOfDay.SetText(dayController.TimeToString());
    }
}
