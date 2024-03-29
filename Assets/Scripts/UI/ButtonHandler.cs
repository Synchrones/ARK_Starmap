using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class ButtonHandler : MonoBehaviour
{
    public GameObject scriptHandler;
    public GameObject UIContainer;
    AudioManagerScript audioManager;
    public GameObject SceneSwitcher;
    //disc
    private Vector2 pos1 = new Vector2(153, 23.4f);
    private Vector2 pos2= new Vector2(163, 0.2f);
    private Vector2 pos3= new Vector2(158, -24.5f);
    public GameObject inspectButton;
    public GameObject informationsButton;
    public bool setalphathreshold;
    public bool activated;
    public bool animated;  
    public Color currentStateColor;
    public bool discButtonHandler;
    public GameObject linkedTab;
    GameObject optionsGO;
    public string URL;
    public GameObject spacebox;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        optionsGO = GameObject.Find("OptionsTab");
        if(animated)
        {
            if(setalphathreshold)gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
            currentStateColor = gameObject.GetComponent<Image>().color;
        }
        if(discButtonHandler)
        {
            setInitialPos();
        }
    }

    public void EnterSystem()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos2, true));
        StartCoroutine(moveButton(informationsButton, pos3, false));
        inspectButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        informationsButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void LoadInfobox()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos1, false));
        StartCoroutine(moveButton(informationsButton, pos2, true));
        inspectButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        informationsButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void setInitialPos()
    {
        StartCoroutine(moveButton(inspectButton, pos2, false));
        StartCoroutine(moveButton(informationsButton, pos3, false));
        inspectButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        informationsButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator moveButton(GameObject button, Vector2 pos, bool executeAction)
    {
        Vector2 basePos = button.GetComponent<RectTransform>().anchoredPosition;
        for(float i = 0; i < 1; i += 5 * Time.deltaTime)
        {
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(basePos, pos, i);
            yield return null;
        }
        if(executeAction)
        {
            DiscScript discScript = UIContainer.GetComponent<DiscScript>();
            if(button.gameObject.name == "Informations")
            {
                if(discScript.isInfoboxActive)
                {
                    discScript.UnloadInfobox();
                }
                else discScript.LoadInfobox();
                
            }
            if(button.gameObject.name == "Inspect")
            {
                if(discScript.mode == 0) scriptHandler.GetComponent<StarSystemsScript>().LoadAndEnterSystem();
            }        
        }
    }

    public void showTunnelsS()
    {
        switchState("JP", "S");
    }

    public void showTunnelsM()
    {
        switchState("JP", "M");
    }

    public void showTunnelsL()
    {
        switchState("JP", "L");
    }
    public void showSystemsUNC()
    {
        switchState("Systems", "UNC");
    }

    public void showSystemsDEV()
    {
        switchState("Systems", "DEV");
    }

    public void showSystemsXIAN()
    {
        switchState("Systems", "XIAN");
    }

    public void showSystemsVNCL()
    {
        switchState("Systems", "VNCL");
    }

    public void showSystemsBANU()
    {
        switchState("Systems", "BANU");
    }

    public void showSystemsUEE()
    {
        switchState("Systems", "UEE");
    }

    private void switchState(string parameter, string value)
    {
        if(activated)
        {
            activated = false;
            audioManager.play("ButtonDesactivate");
            currentStateColor = new Color(currentStateColor.r, currentStateColor.g, currentStateColor.b, 0.1f);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);

            if(parameter == "JP")
            {
                List<GameObject> tunnels = (List<GameObject>)scriptHandler.GetComponent<StarSystemsScript>().GetType().GetField("tunnels" + value).GetValue(scriptHandler.GetComponent<StarSystemsScript>());
                foreach(GameObject tunnel in tunnels)
                {
                    tunnel.SetActive(false);
                }
            }
            else if(parameter == "Systems")
            {
                List<GameObject> systems = (List<GameObject>)scriptHandler.GetComponent<StarSystemsScript>().GetType().GetField("systems" + value).GetValue(scriptHandler.GetComponent<StarSystemsScript>());
                foreach(GameObject system in systems)
                {
                    system.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
                    system.GetComponent<SystemsInfosScript>().lockOpacity = true;
                }
            }
        }
        else
        {
            activated = true;
            audioManager.play("ButtonActivate");
            currentStateColor = new Color(currentStateColor.r, currentStateColor.g, currentStateColor.b, 0.6f);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);

            if(parameter == "JP")
            {
                List<GameObject> tunnels = (List<GameObject>)scriptHandler.GetComponent<StarSystemsScript>().GetType().GetField("tunnels" + value).GetValue(scriptHandler.GetComponent<StarSystemsScript>());
                foreach(GameObject tunnel in tunnels)
                {
                    tunnel.SetActive(true);
                }
            }
            else if(parameter == "Systems")
            {
                List<GameObject> systems = (List<GameObject>)scriptHandler.GetComponent<StarSystemsScript>().GetType().GetField("systems" + value).GetValue(scriptHandler.GetComponent<StarSystemsScript>());
                foreach(GameObject system in systems)
                {
                    system.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    system.GetComponent<SystemsInfosScript>().lockOpacity = false;
                }
            }
        }
    }
    public void hovered()
    {
        gameObject.GetComponent<Image>().color = new Color(currentStateColor.r, currentStateColor.g, currentStateColor.b, 1);
        if(gameObject.transform.parent.name == "JumpPoint")
        {
            gameObject.transform.parent.GetChild(4).GetComponent<TextMeshProUGUI>().text = "/ " + gameObject.name.ToUpper();
        }
        else if(gameObject.transform.parent.name == "Factions")
        {
            gameObject.transform.parent.GetChild(7).GetComponent<TextMeshProUGUI>().text = "/ " + gameObject.name.ToUpper();
        }
        
    }

    public void backToCurrentColor()
    {
        gameObject.GetComponent<Image>().color = currentStateColor;
        
        if(gameObject.transform.parent.name == "JumpPoint")
        {
            gameObject.transform.parent.GetChild(4).GetComponent<TextMeshProUGUI>().text = "";
        }
        else if(gameObject.transform.parent.name == "Factions")
        {
            gameObject.transform.parent.GetChild(7).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void goBack()
    {
        StarSystemsScript starSystemsScript = scriptHandler.GetComponent<StarSystemsScript>();
        if(starSystemsScript.COSelected)
        {
            starSystemsScript.UnselectCO();
        }
        else
        {
            starSystemsScript.UnloadAndExitSystem();
        }
        
    }

    public void loadARK()
    {
        SceneSwitcher.GetComponent<SceneSwitcherScript>().loadARK();
        //SceneManager.LoadScene("Main");
    }

    public void selectOptionTab()
    {
        audioManager.play("SimpleClick");
        for (int i = 0; i < linkedTab.transform.parent.childCount; i++)
        {
            linkedTab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        linkedTab.SetActive(true);
    }

    public void openOptions()
    {
        StartCoroutine(optionsRescale(1));
        audioManager.play("ButtonActivate");
    }

    public void closeOptions()
    {
        StartCoroutine(optionsRescale(0));
        audioManager.play("ButtonDesactivate");
    }

    IEnumerator optionsRescale(int targetScale)
    {
        for (float i = 0; i < 1; i += Time.deltaTime * 5)
        {
            optionsGO.transform.localScale = new Vector3(Mathf.Lerp(1 - targetScale, targetScale, i * i * (3 - 2 * i)), 1, 1);
            yield return null;
        }
        optionsGO.transform.localScale = new Vector3(targetScale, 1, 1);
    }

    public void openLink()
    {
        Application.OpenURL(URL);
    }

    public void activateDesactivateSpaceBox()
    {
        audioManager.play("SimpleClick");
        if(SceneManager.GetActiveScene().name != "Starting screen")
        {
            if(gameObject.GetComponent<Toggle>().isOn)
            {
                spacebox.SetActive(false);
            }
            else
            {
                spacebox.SetActive(true);
            }
        }
    }
}