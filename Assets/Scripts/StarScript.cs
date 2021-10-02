using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour
{

    public Texture2D starTexture;    
    public float alpha;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 2;
    }
    
    // Update is called once per frame
    void Update()
    {
        var pixels = starTexture.GetPixels();
        for (var i=0 ; i < pixels.Length; i++) {
            var pixel = pixels[i];
            float grey = (float)(((double)pixel.r + (double)pixel.g + (double)pixel.b) / alpha);
            pixels[i].a = grey;
        }
        starTexture.SetPixels(pixels);
        starTexture.Apply(updateMipmaps:true);

        gameObject.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", starTexture);
        gameObject.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetTexture("_NoiseTex", starTexture);
    }
}
