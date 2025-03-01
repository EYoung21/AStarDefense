using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;

    public void UpdateHealthBar(float currValue, float maxValue) {
        slider.value = currValue / maxValue;
    }

    void Start() {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        transform.position = target.position + offset;
    }
}
