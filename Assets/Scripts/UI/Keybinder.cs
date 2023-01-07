using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keybinder : MonoBehaviour
{
    string currentBind;
    TextMeshProUGUI textComponent;
    bool waitingForInput;
    InputManagerScript inputManager;
    void Start()
    {
        textComponent = gameObject.GetComponent<TextMeshProUGUI>();
        currentBind = textComponent.text;
        inputManager = GameObject.Find("InputManager").GetComponent<InputManagerScript>();
    }

    public void rebind()
    {
        textComponent.text = ">" + currentBind + "<";
        waitingForInput = true;
    }

    void Update() {
        if(waitingForInput)
        {
            if(Input.anyKeyDown)
            {
                foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if(Input.GetKeyDown(keycode))
                    {
                        textComponent.text = keycode.ToString();
                        string binding = gameObject.transform.parent.name;
                        if(binding == "SHTunnels")
                        {
                            inputManager.SHTunnels = keycode;
                        }
                    }
                }
                waitingForInput = false;
            }
        }
    }
}
