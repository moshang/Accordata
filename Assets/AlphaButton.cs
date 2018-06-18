using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

// see https://answers.unity.com/questions/821613/unity-46-is-it-possible-for-ui-buttons-to-be-non-r.html
public class AlphaButton : MonoBehaviour
{
    public float AlphaThreshold = 0.1f;

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}