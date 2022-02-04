using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiscScript : MonoBehaviour
{

    public GameObject Disc;
    public bool isActive = false;
    public GameObject selectedObject;
    public int mode;
    public TextMeshProUGUI systemName;
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
            this.GetComponent<InfoboxScript>().UnloadInfobox();
        }

        if(isActive == true)
        {
            Disc.transform.position = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
        }
    }

    public void LoadDisc()
    {
        if(mode == 0)
        {
            isActive = true;
            Disc.gameObject.SetActive(true);
            systemName.text = selectedObject.GetComponent<SystemsInfosScript>().code;
        }
        else
        {
            isActive = true;
            Disc.gameObject.SetActive(true);
            systemName.text = "";
        }
    }

    public void UnloadDisc()
    {
        isActive = false;
        Disc.gameObject.SetActive(false);
    }

    public void LoadInfobox()
    {
        this.GetComponent<InfoboxScript>().LoadInfobox(selectedObject, mode);
    }
    public void UnloadInfobox()
    {
        this.GetComponent<InfoboxScript>().UnloadInfobox();
    }

}
