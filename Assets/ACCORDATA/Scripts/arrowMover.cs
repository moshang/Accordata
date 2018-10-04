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

public class arrowMover : MonoBehaviour
{
    float moveTime = 2;
    public float startTime;
    public float endTime;
    Vector3 currentPosition;
    float startY = -115;
    float endY = -170;
    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        endTime = startTime + moveTime;
        currentPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time < startTime + (moveTime / 2))
        {
            float progress = (Time.time - startTime) / (moveTime / 2);
            progress = Easings.CubicEaseInOut(progress);
            float newY = Mathf.Lerp(startY, endY, progress);
            currentPosition.y = newY;
            transform.localPosition = currentPosition;
            }
        else if (Time.time < endTime)
        {
            float progress = (Time.time - (startTime + (moveTime / 2))) / (moveTime / 2);
            progress = Easings.CubicEaseInOut(progress);
            float newY = Mathf.Lerp(endY, startY, progress);
            currentPosition.y = newY;
            transform.localPosition = currentPosition;
        }
        else
        {
            startTime = Time.time;
            endTime = startTime + moveTime;
        }
    }
}
