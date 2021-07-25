using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoboxScript : MonoBehaviour
{

    public Text Object;
    public Text Size;
    public Text Type;
    public Text Affiliation;
    public Text Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadInfobox(GameObject gameObject, int mode)
    {
        if(mode == 0)
        {
            SystemsInfosScript systemInfos = gameObject.GetComponent<SystemsInfosScript>();
            Object.text = "STAR SYSTEM";
            Size.text = systemInfos.size;
            Type.text = systemInfos.type;
            Affiliation.text = systemInfos.affiliationName;
            Name.text = systemInfos.systemName;
        }
        else
        {
            print("WIP");
        }
    }

    
}
