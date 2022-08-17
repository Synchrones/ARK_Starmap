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

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        if(setalphathreshold)gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
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
        switchState();
    }

    public void showTunnelsM()
    {
        switchState();
    }

    public void showTunnelsL()
    {
        switchState();
    }

    private void switchState()
    {
        if(activated)
        {
            activated = false;
        }
        else
        {
            activated = true;
        }
    }
}
