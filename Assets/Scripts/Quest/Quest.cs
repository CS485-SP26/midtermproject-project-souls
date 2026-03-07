using UnityEngine.Events;


namespace Quest
{
    public class QuestObject
    {
        public bool isCompleted;
        public string questName;
        public string description;
        public UnityAction<QuestObject> OnQuestCompleted;

        public bool IsCompleted
    {
        get => isCompleted;
        set
        {
            if (isCompleted != value)
            {
                isCompleted = value;

                if (isCompleted)
                    {
                    OnQuestCompleted?.Invoke(this);
                    }
            }
        }
    }
    }
}