using System.Collections.Generic;
using UnityEngine;
using Farming;

namespace Core
{
    public class TileDataManager : MonoBehaviour
    {
        public struct TileSaveData
        {
            public FarmTile.Condition condition;
            public int daysSinceInteraction;
        }

        private Dictionary<int, TileSaveData> tileStates = new Dictionary<int, TileSaveData>();

        public void SaveTile(int id, FarmTile.Condition condition, int days)
        {
            tileStates[id] = new TileSaveData
            {
                condition = condition,
                daysSinceInteraction = days
            };
        }

        public bool TryGetTile(int id, out TileSaveData data)
        {
            return tileStates.TryGetValue(id, out data);
        }
    }
}