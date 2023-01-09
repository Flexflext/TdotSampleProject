using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    
    [SerializeField] private AnimationCurve itemCurve;
    [Min(1)]
    [SerializeField] private int amountOfItems;
    [Min(1)]
    [SerializeField] private int rowAmount;
    [SerializeField] private float rowSpacing;
    [SerializeField] private float collumSpacing;
    [SerializeField] private GameObject itemToSpawn;

    private Vector2[,] positions;

    private void Awake()
    {
        GeneratePositions();
        SpawnItems();
    }

    private void GeneratePositions()
    {
        positions = new Vector2[amountOfItems, rowAmount];

        float curPosition = itemCurve[0].time;

        float addpercent = Mathf.Abs(itemCurve[0].time - itemCurve[itemCurve.length-1].time) / amountOfItems;

        
            for (int i = 0; i < amountOfItems; i++)
            {
                for (int j = 0; j < rowAmount; j++)
                {
                    positions[i, j] = transform.position + new Vector3(curPosition, itemCurve.Evaluate(curPosition) * collumSpacing, 0) + ((Vector3.up * rowSpacing) * j);
                }
                curPosition += addpercent;
            }
    }

    private void SpawnItems()
    {
        for (int i = 0; i < amountOfItems; i++)
        {
            for (int j = 0; j < rowAmount; j++)
            {
                GameObject itemSpawned = Instantiate(itemToSpawn, positions[i, j], Quaternion.identity);
                itemSpawned.transform.SetParent(this.transform);
            }
        }
    }
    
    
     private void OnDrawGizmos()
    {
        GeneratePositions();
        for (int i = 0; i < amountOfItems; i++)
        {
            for (int j = 0; j < rowAmount; j++)
            {
                Gizmos.DrawSphere(positions[i, j], 0.1f);
            }
        }
    }
}
