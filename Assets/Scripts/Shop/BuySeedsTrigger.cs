using UnityEngine;

public class BuySeeds : MonoBehaviour //basically the same code from farmingtrigger repurposed for the buying seeds hitbox
{

    public GameObject buySeedsButton;
    public GameObject SellOnePlantButton;
    public GameObject SellFivePlantButton;
    public GameObject SellAllPlantButton;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buySeedsButton.SetActive(true);
            SellOnePlantButton.SetActive(true);
            SellFivePlantButton.SetActive(true);
            SellAllPlantButton.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            buySeedsButton.SetActive(false);
            SellOnePlantButton.SetActive(false);
            SellFivePlantButton.SetActive(false);
            SellAllPlantButton.SetActive(false);
        }
    }
}
