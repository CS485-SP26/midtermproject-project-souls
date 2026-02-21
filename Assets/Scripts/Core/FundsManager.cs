using UnityEngine;

namespace Core
{
    public class FundsManager : MonoBehaviour
    {
        private int amount;

        public int Amount => amount;

        public void Initialize(int startingAmount)
        {
            amount = startingAmount;
        }

        public void Add(int value) => amount += value;
        public int Get() => amount;
        public void Set(int value) => amount = value;
    }
}