using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoboxScript : MonoBehaviour
{
    public GameObject infobox;
    public TextMeshProUGUI logo; 
    public TextMeshProUGUI objecttype;
    public TextMeshProUGUI sizeHabitable;
    public TextMeshProUGUI sizeHabitableData;
    public TextMeshProUGUI type;
    public TextMeshProUGUI affiliation;
    public TextMeshProUGUI affiliationData;
    public TextMeshProUGUI objectName;
    public TextMeshProUGUI description;

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
            logo.text = "\ue805";
            objecttype.text = "STAR SYSTEM";
            sizeHabitable.text = "SIZE : ";
            sizeHabitableData.text = " " + systemInfos.size + " AU";
            type.text = systemInfos.type;
            affiliationData.text = " " + systemInfos.affiliationName.ToUpper();
            objectName.text = systemInfos.systemName.ToUpper();
            description.text = systemInfos.description;
        }
        else
        {
            COInfosScript coInfosScript = gameObject.GetComponent<COInfosScript>();
            objecttype.text = coInfosScript.type;
            if(coInfosScript.type == "STAR")
            {
                logo.text = "\ue804";
            }
            else if(coInfosScript.type == "PLANET" || coInfosScript.type == "SATELLITE")
            {
                logo.text = "\ue803";
            }
            if (coInfosScript.type == "JUMPPOINT") 
            {
                logo.text = "\ue801";
                sizeHabitable.text = "HABITABLE : ";
                sizeHabitableData.text = " NO";

                affiliation.text = "SIZE :";
                affiliationData.text = " " + coInfosScript.size;
                type.text = "BIDIRECTIONAL";
            }
            else 
            {
                sizeHabitable.text = "HABITABLE : ";
                if(coInfosScript.habitable.Equals(""))
                {
                    sizeHabitableData.text = " NO";
                }
                else
                {
                    sizeHabitableData.text = " YES";
                }
                type.text = coInfosScript.subtype.ToUpper();
                affiliationData.text = " " + coInfosScript.affiliationName.ToUpper();
            }

            if(coInfosScript.coName == "") objectName.text = coInfosScript.designation.ToUpper();
            else objectName.text = coInfosScript.coName.ToUpper();

            description.text = coInfosScript.description;
        }
        newPos = new Vector2(-190, rectTransform.anchoredPosition.y);
        StartCoroutine("UpdateUI"); //without this, the UI is not placed well 
    
    }

    public void UnloadInfobox()
    {
        newPos = new Vector2(190, rectTransform.anchoredPosition.y);
    }
    
    public IEnumerator UpdateUI()
    {
        yield return null;
        description.text += " ";
    }
}
