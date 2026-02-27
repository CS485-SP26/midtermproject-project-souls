using Core;
using UnityEngine;
using TMPro;

public class DisplayFunds : MonoBehaviour
{
    [SerializeField] TMP_Text fundsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changeFundsDisplay();
    }

    public void changeFundsDisplay()
    {
        fundsText.text = "Funds: $" + GameManager.Instance.Funds.Amount;
    }
}
