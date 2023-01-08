using UnityEngine;

public class InputController : MonoBehaviour
{
    private GameObject Target()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }
    
    public Tile GetTile()
    {
        if (Target() != null && Target().transform.GetComponentInParent<Tile>() != null)
        {
             Tile tile = Target().transform.GetComponentInParent<Tile>();
            return tile;
        }
        else
            return null;
    } 
}
