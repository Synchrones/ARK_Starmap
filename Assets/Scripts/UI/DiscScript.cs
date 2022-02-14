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
    AudioManagerScript audioManager;
    public int mode;
    public TextMeshProUGUI objectName;
    public TextMeshProUGUI objectType;
    public TextMeshProUGUI objectSubtypeText;
    public TextMeshProUGUI objectSubtypeData;
    public TextMeshProUGUI sizeHabitableText;
    public TextMeshProUGUI sizeHabitableData;
    public TextMeshProUGUI affiliation;

    public GameObject edgeCircle;
    public GameObject populationCircle;
    public GameObject economyCircle;
    public GameObject threatCircle;
    // Update is called once per frame
    void Start()
    {
        Disc.gameObject.SetActive(false);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }
    void Update()
    {
        if(isActive == true)
        {
            Disc.transform.position = Camera.main.WorldToScreenPoint(selectedObject.transform.position);

            if(Input.GetKey(KeyCode.Escape))
            {
                UnloadDisc();
                this.GetComponent<InfoboxScript>().UnloadInfobox();
            }
        }
    }

    public void LoadDisc()
    {
        if(mode == 0)
        {
            isActive = true;
            audioManager.play("OpenDisc");
            Disc.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
            Disc.gameObject.SetActive(true);
            SystemsInfosScript systemInfos = selectedObject.GetComponent<SystemsInfosScript>();
            objectName.text = systemInfos.code;
            objectType.text = "STAR SYSTEM";
            objectSubtypeText.text = "SYSTEM TYPE";
            objectSubtypeData.text = systemInfos.type;
            sizeHabitableText.text = "SIZE";
            sizeHabitableData.text = systemInfos.size + " AU";
            affiliation.text = systemInfos.affiliationName;
            
            //return the values in degree that the disc circles circonferences must be (need multiple of 6 for the circle to be cut right)
            int population = (int)Mathf.Round(Mathf.Round(float.Parse(systemInfos.population, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;
            int economy = (int)Mathf.Round(Mathf.Round(float.Parse(systemInfos.economy, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;
            int threat = (int)Mathf.Round(Mathf.Round(float.Parse(systemInfos.danger, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;

            StartCoroutine(DiscFadeIn(population, economy, threat));
        }
        else
        {
            isActive = true;
            Disc.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
            Disc.gameObject.SetActive(true);
            COInfosScript COInfos = selectedObject.GetComponent<COInfosScript>();
            objectName.text = COInfos.coName;
            if (objectName.text == "")objectName.text = COInfos.designation;
            objectType.text = COInfos.type;
            objectSubtypeText.text = "TYPE";
            objectSubtypeData.text = COInfos.subtype;
            sizeHabitableText.text = "HABITABLE";
            if(COInfos.habitable == "1")sizeHabitableData.text = "YES";
            else sizeHabitableData.text = "NO";
            affiliation.text = COInfos.affiliationName;

            //return the values in degree that the disc circles circonferences must be (need multiple of 6 for the circle to be cut right)
            int population = (int)Mathf.Round(Mathf.Round(float.Parse(COInfos.population, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;
            int economy = (int)Mathf.Round(Mathf.Round(float.Parse(COInfos.economy, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;
            int threat = (int)Mathf.Round(Mathf.Round(float.Parse(COInfos.danger, System.Globalization.CultureInfo.InvariantCulture) * 36) / 6) * 6;

            StartCoroutine(DiscFadeIn(population, economy, threat));
        }
    }

    public void UnloadDisc()
    {
        isActive = false;
        audioManager.play("CloseDisc");
        StartCoroutine(DiscFadeOut());
    }

    public void LoadInfobox()
    {
        this.GetComponent<InfoboxScript>().LoadInfobox(selectedObject, mode);
    }
    public void UnloadInfobox()
    {
        this.GetComponent<InfoboxScript>().UnloadInfobox();
    }

    public IEnumerator DiscFadeIn(int population, int economy, int threat)
    {
        for(int i = 0; i <= 70; i++)
        {
            Disc.GetComponent<RectTransform>().sizeDelta = new Vector2(i * 10, i * 10);
            populationCircle.GetComponent<UICircleRendererScript>().drawCircle(65, 4, i * population / 70);
            economyCircle.GetComponent<UICircleRendererScript>().drawCircle(54, 4.5f, i * economy / 70);
            threatCircle.GetComponent<UICircleRendererScript>().drawCircle(43, 4, i * threat / 70);
            yield return null;
        }
        for(float i = 0; i < 1; i += 0.01f)
        {
            Disc.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        
    }

    public IEnumerator DiscFadeOut()
    {
        for(int i = 70; i > 0; i-=2)
        {
            Disc.GetComponent<RectTransform>().sizeDelta = new Vector2(i * 10, i * 10);
            yield return null;
        }
        Disc.gameObject.SetActive(false);
        this.GetComponent<ButtonHandler>().setInitialPos();
    }

}
