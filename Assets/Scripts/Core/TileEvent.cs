using System.Collections.Generic;
using UnityEngine;
using Farming;
using Environment;
using UnityEditor;

public class TileEvent : MonoBehaviour //this code could definitely be improved, event for when all tiles are wet, refreshes check at the end of the day
{

    public UnityEngine.Events.UnityEvent AllTilesWet;
    private FarmTileManager manager;
    private DayController day;
    
    int wetted = 0; //wetted is how many wet tiles there are
    int goal = -1; //goal is the amount of tiles total, set to -1 so this doesnt match wetted by default

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<FarmTile> readTiles = manager.tiles; //i didnt know how to copy this without changing list to public, will ask for help on improving this later
        goal = readTiles.Count; //update goal to reflect how many tiles there are

        foreach (FarmTile farmTile in readTiles) //recycled code from farmtilemanager.cs (i think this is from the same person who made the old progress bar)
        {
            if(farmTile.GetCondition == FarmTile.Condition.Watered)
                wetted++;
        }

        day.dayPassedEvent.AddListener(OnCheckWater); //activates our newly created function when the daypassedevent is invoked

    }

    public void OnCheckWater() //newly created function
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if(day.dayPassedEvent != null && wetted == goal) //if the dayPassedEvent happened and we have as many wet tiles as the goal
        {
            AllTilesWet.Invoke(); //invoke/activate this event
        }
    }
}
