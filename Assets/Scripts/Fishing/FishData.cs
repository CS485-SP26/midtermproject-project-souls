using UnityEngine;

[System.Serializable]
public class FishData
{
    public string fishName;
    public GameObject fishModelPrefab;
    [Range(0f, 1f)]
    public float spawnChance;
}