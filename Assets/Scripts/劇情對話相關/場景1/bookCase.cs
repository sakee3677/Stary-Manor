using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class bookCase : MonoBehaviour, IUsable
{
    public bool active=false;
    public GameObject readpanel;
    public void UseObject()
    {
        if (!active)
        {
            readpanel.SetActive(true);
            active = true;
        }
        else
        {
            readpanel.SetActive(false);
            active = false;
        }


    }
    public void UnUseObject()
    {
        readpanel.SetActive(false);
    }
}
