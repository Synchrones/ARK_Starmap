using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoboxScript : MonoBehaviour
{

    public Text objecttype;
    public Text sizeHabitable;
    public Text sizeHabitableData;
    public Text type;
    public Text affiliation;
    public Text objectName;
    public Text description;

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
            objecttype.text = "STAR SYSTEM";
            sizeHabitable.text = "SIZE : ";
            sizeHabitableData.text = systemInfos.size;
            type.text = systemInfos.type;
            affiliation.text = systemInfos.affiliationName;
            objectName.text = systemInfos.systemName;
            description.text = systemInfos.description;
        }
        else
        {
            COInfosScript coInfosScript = gameObject.GetComponent<COInfosScript>();
            objecttype.text = coInfosScript.type;
            sizeHabitable.text = "HABITABLE : ";
            if(coInfosScript.habitable.Equals(""))
            {
                sizeHabitableData.text = "NO";
            }
            else
            {
                sizeHabitableData.text = "YES";
            }
            type.text = coInfosScript.subtype;
            affiliation.text = coInfosScript.affiliationName;
            objectName.text = coInfosScript.coName;
            description.text = coInfosScript.description;
        }
    }

    
}
