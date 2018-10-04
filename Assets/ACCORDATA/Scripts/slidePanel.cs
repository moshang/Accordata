/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum slideType { upDown, leftRight };
public class slidePanel : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public RectTransform panel;
    public RectTransform altPanel;
    public slideType type;
    public float outPos;
    public float inPos;
    public float slideTime;
    [Header("Site Toggle Specific")]
    public bool isSiteToggle;
    public GameObject siteDetails;
    public GameObject sitesList;
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
        if (isSiteToggle)
        {
            if (uiController.currentMode == Mode.site72Hr)
            {
                sitesList.SetActive(true);
                siteDetails.SetActive(false);
            }
            else
            {
                sitesList.SetActive(false);
                siteDetails.SetActive(true);
            }
        }
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
