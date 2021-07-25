using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscScript : MonoBehaviour
{

    public GameObject Disc;
    public bool isActive = false;
    public GameObject selectedObject;
    // Update is called once per frame
    void Start()
    {
        Disc.gameObject.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            UnloadDisc();
        }

        if(isActive == true)
        {
            Disc.transform.position = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
        }
    }

    public void LoadDisc()
    {
        isActive = true;
        Disc.gameObject.SetActive(true);
    }

    public void UnloadDisc()
    {
        isActive = false;
        Disc.gameObject.SetActive(false);
    }

}
