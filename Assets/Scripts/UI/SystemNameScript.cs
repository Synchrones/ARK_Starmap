using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemNameScript : MonoBehaviour
{
    public Text systemName;

    public void ChangeName(string name)
    {
        systemName.text = name;
    }
}
