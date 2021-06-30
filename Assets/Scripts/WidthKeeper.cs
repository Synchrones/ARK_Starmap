using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidthKeeper : MonoBehaviour
{
    LineRenderer lineRenderer;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        AnimationCurve curve = new AnimationCurve();
        for (int i = 0; i <= 120; i += 10)
        {
            Vector3 pos = lineRenderer.GetPosition(i);
            float width = Vector3.Distance(mainCamera.transform.position, pos) / 400;
            curve.AddKey(i / 120, width);
            
        }
        lineRenderer.widthCurve = curve;
        
    }
}
