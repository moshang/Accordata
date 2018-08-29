using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handleSiteClick : MonoBehaviour
{
    uiController uiCtrl;
    // Use this for initialization
    void Start()
    {
        if (uiCtrl == null)
            uiCtrl = GameObject.Find("UIController").GetComponent<uiController>();
    }

    public void showSelected()
    {
        uiCtrl.showSelectedSite(gameObject);
    }
}
