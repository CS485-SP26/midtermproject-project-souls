using UnityEngine;
using Character;
using Core;
namespace Quest {
public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject questUI;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private PlayerController playerController;

    public Transform currentQuestParent;
    public Transform completedQuestParent;

    public GameObject questEntryPrefab;

    private bool playerInRange = false;
    private bool UIOpen = false;

    void Start()
    {
        questUI.SetActive(false);
    }
    
    public void Open()
    {   
        if(UIOpen) {
            this.Close();
            return;
        }; // Closes shop if E is hit again
        RefreshUI();
        questUI.SetActive(true);
        playerUI.SetActive(false);
        playerController.canMove = false; 
        UIOpen = true;
    }

    public void Close()
    {
        questUI.SetActive(false);
        playerUI.SetActive(true);
        playerController.canMove = true;
        UIOpen = false;
    }
    public bool getOpen()
    {
        return UIOpen;
    }
    public void RefreshUI()
    {
        ClearChildren(currentQuestParent);
        ClearChildren(completedQuestParent);

        foreach (var quest in GameManager.Instance.Quests.currentQuests)
        {
                Debug.Log("Quest found in UI: " + quest);

            GameObject entry = Instantiate(questEntryPrefab, currentQuestParent);
            entry.GetComponent<QuestEntry>().Setup(quest);
        }

        foreach (var quest in GameManager.Instance.Quests.completedQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, completedQuestParent);
            entry.GetComponent<QuestEntry>().Setup(quest);
        }
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
}