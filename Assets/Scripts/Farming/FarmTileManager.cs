using System.Collections.Generic;
using Art.UI.ProgressBar;
using UnityEngine;
using UnityEditor;
using Environment;

namespace Farming
{
    public class FarmTileManager:MonoBehaviour //this has changes to match the professors lecture from 2/24/26
    {
        [SerializeField] private GameObject farmTilePrefab;
        [SerializeField] DayController dayController;
        [SerializeField] private int rows = 4;
        [SerializeField] private int cols = 4;
        [SerializeField] private float tileGap = 0.1f;
        [SerializeField] private GameObject plantPrefab; // Added to spawn saved plants
        private List<FarmTile> tiles = new List<FarmTile>();
        
        public GameObject PlantPrefab => plantPrefab;
        public SimpleProgressBar progressBar;
        private int tileCount = 0;
        
        void Start()
        {
            Debug.Assert(farmTilePrefab, "FarmTileManager requires a farmTilePrefab");
            Debug.Assert(dayController, "FarmTileManager requires a dayController");
            Debug.Assert(progressBar, "FarmTileManager requires a progressBar"); 

            // Clear and repopulate the list based entirely on children to guarantee correct order
            tiles.Clear();
            tiles.AddRange(GetComponentsInChildren<FarmTile>());

            Debug.Log($"[FarmTileManager] Start() - Found and registered {tiles.Count} tiles in hierarchy. Assigning explicit IDs...");

            // Assign deterministic explicit IDs rather than relying on GameObject sibling index
            for(int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].id = i;
                    tiles[i].LoadData();
                }
            }

            UpdateProgressBar();
        }

        void OnEnable()
        {
            dayController.dayPassedSystem += OnDayPassed; //we can add as many events as we want, these are fast compared to send message
            dayController.dayPassedEvent.AddListener(this.OnDayPassed);
        }

        void OnDisable()
        {
            dayController.dayPassedSystem -= OnDayPassed;
            dayController.dayPassedEvent.RemoveListener(this.OnDayPassed);            
        }

        public void OnDayPassed()
        {
            IncrementDays(1);
        }

        public void IncrementDays(int count)
        {
            while (count > 0)
            {
                foreach (FarmTile farmTile in tiles)
                {
                    farmTile.OnDayPassed();
                }
                count--;
            }
        }
        public int ConfirmCount()
        {
            
            int countTiles = 0;
            foreach (FarmTile farmTile in tiles)
            {
                if(farmTile.GetCondition == FarmTile.Condition.Grass)
                {
                    countTiles++;
                }
            }
            //Debug.Log("tiles counted: " + countTiles);
            return countTiles;
        }

        public int ConfirmTiles()
        {
            int wetTiles = 0;

            foreach (FarmTile farmTile in tiles)
            {
                if(farmTile.GetCondition == FarmTile.Condition.Watered)
                {
                    wetTiles++;
                }
            }

            return wetTiles;
        }

        public void UpdateProgressBar()
        {
            
            if(!progressBar) return;
            
            int readyCount = 0;
            foreach (FarmTile farmTile in tiles)
            {
                if(farmTile.GetCondition == FarmTile.Condition.Watered)
                {
                    readyCount++;
                }
            }
            
            progressBar.SetProgress(readyCount, 0, tiles.Count);
            
        }

        void InstantiateTiles()
        {
            Vector3 spawnPos = transform.position;
            int count = 0;
            GameObject clone = null; 

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    clone = Instantiate(farmTilePrefab, spawnPos, Quaternion.identity);
                    clone.name = "Farm Tile " + count++.ToString();
                    spawnPos.x += clone.transform.localScale.x + tileGap;
                    clone.transform.parent = transform; // build heirarchy
                    var tile = clone.GetComponent<FarmTile>();
                    //Debug.Assert(tile, $"farmTilePrefab is missing a FarmTile component on {clone.name}");
                    if (tile) tiles.Add(tile);
                }

                if (clone != null)
                {
                    spawnPos.z += clone.transform.localScale.z + tileGap;
                    spawnPos.x = transform.position.x;
                }
            }
            
        }

        // ***************************************************************** //
        // Below this line is code to suppor the Unity Editor (Advanced)
        // Please feel free to disregard everything below this
        // ***************************************************************** //
        void OnValidate()
        {
            #if UNITY_EDITOR
            //EditorApplication.delayCall += () => { //we do register, but we never UNREGISTERED, fix by...
            EditorApplication.delayCall -= DelayValidateGrid; //changed for lecture
            EditorApplication.delayCall += DelayValidateGrid; //changed for lecture
            //};
            #endif
        }

        void DelayValidateGrid() //added by lecture
        {
                if (this == null) return; // Guard against the object being deleted
                ValidateGrid();
        }

        void ValidateGrid() 
        {
            if (!farmTilePrefab) return;
            
            // In play mode, we shouldn't be destroying and recreating the grid dynamically 
            // since that would wipe out our loaded tiles.
            if (Application.isPlaying) 
            {
                Debug.Log("[FarmTileManager] ValidateGrid() ignored because Application is playing.");
                return;
            }

            tiles.Clear();
            tiles.AddRange(GetComponentsInChildren<FarmTile>());

            int newCount = rows * cols;

            if (tiles.Count != newCount)
            {
                DestroyTiles();
                InstantiateTiles();
            }
        }

        void DestroyTiles()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                #if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
                #else
                Destroy(transform.GetChild(i).gameObject);
                #endif
            }
            tiles.Clear();
        }
    }
}