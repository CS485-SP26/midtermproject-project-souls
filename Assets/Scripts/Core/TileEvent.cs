using UnityEngine;
using Farming;
using Environment;
using System.Collections;
using UnityEngine.Events;
using Core;
using Character;
using Quest;
public class TileEvent : MonoBehaviour //this code could definitely be improved, event for when all tiles are wet, refreshes check at the end of the day
{
    public UnityEvent AllTilesWet = new UnityEvent();
    [SerializeField] FarmTileManager manager; //these need to be serialize fields or else you will get reference object is not set to object error (trying to use null object)
    [SerializeField] PlayerFarming interactCheck; //needed for the same reason as manager
    [SerializeField] private int tileIndex;
    bool questCompleted; //we want to disable the quest if its already been completed
    int wetted = 0; //wetted is how many wet tiles there are
    int goal = -1; //goal is the amount of tiles total, set to -1 so this doesnt match wetted by default
        private IEnumerator Start() // IEnumerator Start is called once before the first execution of Update after the MonoBehaviour is created and also lets you set a seconds delay
    {
        //wait until manager exists
        while (manager == null || manager.ConfirmCount() == 0)
            yield return null;
        Debug.Assert(manager, "the farm tile manager is missing for tile event!");
        goal = manager.ConfirmCount();
        Debug.Log("TileEvent ready. Goal = " + goal);

        interactCheck.interacting.AddListener(OnCheckWater);
    }

    public void OnCheckWater() //newly created function
    {
        // ?fixed with chatGPTs help for debugging
        //Debug.Log("interactCheck has been tripped!");

        if (manager == null || manager.ConfirmCount() == 0) return;
        //wetted = 0; // needs to be reset each time otherwise goes up every 10secs

        //Debug.Log("interactCheck has been tripped!");

        wetted = manager.ConfirmTiles();

        //Debug.Log("wetted tiles: " + wetted);

        if(wetted == goal && questCompleted == false) //if we have as many wet tiles as the goal
        {
            AllTilesWet?.Invoke(); //invoke/activate this event
            CancelInvoke(); //cancel all invokes in this script once alltileswet invoke happens                
            wetted = 0;
            questCompleted = true; //make sure the quest complete notification doesnt show up again after triggering this
        }
    }

    public void QuestRewardAddFunds() //note: this is called through a button and wont happen naturally
    {
        QuestObject quest = GameManager.Instance.Quests.currentQuests.Find(q => q.questName == "Get Watering");
        FundsManager.Instance.Add(100);
        GameManager.Instance.Quests.CompleteQuest(quest);
    }
}
