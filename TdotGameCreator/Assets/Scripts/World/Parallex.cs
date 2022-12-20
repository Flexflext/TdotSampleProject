using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Parallex : MonoBehaviour
{
    [System.Serializable]
    struct ParallexBackground
    {
        [SerializeField] public SpriteRenderer rendererObject;
        [Range(0,1)]
        public float moveWithCamera;
        [HideInInspector]
        public float Lenght;
        [HideInInspector]
        public float StartPos;
    }


    [SerializeField] private ParallexBackground[] parallexBackgrounds;
    private Transform camTransform;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < parallexBackgrounds.Length; i++)
        {
            parallexBackgrounds[i].StartPos = parallexBackgrounds[i].rendererObject.transform.position.x;
            parallexBackgrounds[i].Lenght = parallexBackgrounds[i].rendererObject.bounds.size.x;
        }
        
        camTransform = Camera.main.transform;
        
    }

    private void LateUpdate()
    {
        float temp = 0;
        float distance = 0;
        
        for (int i = 0; i < parallexBackgrounds.Length; i++)
        {
            temp = (camTransform.transform.position.x * (1 - parallexBackgrounds[i].moveWithCamera));
            distance = (camTransform.transform.position.x * parallexBackgrounds[i].moveWithCamera);

            parallexBackgrounds[i].rendererObject.transform.position = new Vector3(parallexBackgrounds[i].StartPos + distance, parallexBackgrounds[i].rendererObject.transform.position.y, parallexBackgrounds[i].rendererObject.transform.position.z);

            if (temp > parallexBackgrounds[i].StartPos + parallexBackgrounds[i].Lenght)
            {
                parallexBackgrounds[i].StartPos += parallexBackgrounds[i].Lenght;
            }
            else if (temp < parallexBackgrounds[i].StartPos - parallexBackgrounds[i].Lenght)
            {
                parallexBackgrounds[i].StartPos -= parallexBackgrounds[i].Lenght;
            }
        }
        
        
    }
}
