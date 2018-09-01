using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum slideType { upDown, leftRight };
public class slidePanel : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public RectTransform panel;
    public slideType type;
    public float outPos;
    public float inPos;
    public float slideTime;
 
    Toggle toggle;


    private void OnEnable()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        setPos();
    }

    void setPos() // no animation
    {
        Vector3 targetPos;
        targetPos = panel.anchoredPosition;
        if (toggle.isOn)
        {
            if (type == slideType.upDown)
                targetPos.y = inPos;
            else
                targetPos.x = inPos;
        }
        else
        {
            if (type == slideType.upDown)
                targetPos.y = outPos;
            else
                targetPos.x = outPos;
        }
        panel.anchoredPosition = targetPos;
    }

    public void doSlide()
    {
        StopAllCoroutines();
        StartCoroutine(slide());
    }

    IEnumerator slide()
    {
        Vector3 targetPos;
        targetPos = panel.anchoredPosition;

        float startPos;
        float endPos;
        if (type == slideType.upDown)
            startPos = panel.anchoredPosition.y;
        else
            startPos = panel.anchoredPosition.x;

        if (toggle.isOn)
            endPos = inPos;
        else
            endPos = outPos;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / slideTime;
            float easedProgress = Easings.BounceEaseOut(progress);
            float progressPos = Mathf.Lerp(startPos, endPos, easedProgress);
            if (type == slideType.upDown)
                targetPos.y = progressPos;
            else
                targetPos.x = progressPos;

            panel.anchoredPosition = targetPos;
            yield return null;
        }
    }
}
