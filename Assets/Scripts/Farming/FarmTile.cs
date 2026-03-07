using System.Collections.Generic;
using UnityEngine;
using Environment;
using System;
using Core;
using Quest;

namespace Farming
{
    public class FarmTile : MonoBehaviour , IWaterable//newly added interface
    {
        public enum Condition { Grass, Tilled, Watered, Planted }

        [SerializeField] private Condition tileCondition = Condition.Grass; 

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        [SerializeField] private Transform plantSpawnPoint;
        MeshRenderer tileRenderer;

        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        List<Material> materials = new List<Material>();

        private int daysSinceLastInteraction = 0;
        public FarmTile.Condition GetCondition { get { return tileCondition; } } // TODO: Consider what the set would do?

        [HideInInspector] public int id = -1;
        private bool isLoading = false;
        
        public void LoadData()
        {
            isLoading = true;
            if (GameManager.Instance != null && GameManager.Instance.TileData != null)
            {
                if (GameManager.Instance.TileData.TryGetTile(id, out var data))
                {
                    Debug.Log($"[FarmTile] LoadData - Tile {id} found data: Condition={data.condition}, HasPlant={data.hasPlant}, Scale={data.plantScale}");
                    tileCondition = data.condition;
                    daysSinceLastInteraction = data.daysSinceInteraction;
                    
                    if (data.hasPlant)
                    {
                        GameObject prefabToPlant = null;
                        
                        // Try to find the specific plant prefab from PlayerFarming
                        Character.PlayerFarming playerFarming = FindFirstObjectByType<Character.PlayerFarming>();
                        if (playerFarming != null && playerFarming.PlantPrefabs != null)
                        {
                            foreach (GameObject p in playerFarming.PlantPrefabs)
                            {
                                if (p != null && (p.name == data.plantPrefabName || p.name + "(Clone)" == data.plantPrefabName))
                                {
                                    prefabToPlant = p;
                                    break;
                                }
                            }
                        }

                        // Fallback to FarmTileManager's set prefab if not found by name
                        if (prefabToPlant == null)
                        {
                            FarmTileManager manager = GetComponentInParent<FarmTileManager>();
                            if (manager != null)
                            {
                                prefabToPlant = manager.PlantPrefab;
                            }
                        }

                        if (prefabToPlant != null)
                        {
                            Plant(prefabToPlant);
                            if (currentPlant != null)
                            {
                                var growth = currentPlant.GetComponent<PlantGrowth>();
                                if (growth != null)
                                {
                                    growth.SetGrowthState(data.plantScale, data.plantFullyGrown);
                                    SaveData(); // Ensures the correct scale overrides the Plant() default scale save
                                }
                            }
                        }
                    }

                    UpdateVisual();
                }
                else
                {
                    Debug.Log($"[FarmTile] LoadData - Tile {id} found NO saved data in TileDataManager.");
                }
            }
            else
            {
                Debug.LogWarning($"[FarmTile] LoadData - Tile {id} failed. GameManager or TileDataManager is null!");
            }
            isLoading = false;
        }

        public void SaveData()
        {
            if (isLoading) return;

            if (GameManager.Instance != null && GameManager.Instance.TileData != null && id >= 0)
            {
                bool hasPlant = currentPlant != null;
                string prefabName = "";
                Vector3 scale = Vector3.zero;
                bool isFullyGrown = false;

                if (hasPlant)
                {
                    prefabName = currentPlant.name.Replace("(Clone)", "").Trim();
                    var growth = currentPlant.GetComponent<PlantGrowth>();
                    if (growth != null)
                    {
                        scale = growth.GetScale();
                        isFullyGrown = growth.CanHarvest();
                    }
                }

                Debug.Log($"[FarmTile] SaveData - Tile {id} saving: Condition={tileCondition}, HasPlant={hasPlant}, Scale={scale}");
                GameManager.Instance.TileData.SaveTile(id, tileCondition, daysSinceLastInteraction, hasPlant, prefabName, scale, isFullyGrown);
            }
            else
            {
                if (id < 0) Debug.LogWarning("[FarmTile] SaveData skipped because ID is less than 0.");
                else Debug.LogWarning("[FarmTile] SaveData skipped because GameManager or TileDataManager is null.");
            }
        }

        private GameObject currentPlant;

        void Awake()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");

            foreach (Transform edge in transform)
            {
                MeshRenderer renderer = edge.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    materials.Add(renderer.material);
                }
            }
        }

        public void Interact()
        {
            // temp debug helper
            Debug.Log("Tile interact condition " + tileCondition);

            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled: Water(); break;
                case FarmTile.Condition.Watered: Debug.Log("Ready for planting"); break;
                //new Interact
                case FarmTile.Condition.Planted: Harvest(); break;
            }
            daysSinceLastInteraction = 0;
            SaveData();
        }

        public void Till()
        {
            tileCondition = FarmTile.Condition.Tilled;
            UpdateVisual();
            tillAudio?.Play();
        }

        public void Water(float amount = 0f)
        {
            tileCondition = FarmTile.Condition.Watered;
            UpdateVisual();
            waterAudio?.Play();
        }


        public void Plant(GameObject plantPrefab)
        {
            if (currentPlant != null) return;
            if (plantSpawnPoint == null || plantPrefab == null) return;

            currentPlant = Instantiate(plantPrefab, plantSpawnPoint.position, plantSpawnPoint.rotation);

            currentPlant.transform.SetParent(plantSpawnPoint, true);
            currentPlant.transform.localPosition = Vector3.zero;
            currentPlant.transform.localRotation = Quaternion.identity;

            Debug.Log("Spawn world pos: " + plantSpawnPoint.position);
            Debug.Log("Tile world pos: " + transform.position);

            tileCondition = Condition.Planted;
            
            PlantGrowth growth = currentPlant.GetComponent<PlantGrowth>();
            if (growth != null)
            {
                // Unsubscribe first just in case to clearly prevent duplicates, though this is a fresh plant
                growth.OnSizeChanged.RemoveListener(OnPlantSizeChanged);
                growth.OnSizeChanged.AddListener(OnPlantSizeChanged);
            }

            SaveData();
        }

        private void OnPlantSizeChanged(Vector3 newScale)
        {
            SaveData();
        }

        public void Harvest()
        {
            //if there is no plant on tile
            if (currentPlant == null) return;
            //check if plant is harvestable
                IHarvestable harvestable = currentPlant.GetComponent<IHarvestable>();
            //if it's not fully grown
            if (!harvestable.CanHarvest())
            {
                Debug.Log("Plant is not ready to harvest.");
                return;
            }
            
            //see plants value
            int value = harvestable.Harvest();
            Debug.Log("Harvested for " + value);

            //Adding plants to be sold
            GameManager.Instance.AddPlants(1);  

            //remove the plant from the scene.
            Destroy(currentPlant);
            //clear object reference
            currentPlant = null;
            //after harvesting, for now changes to tilled, we'll decide if we wanna change tht
            tileCondition = Condition.Tilled;
            //update visuals.
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if(tileRenderer == null) return;
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: tileRenderer.material = grassMaterial; break;
                case FarmTile.Condition.Tilled: tileRenderer.material = tilledMaterial; break;
                case FarmTile.Condition.Watered: tileRenderer.material = wateredMaterial; break;
                case FarmTile.Condition.Planted: tileRenderer.material = tilledMaterial; break;
            }
        }

        public void SetHighlight(bool active)
        {
            foreach (Material m in materials)
            {
                if (active)
                {
                    m.EnableKeyword("_EMISSION");
                } 
                else 
                {
                    m.DisableKeyword("_EMISSION");
                }
            }
            if (active) stepAudio.Play();
        }

        public void OnDayPassed()
        {
            daysSinceLastInteraction++;
            if(daysSinceLastInteraction >= 2)
            {
                if(tileCondition == FarmTile.Condition.Watered) tileCondition = FarmTile.Condition.Tilled;
                else if(tileCondition == FarmTile.Condition.Tilled) tileCondition = FarmTile.Condition.Grass;
            }
            UpdateVisual();
            SaveData();
        }
    }
}