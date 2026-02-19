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

        private FundsData funds = new FundsData(10);
        public FundsData Funds => funds;

        public void AddFunds(int amount) => funds.Add(amount);
        public int GetFunds() => funds.Get();

        public struct FundsData
        {
            private int amount;

            public FundsData(int startingAmount)
            {
                amount = startingAmount;
            }

            public int Amount => amount;
            public void Add(int value) => amount += value;
            public int Get() => amount;
            public void Set(int value) => amount = value;
        }


        void Awake()
        {
            if (GameManager.instance == null)
            {
                // keep this as close to the top of Awake as possible to avoid
                // multiple instancing
                instance = this; 
                DontDestroyOnLoad(this);
                Debug.Log("GameManager set through Awake");
            }
            else
            {
                Debug.Log("Duplicate GameManager attempted. Deleting new attempt.");
                Destroy(this);
            }
        }

        public void LoadScenebyName(string name)
        {
            Debug.Log("Loading scene: " + name);
            SceneManager.LoadScene(name);
        }
    }
}