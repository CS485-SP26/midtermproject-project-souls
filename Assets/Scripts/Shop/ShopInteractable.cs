using Character;
using UnityEngine;

public class ShopUI : MonoBehaviour, IInteractable, IUI
{
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private PlayerController playerController;

    private bool playerInRange = false;
    private bool UIOpen = false;

    void Start()
    {
        interactPrompt.SetActive(false); //hides button until collision
        shopUI.SetActive(false);
    }
    
    public void Interact()
    {   
        if(!playerInRange) return;   // Player is too far
        if(UIOpen) {
            this.Close();
            return;
        }; // Closes shop if E is hit again
        shopUI.SetActive(true);
        interactPrompt.SetActive(false);
        playerUI.SetActive(false);
        playerController.canMove = false; 
        UIOpen = true;
    }

    public void Close()
    {
        shopUI.SetActive(false);
        interactPrompt.SetActive(true);
        playerUI.SetActive(true);
        playerController.canMove = true;
        UIOpen = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         playerInRange = true;
         interactPrompt.SetActive(true);   
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactPrompt.SetActive(false);
        }
    }

    public bool getOpen()
    {
        return UIOpen;
    }
}