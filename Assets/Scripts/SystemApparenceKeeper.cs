using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemApparenceKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    Transform cameraTransform;
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform);
        transform.Rotate(Vector3.up, 180);
        float size = Vector3.Distance(transform.position, cameraTransform.position) / 10;
        transform.localScale = new Vector3(size, size, size);
         
    }
}
