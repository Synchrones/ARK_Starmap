using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTailScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    private float offset;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        offset = 0;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset = new Vector2(offset, 0);
        offset -= 0.0003f;
    }
}
