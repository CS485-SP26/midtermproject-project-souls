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
            public bool hasPlant;
            public Vector3 plantScale;
            public bool plantFullyGrown;
        }

        private static Dictionary<int, TileSaveData> tileStates = new Dictionary<int, TileSaveData>();

        public void SaveTile(int id, FarmTile.Condition condition, int days, bool hasPlant, Vector3 plantScale, bool fullyGrown)
        {
            tileStates[id] = new TileSaveData
            {
                condition = condition,
                daysSinceInteraction = days,
                hasPlant = hasPlant,
                plantScale = plantScale,
                plantFullyGrown = fullyGrown
            };
        }

        public bool TryGetTile(int id, out TileSaveData data)
        {
            if (tileStates.TryGetValue(id, out data))
            {
                return true;
            }
            
            Debug.LogWarning($"[TileDataManager] TryGetTile failed for ID {id}. Currently saved IDs: {string.Join(", ", tileStates.Keys)}");
            return false;
        }
    }
}