using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour
{

    public Texture2D starTexture;    
    public float alpha;
    private float savedalpha;

    private GameObject innerSphere;
    private GameObject outerSphere;

    public float rotation1;
    public float rotation2;

    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        alpha = 2;

        outerSphere = gameObject.transform.GetChild(0).gameObject;
        innerSphere = gameObject.transform.GetChild(1).gameObject;
        rotationSpeed = 0.5f;

    }
    
    // Update is called once per frame
    void Update()
    {
        if(savedalpha != alpha)
        {
            var pixels = starTexture.GetPixels();
            for (var i=0 ; i < pixels.Length; i++) {
                var pixel = pixels[i];
                float grey = (float)(((double)pixel.r + (double)pixel.g + (double)pixel.b) / alpha);
                pixels[i].a = grey;
            }
            starTexture.SetPixels(pixels);
            starTexture.Apply(updateMipmaps:true);

            outerSphere.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", starTexture);
            outerSphere.GetComponent<Renderer>().sharedMaterial.SetTexture("_NoiseTex", starTexture);

            innerSphere.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", starTexture);
            innerSphere.GetComponent<Renderer>().sharedMaterial.SetTexture("_NoiseTex", starTexture);

            savedalpha = alpha;
        }
        
        outerSphere.transform.Rotate(0, Time.deltaTime * (Mathf.Acos(rotation1) - 0.25f * Mathf.PI) * rotationSpeed, 0);
        innerSphere.transform.Rotate(0, Time.deltaTime * (Mathf.Acos(rotation2) - 0.25f * Mathf.PI) * 2 * rotationSpeed, 0);
        
    }
}
