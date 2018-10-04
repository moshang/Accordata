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
public class inputController : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public RectTransform mapPanel;
    private Vector2 mapScale;
    private Vector2 mapStartPos;
    private Vector2 mouseDownPos;
    public ScrollRect scrollRct;
    private float zoomSpeed = 0.005f;

    private float last2Touch; // fixing a jitter issue by setting a coolof period where scroll rect is disabled

    // Use this for initialization
    void Start()
    {
        mapScale = new Vector2(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float nowTime = Time.time;

        // Scroll-wheel zoom
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            // scroll up
            float scaleFactor = mapScale.x;
            scaleFactor = Mathf.Clamp(scaleFactor + 0.1f, 0.5f, 10);
            mapScale.x = scaleFactor;
            mapScale.y = scaleFactor;
            mapPanel.localScale = mapScale;
        }
        else if (d < 0f)
        {
            // scroll down
            float scaleFactor = mapScale.x;
            scaleFactor = Mathf.Clamp(scaleFactor - 0.1f, 0.5f, 10);
            mapScale.x = scaleFactor;
            mapScale.y = scaleFactor;
            mapPanel.localScale = mapScale;
        }

        // Pinch to zoom
        if (Input.touchCount == 2)
        {
            last2Touch = nowTime;
            scrollRct.enabled = false;
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the canvas size based on the change in distance between the touches.
            float scaleFactor = mapScale.x;
            scaleFactor = Mathf.Clamp(scaleFactor - (deltaMagnitudeDiff * zoomSpeed), 0.5f, 10);
            mapScale.x = scaleFactor;
            mapScale.y = scaleFactor;
            mapPanel.localScale = mapScale;
            // Make sure the canvas size never drops below 0.1
            //canvas.scaleFactor = Mathf.Max(canvas.scaleFactor, 0.1f);
        }

        if (nowTime > (last2Touch + 0.2f)) // set a cooloff of 0.2s before switching the scroll rect back on
            scrollRct.enabled = true;
    }
}
