using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class toggleImage : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public Sprite deactivated;
    public Sprite activated;
    public Image image;
    Toggle toggle;
    private void OnEnable()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        setImage();
    }

    public void setImage()
    {
        if (toggle.isOn)
            image.sprite = activated;
        else
            image.sprite = deactivated;
    }
}
