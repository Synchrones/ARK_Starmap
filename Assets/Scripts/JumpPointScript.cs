using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPointScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    private float offset;
    Transform cameraTransform;
    Transform jumpHeadTransform;
    void Start()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        offset = 0;
        cameraTransform = Camera.main.transform;
        jumpHeadTransform = transform.GetChild(1).transform;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset = new Vector2(offset, 0);
        offset -= 0.0003f;

        jumpHeadTransform.LookAt(cameraTransform);
    }
}
