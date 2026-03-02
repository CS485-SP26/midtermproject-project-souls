using Character;
using Farming;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class WaterPickup : MonoBehaviour
{
    [SerializeField] private AudioSource pickupAudio;
    [SerializeField] private PlayerFarming farmer; //refill water wont know what to do unless you make this a serializefield
    MeshRenderer objectRenderer;
    List<Material> materials = new List<Material>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectRenderer = GetComponent<MeshRenderer>();
        Debug.Assert(objectRenderer, "An object with water pickup requires a MeshRenderer");
        foreach (Transform edge in transform)
        {
            materials.Add(edge.gameObject.GetComponent<MeshRenderer>().material);
        }

        SetHighlight(true);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, 3, 0); //make the object this script is attached to spin
    }

    public void SetHighlight(bool active)
    {
        foreach (Material m in materials)
        {
            if (active)
            {
                m.EnableKeyword("_EMISSION");
            } 
            else 
            {
                m.DisableKeyword("_EMISSION");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("water pickup trigger entered");
        if (other.CompareTag("Player"))
        {
            //Debug.Log("player tag detected in water pickup hitbox");
            pickupAudio?.Play(); //play the audio
            StartCoroutine(farmer.RefillWater()); //!!dont forget to always put StartCouroutine in front of IEnumerable function calls!!
        }
    }

}
