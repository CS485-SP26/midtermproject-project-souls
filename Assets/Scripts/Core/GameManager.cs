using UnityEngine;
using UnityEngine.SceneManagement;
using Core;

namespace Core
{
    public class GameManager:MonoBehaviour
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

        public FundsManager Funds => fundsManager;


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