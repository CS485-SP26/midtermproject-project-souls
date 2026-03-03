using UnityEngine;

namespace Core
{
    public class SeedsManager : MonoBehaviour //this is basically just a copy/paste of funds manager but for seeds
    {
            public static SeedsManager Instance { get; private set; }
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
