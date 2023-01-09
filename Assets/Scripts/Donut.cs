using UnityEngine;

public class Donut : MonoBehaviour
{
    public int Id;
    public Color color;
    private Renderer _material;
    
    public void Init(DonutType Type)
    {
        Id = Type.Id;
        _material = GetComponent<Renderer>();
        color = Type.Color;
        _material.material.color = color;
    }
}
