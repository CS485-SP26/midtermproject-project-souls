using Core;
using UnityEngine;

public class Sell : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SellOneButton() {
        SellPlant(1);
        }
    public void SellFiveButton(){SellPlant(5);}
    public void SellMaxButton() {SellPlant(GameManager.Instance.GetPlants());}
    private void SellPlant(int amount) {
        if (GameManager.Instance.GetPlants() >= amount) {
            GameManager.Instance.Funds.Add(amount * 5);
            GameManager.Instance.AddPlants(-amount);
            Debug.Log("Added amount");
        }
    }
}

