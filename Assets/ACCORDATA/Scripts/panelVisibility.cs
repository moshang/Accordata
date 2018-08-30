using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panelVisibility : MonoBehaviour
{

    [Header("-> ACCORDATA <-")]
    public GameObject panel;
    Toggle toggle;

    private void OnEnable()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        setVisibility();
    }

    public void setVisibility()
    {
        if (toggle.isOn)
            panel.SetActive(true);
        else
            panel.SetActive(false);
    }
}
