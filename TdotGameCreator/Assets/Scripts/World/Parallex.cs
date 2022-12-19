using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    private float lenght;
    private float startPos;
    private Transform camTransform;
    [Range(0,1)]
    [SerializeField] private float moveWithCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float temp = (camTransform.transform.position.x * (1 - moveWithCamera));
        float distance = (camTransform.transform.position.x * moveWithCamera);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (temp > startPos + lenght)
        {
            startPos += lenght;
        }
        else if (temp < startPos - lenght)
        {
            startPos -= lenght;
        }
    }
}
