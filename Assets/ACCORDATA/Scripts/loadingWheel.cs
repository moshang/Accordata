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

public class loadingWheel : MonoBehaviour
{
    public Image[] spokes;
    private int startSpoke = 12;
    private Color32 startColor = new Color32(255, 255, 255, 255);

    private void OnEnable()
    {
        //InvokeRepeating("updateColors", 0, 0.083f);
        StartCoroutine(updateColors());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator updateColors()
    {
        while (true)
        {
            startSpoke = (startSpoke - 1 >= 0) ? startSpoke - 1 : 11;
            for (int i = 0; i < 12; i++)
            {
                Color32 newColor = startColor;
                newColor.a = (byte)(255 - (i * 15));
                spokes[(startSpoke + i) % 12].color = newColor;
            }
            yield return new WaitForSeconds(0.083f);
        }
    }
}
