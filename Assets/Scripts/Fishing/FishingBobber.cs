using UnityEngine;
using System;
using System.Collections;
using Character;

public class FishingBobber : MonoBehaviour
{
    [SerializeField] public float minBiteTime = 2f;
    [SerializeField] public float maxBiteTime = 6f;
    [SerializeField] public float autoReelDelay = 2f;

    private bool fishBiting = false;

    public Action OnFishCaught;
    public AnimatedController playerAnim;

    void Start()
    {
        StartCoroutine(FishBiteLoop());
    }

    IEnumerator FishBiteLoop()
    {
        while (true)
        {
            float wait = UnityEngine.Random.Range(minBiteTime, maxBiteTime);
            yield return new WaitForSeconds(wait);

            FishBite();
        }
    }

    void FishBite()
    {
        fishBiting = true;
        Debug.Log("Got a bite!");

        StartCoroutine(BobAnimation());
        StartCoroutine(AutoReel());
    }

    IEnumerator BobAnimation()
    {
        Vector3 start = transform.position;

        for (int i = 0; i < 3; i++)
        {
            transform.position = start + Vector3.down * 0.2f;
            yield return new WaitForSeconds(0.15f);

            transform.position = start;
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator AutoReel()
    {
        yield return new WaitForSeconds(autoReelDelay);

        if (fishBiting)
        {
            fishBiting = false;
            OnFishCaught?.Invoke();

            if (playerAnim != null)
                playerAnim.FishingReel();

            Debug.Log("Auto-reeling fish!");
            Destroy(gameObject);
        }
    }

    public bool TryCatch()
    {
        if (fishBiting)
        {
            fishBiting = false;
            OnFishCaught?.Invoke();
            return true;
        }

        return false;
    }

    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.0005f;
    }
}