using UnityEngine;

namespace Core
{
    public class FundsManager : MonoBehaviour //edited to be claytons suggestion on discord 5:13pm 2-26-26
    {
        public static FundsManager Instance { get; private set; }
        private int amount;
        public int Amount => amount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(int startingAmount)
        {
            amount = startingAmount;
        }

        public void Add(int value) => amount += value;
        public int Get() => amount;
        public void Set(int value) => amount = value;
    }
}