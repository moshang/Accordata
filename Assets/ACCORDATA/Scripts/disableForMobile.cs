using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableForMobile : MonoBehaviour
{
    // -> ACCORDATA <-
    private void OnEnable()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            gameObject.SetActive(false);
        }
    }
}
