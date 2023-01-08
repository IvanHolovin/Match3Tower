using UnityEngine;

public class Tile : MonoBehaviour
{
    private int _x;
    private int _y;

    public int X => _x; 
    public int Y => _y;

    public Block OccupiedBlock;
        
    public void Init(int xPos, int yPos)
    {
        _x = xPos;
        _y = yPos;
    }
        
}
