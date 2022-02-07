using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    public GameObject scriptHandler;
    public GameObject UIContainer;
    //disc
    private int currentState = 0;
    private Vector2 pos1 = new Vector2(153, 23.4f);
    private Vector2 pos2= new Vector2(163, 0.2f);
    private Vector2 pos3= new Vector2(153, -28);
    public GameObject inspectButton;
    public GameObject informationsButton;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterSystem()
    {
        scriptHandler.GetComponent<StarSystemsScript>().LoadAndEnterSystem();
    }

    public void LoadInfobox()
    {
        StartCoroutine(moveButton(informationsButton, pos2, true));
        StartCoroutine(moveButton(inspectButton, pos1, false));
    }

    IEnumerator moveButton(GameObject button, Vector2 pos, bool executeAction)
    {
        Vector2 basePos = button.GetComponent<RectTransform>().anchoredPosition;
        for(float i = 0; i < 1; i += 0.02f)
        {
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(basePos, pos, i);
            yield return null;
        }
        if(executeAction)
        {
            if(button.gameObject.name == "Informations")UIContainer.GetComponent<DiscScript>().LoadInfobox();
        }
    }
}
