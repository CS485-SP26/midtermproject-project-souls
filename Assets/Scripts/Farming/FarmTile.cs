using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Farming
{
    public class FarmTile : MonoBehaviour , IWaterable //newly added interface
    {
        public enum Condition { Grass, Tilled, Watered, Planted }

        [SerializeField] private Condition tileCondition = Condition.Grass; 

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        MeshRenderer tileRenderer;

        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        List<Material> materials = new List<Material>();

        private int daysSinceLastInteraction = 0;
        public FarmTile.Condition GetCondition { get { return tileCondition; } } // TODO: Consider what the set would do?

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");

            foreach (Transform edge in transform)
            {
                materials.Add(edge.gameObject.GetComponent<MeshRenderer>().material);
            }
        }

        public void Interact()
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled: Water(); break;
                case FarmTile.Condition.Watered: Debug.Log("Ready for planting"); break;
                //new Interact
                case FarmTile.Condition.Planted: Harvest(); break;
            }
            daysSinceLastInteraction = 0;
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
        }
    }
}