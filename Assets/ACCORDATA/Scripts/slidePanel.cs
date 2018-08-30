using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum slideType { upDown, leftRight };
public class slidePanel : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public GameObject panel;
    public slideType type;
    public float outPos;
    public float inPos;
    public float slideTime;

    Toggle toggle;

    private void OnEnable()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        // assuming the panel is on-screen at start
        Vector3 localPos = panel.transform.localPosition;
        if (type == slideType.upDown)
            inPos = localPos.y;
        else
            inPos = localPos.x;

        setPos();
    }

    void setPos() // no animation
    {
        Vector3 targetPos;
        targetPos = panel.transform.position;
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
        panel.transform.position = targetPos;
    }

    public void doSlide()
    {
        StopAllCoroutines();
        StartCoroutine(slide());
    }

    IEnumerator slide()
    {
        Vector3 targetPos;
        targetPos = panel.transform.localPosition;

        float startPos;
        float endPos;
        if (type == slideType.upDown)
            startPos = panel.transform.localPosition.y;
        else
            startPos = panel.transform.localPosition.x;

        if (toggle.isOn)
            endPos = inPos;
        else
            endPos = outPos;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / slideTime;
            Debug.Log(progress);
            float easedProgress = Easings.BounceEaseOut(progress);
            float progressPos = Mathf.Lerp(startPos, endPos, easedProgress);

            if (type == slideType.upDown)
                targetPos.y = progressPos;
            else
                targetPos.x = progressPos;

            panel.transform.localPosition = targetPos;

            yield return null;
        }
    }
}
