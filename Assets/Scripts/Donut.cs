using UnityEngine;

public class Donut : MonoBehaviour
{
    public int Id;
    private Renderer _material;
    
    public void Init(DonutType Type)
    {
        Id = Type.Id;
        _material = GetComponent<Renderer>();
        _material.material.color = Type.Color;
    }
}
