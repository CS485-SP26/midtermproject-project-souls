using Core;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        if (GameManager.Instance?.Funds != null)
            GameManager.Instance.Funds.OnFundsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Funds != null)
        {
            GameManager.Instance.Funds.OnFundsChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        if (GameManager.Instance?.Funds != null)
            UpdateUI(GameManager.Instance.Funds.Get());
    }

    private void UpdateUI(int amount)
    {
        if (moneyText)
        {
            moneyText.SetText($"Money: ${amount}");
        }
    }
}
