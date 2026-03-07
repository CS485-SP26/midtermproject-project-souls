using UnityEngine;
using System.Collections.Generic;

namespace Quest {
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<QuestObject> currentQuests = new List<QuestObject>();
    public List<QuestObject> completedQuests = new List<QuestObject>();

    void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
            Debug.Log("QuestManager Initialize Running");

        QuestObject waterFill  = new QuestObject
        {
            isCompleted = false,
            questName = "Get Watering",
            description = "Water all tiles"
        };

        QuestObject firstHarvest = new QuestObject
        {
            isCompleted = false,
            questName = "Harvested!",
            description = "Harvest your first plant"
        };

        AddQuest(waterFill);
        AddQuest(firstHarvest);
        Debug.Log("Current quest count: " + currentQuests.Count);
    }

    public void CompleteQuest(QuestObject quest)
    {
        currentQuests.Remove(quest);
        quest.isCompleted = true;
        completedQuests.Add(quest);
    }

    public void AddQuest(QuestObject quest)
    {
            Debug.Log("Adding quest: " + quest.questName);

        currentQuests.Add(quest);
    }
}
}