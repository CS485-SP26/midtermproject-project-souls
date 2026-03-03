using Core;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        FundsManager.Instance.OnFundsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        FundsManager.Instance.OnFundsChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI(FundsManager.Instance.Get());
    }

    private void UpdateUI(int amount)
    {
        moneyText.text = "Funds: $" + amount;
    }
}
