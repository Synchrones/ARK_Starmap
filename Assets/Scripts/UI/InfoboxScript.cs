using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoboxScript : MonoBehaviour
{
    public GameObject infobox;
    public Text objecttype;
    public Text sizeHabitable;
    public Text sizeHabitableData;
    public Text type;
    public Text affiliation;
    public Text objectName;
    public Text description;

    private Vector2 newPos;
    private RectTransform rectTransform;

    void Start(){
        rectTransform = infobox.GetComponent<RectTransform>();
        newPos = new Vector2(190, -125);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(rectTransform.anchoredPosition, newPos) > 0.001f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, newPos, Time.deltaTime * 4);

        }
        
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
        newPos = new Vector2(-190, -125);
        print(rectTransform.anchoredPosition);
    }

    public void UnloadInfobox()
    {
        newPos = new Vector2(190, rectTransform.anchoredPosition.y);
    }
    
}
