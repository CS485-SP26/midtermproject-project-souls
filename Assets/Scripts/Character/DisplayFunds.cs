using Core;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        GameManager.Instance.Funds.OnFundsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.Funds.OnFundsChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI(GameManager.Instance.Funds.Get());
    }

    private void UpdateUI(int amount)
    {
        moneyText.text = "Funds: $" + amount;
    }
}
