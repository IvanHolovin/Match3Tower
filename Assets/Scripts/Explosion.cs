using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _duration = 1f;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private Material _material;

    private float _timer = 0f;

    public void Initialize(Vector3 position, float flashRadius, Color color)
    {
        _material.color = color;
        transform.position = position;
        transform.localScale = Vector3.one * flashRadius;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _duration)
                Destroy(this);
    }
    
}
