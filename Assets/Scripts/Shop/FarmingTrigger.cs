using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmTrigger : MonoBehaviour
{
    public GameObject goToFarmButton;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            goToFarmButton.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            goToFarmButton.SetActive(false);
        }
    }

}