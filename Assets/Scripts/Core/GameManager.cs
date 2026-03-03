using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Core
{
    public class GameManager:MonoBehaviour //game manager has changes to it, specifically with compatibility to seedsManager which follows the same logic as fundsManager
    {
        static private GameManager instance = null;

        static public GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                    Debug.Log("Create a new GameManager.");
                }
                return instance;
            }
        }

        private FundsManager fundsManager;
        private SeedsManager seedsManager; //added for seeds purchasing
        private SeasonManager seasonManager;

        public FundsManager Funds => fundsManager;
        public SeedsManager Seeds => seedsManager; //added for seeds purchasing

        public  SeasonManager Seasons => seasonManager;
        

        void Awake()
        {
            if (GameManager.instance == null)
            {
                // keep this as close to the top of Awake as possible to avoid
                // multiple instancing
                instance = this; 
                DontDestroyOnLoad(this);
                
                if (fundsManager == null)
                    fundsManager = GetComponent<FundsManager>();

                if (seedsManager == null) //added for seeds purchasing
                    seedsManager = GetComponent<SeedsManager>(); //added for seeds purchasing
                
                if (seasonManager == null)
                    seasonManager = GetComponent<SeasonManager>();

                fundsManager.Initialize(0);
                seedsManager.Initialize(5); //added for seeds purchasing
                seasonManager.Initialize();
                Debug.Log("GameManager set through Awake");
            }
            else
            {
                Debug.Log("Duplicate GameManager attempted. Deleting new attempt.");
                Destroy(this);
            }
        }

        public void AddFunds(int amount) => fundsManager.Add(amount);
        public int GetFunds() => fundsManager.Get();


        public void AddSeeds(int amount) => seedsManager.Add(amount); //same idea as addfunds, added for seeds manager
        public int GetSeeds() => seedsManager.Get(); //same idea as getfunds, added for seeds manager

        private int plants;
        public void AddPlants(int add)
        {
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