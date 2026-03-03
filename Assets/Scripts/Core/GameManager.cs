using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
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

        public FundsManager Funds => fundsManager;
        public TileDataManager TileData => tileDataManager;

        void Awake()
        {
            if (GameManager.instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);

                if (fundsManager == null) fundsManager = GetComponent<FundsManager>();
                if (tileDataManager == null) tileDataManager = GetComponent<TileDataManager>();

                fundsManager.Initialize(10);
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

        public void LoadScenebyName(string name)
        {
            Debug.Log("Loading scene: " + name);
            SceneManager.LoadScene(name);
        }
    }
}