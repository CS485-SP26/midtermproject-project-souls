using UnityEngine;
using System.Collections;

namespace Quest
{
    public class QuestComplete : MonoBehaviour
    {
        public static QuestComplete Instance { get; private set; }

        [SerializeField] private GameObject questCompleteText;

        void Awake()
        {
            Instance = this;
        }

        public void ShowQuestComplete()
        {
            StartCoroutine(ShowAndHide());
        }

        private IEnumerator ShowAndHide()
        {
            questCompleteText.SetActive(true);
            yield return new WaitForSeconds(5f);
            questCompleteText.SetActive(false);
        }
    }
}