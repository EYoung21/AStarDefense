using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    [SerializeField] protected SpriteRenderer _renderer;
    //protected: effectively private, but derrived tiles (from this class) can also access it

    [SerializeField] private GameObject _highlight;
    
    //abstract class and virtual func combo!: allows this function to be defaulted to this in every instance, but if redefined will have logic specific to another tile
    public virtual void init(int x, int y) {
        
    }

    void OnMouseEnter() {
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    } 
}
