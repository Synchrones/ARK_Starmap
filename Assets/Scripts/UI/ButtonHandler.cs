using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    public GameObject scriptHandler;
    public GameObject UIContainer;
    AudioManagerScript audioManager;
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

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        if(animated)
        {
            if(setalphathreshold)gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
            currentStateColor = gameObject.GetComponent<Image>().color;
        }
        
    }

    public void EnterSystem()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos2, true));
        StartCoroutine(moveButton(informationsButton, pos3, false));
    }

    public void LoadInfobox()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos1, false));
        StartCoroutine(moveButton(informationsButton, pos2, true));
    }

    public void setInitialPos()
    {
        StartCoroutine(moveButton(inspectButton, pos2, false));
        StartCoroutine(moveButton(informationsButton, pos3, false));
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
    }

    public void backToCurrentColor()
    {
        gameObject.GetComponent<Image>().color = currentStateColor;
    }
}
