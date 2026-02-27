using UnityEngine;

public class BuySeeds : MonoBehaviour //basically the same code from farmingtrigger repurposed for the buying seeds hitbox
{

    public GameObject buySeedsButton;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buySeedsButton.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buySeedsButton.SetActive(false);
        }
    }
}
