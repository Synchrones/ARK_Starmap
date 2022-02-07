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
    public TextMeshProUGUI systemType;
    public TextMeshProUGUI systemSize;
    public TextMeshProUGUI affiliation;
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
            Disc.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
            StartCoroutine(DiscFadeIn());
            Disc.gameObject.SetActive(true);
            SystemsInfosScript systemInfos = selectedObject.GetComponent<SystemsInfosScript>();
            systemName.text = systemInfos.code;
            systemType.text = systemInfos.type;
            systemSize.text = systemInfos.size + " AU";
            affiliation.text = systemInfos.affiliationName;
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

    public IEnumerator DiscFadeIn()
    {
        for(int i = 0; i < 70; i++)
        {
            Disc.GetComponent<RectTransform>().sizeDelta = new Vector2(i * 10, i * 10);
            yield return null;
        }
        for(float i = 0; i < 1; i += 0.01f)
        {
            Disc.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
    }
}
