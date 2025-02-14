using UnityEngine;

public class Tile : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] SpriteRenderer _renderer;

    [SerializeField] private GameObject _highlight;

    public void init(bool isOffset) {
        Color tileColor = isOffset ? _offsetColor : _baseColor;
        tileColor.a = 1f; //force alpha (transparency level) to 1
        _renderer.color = tileColor;
    }

    void OnMouseEnter() {
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    } 
}
