using UnityEngine;
using TMPro;
using Core;

public class DisplaySeeds : MonoBehaviour //basically a copy of display funds
{
    [SerializeField] TMP_Text seedsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        changeSeedsDisplay();
    }

    public void changeSeedsDisplay()
    {
        seedsText.text = "Seeds: " + GameManager.Instance.Seeds.Amount;
    }
}
