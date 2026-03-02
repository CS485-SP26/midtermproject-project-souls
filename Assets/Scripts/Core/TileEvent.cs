using UnityEngine;
using Farming;
using Environment;
using System.Collections;
using UnityEngine.Events;
using Core;
using Character;

public class TileEvent : MonoBehaviour //this code could definitely be improved, event for when all tiles are wet, refreshes check at the end of the day
{
    public UnityEvent AllTilesWet = new UnityEvent();
    [SerializeField] FarmTileManager manager; //these need to be serialize fields or else you will get reference object is not set to object error (trying to use null object)
    [SerializeField] PlayerFarming interactCheck; //needed for the same reason as manager
    
    int wetted = 0; //wetted is how many wet tiles there are
    int goal = -1; //goal is the amount of tiles total, set to -1 so this doesnt match wetted by default

    private void Start()
    {
        Debug.Assert(manager, "the farm tile manager is missing for tile event!");
        goal = manager.ConfirmCount();
        Debug.Log("goal is " + goal);

        interactCheck.interacting.AddListener(OnCheckWater);
        //InvokeRepeating(nameof(OnCheckWater), 10f, 10f); //repeat a check every 10 seconds, this NEEDS to be replaced to a trigger on tile interaction later (havent figured it out yet) 
    }

    public void OnCheckWater() //newly created function
    {
        Debug.Log("interactCheck has been tripped!");

        wetted = manager.ConfirmTiles();

        Debug.Log("wetted tiles: " + wetted);

        if(wetted == goal) //if we have as many wet tiles as the goal
        {
            AllTilesWet?.Invoke(); //invoke/activate this event
            CancelInvoke(); //cancel all invokes in this script once alltileswet invoke happens                
            wetted = 0;
        }
    }

    public void QuestRewardAddFunds() //note: this is called through a button and wont happen naturally
    {
        FundsManager.Instance.Add(100);
    }

}
