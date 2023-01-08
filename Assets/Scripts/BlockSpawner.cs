using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private Block _prefabBlock;
    [SerializeField] private Transform _previewSpot;
    [SerializeField] private List<DonutType> _types;
    [SerializeField] private Donut _prefabDonut;

    public DonutType GetRandomDonutType()
    {
        float value = Random.Range(0f, _types.Count - 0.01f);
        return _types[(int)value];
    }
    
    public Block SpawnBlock()
    {
        Block block = Instantiate(_prefabBlock,_previewSpot,false);
        transform.localPosition = _previewSpot.position;

        int donutsCount = (int)Random.Range(1f, 3.99f);
        List<Donut> donutsToAdd = new List<Donut>();
        for(int i=0; i<donutsCount; i++)
        {
            Donut donutToAdd = Instantiate(_prefabDonut);
            if (i == 2 && donutsToAdd[0].Id == donutsToAdd[1].Id)
            {
              donutToAdd.Init(_types.First(b=> b.Id != donutsToAdd[1].Id));  
            }
            else
            {
                donutToAdd.Init(GetRandomDonutType());  
            }
            donutsToAdd.Add(donutToAdd);
        }
        block.Init(donutsToAdd);
        return block;
    }
    
    
    
    
}
[Serializable]
public struct DonutType
{
    public int Id;
    public Color Color;
}