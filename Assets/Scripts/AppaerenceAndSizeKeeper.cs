using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AppaerenceAndSizeKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    Transform cameraTransform;
    public float maxSize;
    public float minSize;
    public float scaleMultiplier;
    public bool isCelestialObject;

    public GameObject rescalingTextGO;
    public GameObject rescalingCircleGO;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        minSize = 0.001f;
        maxSize = 200;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraTransform.rotation;

        if(isCelestialObject)
        {
            float size = Vector3.Distance(transform.position, cameraTransform.position) / 10 * scaleMultiplier;
            size = Mathf.Clamp(size, minSize, maxSize);
            rescalingTextGO.GetComponent<TextMeshPro>().fontSize = size * 5;
            size *= 0.5f;
            if(size < 0.08f)
            {
                rescalingCircleGO.SetActive(false);
            }
            else
            {
                rescalingCircleGO.SetActive(true);
                rescalingCircleGO.transform.localScale = new Vector3(size, size, size);
            }
        }
        else
        {
            
            float size = Vector3.Distance(transform.position, cameraTransform.position) / 10 * scaleMultiplier;
            size = Mathf.Clamp(size, minSize, maxSize);
            transform.localScale = new Vector3(size, size, size);
        }
         
    }
}
