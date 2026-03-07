using UnityEngine;
using TMPro;

namespace Quest
{
    public class QuestEntry : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public void Setup(QuestObject quest)
        {
            if (quest == null)
    {
        Debug.Log("Quest passed to QuestEntry is NULL");
        return;
    }
        titleText.text = quest.questName;
        descriptionText.text = quest.description;
        }
    }
}