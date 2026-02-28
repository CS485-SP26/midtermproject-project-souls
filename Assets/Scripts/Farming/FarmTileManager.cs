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
        public List<FarmTile> tiles = new List<FarmTile>(); //changed from private to public for tileevent.cs
        
        public SimpleProgressBar progressBar;
        private int tileCount = 0;
        
        void Start()
        {
            Debug.Assert(farmTilePrefab, "FarmTileManager requires a farmTilePrefab");
            Debug.Assert(dayController, "FarmTileManager requires a dayController");
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

        public void UpdateProgressBar()
        {
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
                    tiles.Add(clone.GetComponent<FarmTile>()); // for resize/delete
                }
                spawnPos.z += clone.transform.localScale.z + tileGap;
                spawnPos.x = transform.position.x;
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
            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<FarmTile>(out var tile))
                {
                    tiles.Add(tile);
                }
            }

            int newCount = rows * cols;

            if (tiles.Count != newCount)
            {
                DestroyTiles();
                InstantiateTiles();
            }
        }

        void DestroyTiles()
        {
            foreach (FarmTile tile in tiles)
            {
                #if UNITY_EDITOR
                DestroyImmediate(tile.gameObject);
                #else
                Destroy(tile.gameObject);
                #endif
            }
            tiles.Clear();
        }
    }
}