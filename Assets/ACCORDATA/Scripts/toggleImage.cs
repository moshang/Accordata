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
    [Header("Alternatively assign images on gameobjects")]
    public GameObject deactivatedImageGO;
    public GameObject activatedImageGO;
    Toggle toggle;
    public Color interactableColor;
    public Color nonInteractableColor;
    private void OnEnable()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        setImage();
    }

    public void setImage()
    {
        if (image != null)
        {
            if (toggle.isOn)
                image.sprite = activated;
            else
                image.sprite = deactivated;
        }
        else
        {
            if (toggle.isOn)
            {
                if (activatedImageGO != null)
                    activatedImageGO.SetActive(true);
                if (deactivatedImageGO != null)
                    deactivatedImageGO.SetActive(false);
            }
            else
            {
                if (activatedImageGO != null)
                    activatedImageGO.SetActive(false);
                if (deactivatedImageGO != null)
                    deactivatedImageGO.SetActive(true);
            }
        }
    }
}
