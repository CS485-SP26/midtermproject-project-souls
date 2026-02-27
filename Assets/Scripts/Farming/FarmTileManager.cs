using System.Collections.Generic;
using Art.UI.ProgressBar;
using UnityEngine;
using UnityEditor;
using Environment;
using Core;

namespace Farming
{
    public class FarmTileManager : MonoBehaviour
    {
        [SerializeField] private GameObject farmTilePrefab;
        [SerializeField] private DayController dayController;
        [SerializeField] private int rows = 4;
        [SerializeField] private int cols = 4;
        [SerializeField] private float tileGap = 0.1f;
        private List<FarmTile> tiles = new List<FarmTile>();

        public SimpleProgressBar progressBar;

        void Start()
        {
            Debug.Assert(farmTilePrefab, "FarmTileManager requires a farmTilePrefab");
            Debug.Assert(dayController, "FarmTileManager requires a dayController");

            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<FarmTile>(out FarmTile tile))
                    tiles.Add(tile);
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].tileID = i;
                if (GameManager.Instance.TileData.TryGetTile(i, out var saved))
                    tiles[i].LoadState(saved.condition, saved.daysSinceInteraction);
            }

            UpdateProgressBar();
        }

        void OnEnable()
        {
            dayController.dayPassedEvent.AddListener(OnDayPassed);
        }

        void OnDisable()
        {
            dayController.dayPassedEvent.RemoveListener(OnDayPassed);
        }

        public void OnDayPassed()
        {
            IncrementDays(1);
            UpdateProgressBar();
        }

        public void IncrementDays(int count)
        {
            while (count > 0)
            {
                foreach (FarmTile tile in tiles)
                    tile.OnDayPassed();
                count--;
            }
        }

        public void UpdateProgressBar()
        {
            if (progressBar == null) return;
            int readyCount = 0;
            foreach (FarmTile tile in tiles)
            {
                if (tile.GetCondition == FarmTile.Condition.Watered)
                    readyCount++;
            }

            progressBar.SetProgress(readyCount, 0, tiles.Count);
        }

        // ***************************************************************** //
        // Below this line is code to support the Unity Editor (Advanced)
        // Please feel free to disregard everything below this
        // ***************************************************************** //

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
                    clone.transform.parent = transform;
                    tiles.Add(clone.GetComponent<FarmTile>());
                }

                spawnPos.z += clone.transform.localScale.z + tileGap;
                spawnPos.x = transform.position.x;
            }
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                if (Application.isPlaying) return;
                ValidateGrid();
            };
#endif
        }

        void ValidateGrid()
        {
            if (!farmTilePrefab) return;
            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<FarmTile>(out var tile))
                    tiles.Add(tile);
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