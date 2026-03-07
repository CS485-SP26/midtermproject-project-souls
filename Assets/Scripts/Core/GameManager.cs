using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using Quest;
namespace Core
{
    public class GameManager:MonoBehaviour //game manager has changes to it, specifically with compatibility to seedsManager which follows the same logic as fundsManager
    {
        public static GameManager Instance { get; private set; }
        

        private FundsManager fundsManager;
        private SeedsManager seedsManager; //added for seeds purchasing
        private SeasonManager seasonManager;
        private TileDataManager tileDataManager;
        private QuestManager questManager;

        public QuestManager Quests => questManager;
        public FundsManager Funds => fundsManager;
        public SeedsManager Seeds => seedsManager; //added for seeds purchasing

        public SeasonManager Seasons => seasonManager;
        public TileDataManager TileData => tileDataManager;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // keep this as close to the top of Awake as possible to avoid
            // multiple instancing
            Instance = this;
            DontDestroyOnLoad(this);
            
            if (fundsManager == null) 
                fundsManager = GetComponent<FundsManager>();

            if (seedsManager == null) //added for seeds purchasing
                seedsManager = GetComponent<SeedsManager>(); //added for seeds purchasing
                
            if (seasonManager == null)
                seasonManager = GetComponent<SeasonManager>();
                
            if (tileDataManager == null)
                tileDataManager = GetComponent<TileDataManager>();

            if (questManager == null)
                questManager = GetComponent<QuestManager>();

            
            if (questManager != null) questManager.Initialize();
            if (fundsManager != null) fundsManager.Initialize(0);
            if (seedsManager != null) seedsManager.Initialize(5); //added for seeds purchasing
            if (seasonManager != null) seasonManager.Initialize();
            Debug.Log("GameManager set through Awake");

        }

        public void AddFunds(int amount) => fundsManager?.Add(amount);
        public int GetFunds() => fundsManager != null ? fundsManager.Get() : 0;


        public void AddSeeds(int amount) => seedsManager?.Add(amount); //same idea as addfunds, added for seeds manager
        public int GetSeeds() => seedsManager != null ? seedsManager.Get() : 0;

        private float currentWaterLevel = 1.0f;
        public float GetWaterLevel() => currentWaterLevel;
        public void SetWaterLevel(float level) => currentWaterLevel = level;

        private int plants;
        public bool firstHarvest = true;

        public void AddPlants(int add)
        {   
            QuestObject quest = GameManager.Instance.Quests.currentQuests.Find(q => q.questName == "Harvested!");
            if (firstHarvest && quest != null){
                quest.isCompleted = true;
                GameManager.Instance.Funds.Add(50);
                firstHarvest = false;
            }
            plants = plants + add;
        }
        public int GetPlants() => plants;
        public void LoadScenebyName(string name)
        {
            Debug.Log("Loading scene: " + name);
            SceneManager.LoadScene(name);
        }
    }
}