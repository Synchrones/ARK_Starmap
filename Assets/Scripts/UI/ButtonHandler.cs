using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    public GameObject scriptHandler;
    public GameObject UIContainer;
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
        UIContainer.GetComponent<DiscScript>().LoadInfobox();
    }

}
