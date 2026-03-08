using Core;
using UnityEngine;

public class PurchaseItem : MonoBehaviour
{

    public void buySeed() //this is only called by button, not accessed naturally
    {
        if(FundsManager.Instance.Amount >= 20) //check if we have enough money to purchase the seeds, must be 20 or higher
        {
            FundsManager.Instance.Add(-20);
            if (GameManager.Instance != null && GameManager.Instance.Seeds != null)
            {
                GameManager.Instance.Seeds.Add(1);
                SeedsManager.Instance.Add(1);
            }
        }
    }

}
