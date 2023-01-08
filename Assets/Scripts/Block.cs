using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Block : MonoBehaviour
{
    [SerializeField] private Transform[] _placeForDonut;
    
    private Tile _currentTile;
    public int donutCount => donutsList.Count;
    public Tile currentTile => _currentTile;

    public List<Donut> donutsList = new List<Donut>();

    public Donut currentTopDonut => donutsList[^1];

    public void Init(List<Donut> donutsToAdd)
    {
        foreach (var donut in donutsToAdd)
        {
            donutsList.Add(donut);
            donut.transform.SetParent(PlaceToMoveDonut());
        }
        ResetDonutsLocalPositionOrKill();
    }
    
    public void MoveToNewTile(Tile tile)
    {
        if(_currentTile != null)
            _currentTile.OccupiedBlock = null;
        _currentTile = tile;
        _currentTile.OccupiedBlock = this;
    }

    public void AddDonut(Donut donut)
    {
        if (donutsList.Count < 3)
        {
            donutsList.Add(donut);
            Debug.Log("added");
            donut.transform.SetParent(PlaceToMoveDonut());
        }
        
    }

    public Donut RemoveDonut()
    {
        Donut topDonut = donutsList.Last();
        donutsList.Remove(donutsList.Last());
        
        if (donutsList.Count == 0)
        {
            ReclaimBlock();
        }
        return topDonut;
    }

    
    public Transform PlaceToMoveDonut()
    {
        return _placeForDonut[donutsList.Count-1];
    }

    public void ResetDonutsLocalPositionOrKill()
    {
        foreach (var donut in donutsList)
        {
            donut.transform.localPosition = Vector3.zero;
        }
        if (donutsList.Count == 3 && donutsList[0].Id == donutsList[1].Id && donutsList[1].Id == donutsList[2].Id)
        {
            ReclaimBlock();
        }
    }


    private void ReclaimBlock()
    {
        _currentTile.OccupiedBlock = null;
        Destroy(this.gameObject);
    }
}
