using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private AnimationCurve itemCurve;
    [SerializeField] private int amountOfItems;
    [SerializeField] private GameObject itemToSpawn;

    private Vector2[] positions;

    private void GeneratePositions()
    {
        positions = new Vector2[amountOfItems];

        float curPosition = itemCurve[0].time;

        float addpercent = Mathf.Abs(itemCurve[0].time - itemCurve[itemCurve.length-1].time) / amountOfItems;

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = transform.position + new Vector3(curPosition, itemCurve.Evaluate(curPosition), 0);
            curPosition += addpercent;
        }
    }
    
    
     private void OnDrawGizmos()
    {
        GeneratePositions();
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i], 0.1f);
        }
    }
}
