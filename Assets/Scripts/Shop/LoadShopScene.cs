using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShopScene : MonoBehaviour
{
    public void GoToShop()
    {
        SceneManager.LoadScene("Scene2-Shop");
    }
}
