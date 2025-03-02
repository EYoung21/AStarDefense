using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1); //size increase on hover
    public Color hoverColor = Color.yellow; //change this to your preferred color
    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;

    void Start()
    {
        originalScale = transform.localScale; //store original size
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; //store original color
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; //increase size
        buttonImage.color = hoverColor; //change color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; //reset size
        buttonImage.color = originalColor; //reset color
    }
}

