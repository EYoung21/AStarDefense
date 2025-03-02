using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1); // Size increase on hover
    public Color hoverColor = Color.yellow; // Change this to your preferred color
    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;

    void Start()
    {
        originalScale = transform.localScale; // Store original size
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color; // Store original color
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; // Increase size
        buttonImage.color = hoverColor; // Change color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // Reset size
        buttonImage.color = originalColor; // Reset color
    }
}

