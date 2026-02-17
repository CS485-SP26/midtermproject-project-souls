using System.Collections.Generic;
using UnityEngine;
using Farming;

namespace Character 
{
    public class TileSelector : MonoBehaviour
    {
        [SerializeField] protected FarmTile activeTile; // good for debugging
        public FarmTile GetSelectedTile() { return activeTile; }

        protected void SetActiveTile(FarmTile tile)
        {
            if (activeTile != tile)
            {
                activeTile?.SetHighlight(false);
                activeTile = tile;
                activeTile?.SetHighlight(true);
            }
        }
        
        // Inside TileSelector.cs
        public virtual void SetSelectedTiles(List<FarmTile> tiles) 
        {
            // Default implementation: maybe just pick the first one?
            if(tiles.Count > 0) SetActiveTile(tiles[0]);
        }
    }
}
