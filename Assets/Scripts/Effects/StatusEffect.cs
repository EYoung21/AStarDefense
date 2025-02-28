using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float duration = 2f;
    
    private void Start()
    {
        if (particles != null)
        {
            var main = particles.main;
            main.duration = duration;
            Destroy(gameObject, duration);
        }
    }
} 