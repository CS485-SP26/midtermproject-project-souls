using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFarmScene : MonoBehaviour
{
    public void GoToFarm()
    {
        SceneManager.LoadScene("Scene1-FarmingSim");
    }
}