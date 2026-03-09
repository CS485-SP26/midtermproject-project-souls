using Core;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        if (FundsManager.Instance != null)
        {
            FundsManager.Instance.OnFundsChanged += UpdateUI;
            UpdateUI(FundsManager.Instance.Get());
        }
        else
        {
            Debug.LogError("FundsManager.Instance is null! Make sure a FundsManager exists in the scene.");
        }
    }

    private void OnEnable()
    {
        FundsManager.Instance.OnFundsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        FundsManager.Instance.OnFundsChanged -= UpdateUI;
    }

    private void UpdateUI(int amount)
    {
        if (moneyText)
        {
            moneyText.SetText($"Funds: ${amount}");
        }
    }
}
