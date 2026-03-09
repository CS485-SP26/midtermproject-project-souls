using UnityEngine;
using Character;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

public class FishingSpot : MonoBehaviour, IInteractable
{
    [Header("Fishing Settings")]
    public GameObject bobberPrefab;
    public Transform castPoint;

    [Header("Fish Data Table")]
    public List<FishData> possibleFish;

    private FishingBobber currentBobber;

    public void Interact(GameObject interactor)
    {
        PlayerController player = interactor.GetComponent<PlayerController>();
        AnimatedController anim = interactor.GetComponent<AnimatedController>();

        if (currentBobber == null)
        {
            // Disable movement and show rod
            player.canMove = false;
            player.fishingRod.SetActive(true);

            // Play cast animation
            if (anim != null) anim.FishingCast();
            Debug.Log("casting anim triggered");

            // Spawn bobber
            Cast(anim);
        }
    }

    void Cast(AnimatedController anim)
    {
        GameObject bobber = Instantiate(
            bobberPrefab,
            castPoint.position + transform.forward * 0.5f,
            Quaternion.identity
        );

        currentBobber = bobber.GetComponent<FishingBobber>();
        currentBobber.playerAnim = anim;
        currentBobber.OnFishCaught += () =>
        {
            FishData caughtFish = ChooseRandomFish();
            Debug.Log(caughtFish);
            ShowFishPopup(caughtFish);
            CatchFish();
        };
    }

    void CatchFish()
    {
        Debug.Log("Fish caught automatically!");

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.canMove = true;
            player.fishingRod.SetActive(false);
        }

        currentBobber = null;
    }

    FishData ChooseRandomFish()
    {
        float total = 0f;
        Debug.Log("Choosing random fish");

        foreach(var fish in possibleFish)
            total += fish.spawnChance;
        
        float rand = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var fish in possibleFish)
        {
            cumulative += fish.spawnChance;
            if (rand <= cumulative)
            {
                Debug.Log("Picked " + fish);
                return fish;   
            }
        }

        Debug.Log("No fish selected, default is selected: " + possibleFish[0]);
        return possibleFish[0]; //default
    }

    void ShowFishPopup(FishData fish)
    {
        if (fish == null || fish.fishModelPrefab == null || Camera.main == null)
        {
            Debug.LogWarning("Fish popup failed: missing ref");
            return;
        }
        
        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;

        Debug.Log("Spawning " + fish.fishName + " at " + spawnPos);
        GameObject popup = Instantiate(fish.fishModelPrefab, spawnPos, Quaternion.identity);
        popup.transform.localScale = Vector3.one * 0.5f;
        popup.transform.Rotate(0f, 90f, 0f);

        Destroy(popup, 1.5f);
    }


    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.canMove = true;
            player.fishingRod.SetActive(false);
        }
    }
}