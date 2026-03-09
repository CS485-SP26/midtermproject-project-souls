using UnityEngine;
using Character; // For AnimatedController
using System;

public class PlayerFishing : MonoBehaviour
{
    [Header("Fishing Settings")]
    public GameObject bobberPrefab;
    public Transform castPoint;

    [Header("References")]
    public Animator animator;
    public AnimatedController animatedController;

    private FishingBobber currentBobber;

   
    public void TryFish()
    {
        if (currentBobber == null)
            CastLine();
    }

    private void CastLine()
    {
        if (animatedController != null)
            animatedController.FishingCast();

        GameObject bobber = Instantiate(
            bobberPrefab,
            castPoint.position + transform.forward * 0.5f,
            Quaternion.identity
        );

        currentBobber = bobber.GetComponent<FishingBobber>();
        if (currentBobber != null)
        {
            currentBobber.playerAnim = animatedController;

            currentBobber.OnFishCaught += CatchFish;
        }
    }


    private void CatchFish()
    {
        Debug.Log("Fish caught automatically!");

        // Destroy bobber
        if (currentBobber != null)
        {
            Destroy(currentBobber.gameObject);
            currentBobber = null;
        }


        if (animatedController != null)
            animatedController.FishingReel();
    }
}