using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField] private FundsManager fundsManager;
        [SerializeField] private TileDataManager tileDataManager;
        [SerializeField] private SeedsManager seedsManager; //added for seeds purchasing


        public FundsManager Funds => fundsManager;
        public TileDataManager TileData => tileDataManager;
        public SeedsManager Seeds => seedsManager; //added for seeds purchasing


        void Awake()
        {
            if (GameManager.instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);

                if (fundsManager == null) fundsManager = GetComponent<FundsManager>();
                if (tileDataManager == null) tileDataManager = GetComponent<TileDataManager>();

                if (seedsManager == null) //added for seeds purchasing
                    seedsManager = GetComponent<SeedsManager>(); //added for seeds purchasing

                fundsManager.Initialize(0);
                seedsManager.Initialize(0); //added for seeds purchasing
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