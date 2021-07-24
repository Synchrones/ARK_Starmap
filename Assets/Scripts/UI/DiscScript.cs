using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscScript : MonoBehaviour
{

    public GameObject Disc;
    public bool isActive = false;
    // Update is called once per frame
    void Start()
    {
        Disc.GetComponent<RawImage>().enabled = false;
    }
    void Update()
    {
        
    }

    public void LoadDisc()
    {
        isActive = true;
        Disc.GetComponent<RawImage>().enabled = true;
    }

    public void UnloadDisc()
    {
        isActive = false;
        Disc.GetComponent<RawImage>().enabled = false;
    }

}
