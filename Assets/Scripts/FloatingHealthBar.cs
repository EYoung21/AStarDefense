using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    [SerializeField] private Slider slider;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;

    public void UpdateHealthBar(float currValue, float maxValue) {
        slider.value = currValue / maxValue;
    }

    void Start() {
        // mainCamera = GameObject.FindGameObjectWithTag("TheGameCamera").GetComponent<Camera>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        transform.position = target.position + offset;
    }
}
