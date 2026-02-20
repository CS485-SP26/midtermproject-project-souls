using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepletingBar : MonoBehaviour //behavior for a bar that depletes from an image
{
    [SerializeField] private Image fillImage; //draw order for images in unity editor is top to bottom, keep in mind.

    public void SetFill(float value)
    {
        fillImage.fillAmount = value;
    }

    public float Fill { get { return fillImage.fillAmount; } set { fillImage.fillAmount = value; } } //get and set

}