using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManagerScript : MonoBehaviour
{
    public GameObject keybindingsTab;
    public KeyCode SHTunnels;

    void Start() 
    {
        if(!PlayerPrefs.HasKey("KCODE_SHTunnels"))
        {
            PlayerPrefs.SetInt("KCODE_SHTunnels", (int)KeyCode.J);
        }
        
        SHTunnels = (KeyCode)PlayerPrefs.GetInt("KCODE_SHTunnels");


        keybindingsTab.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = ((KeyCode)SHTunnels).ToString();
    }
}
