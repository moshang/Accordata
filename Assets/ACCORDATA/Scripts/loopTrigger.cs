﻿/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
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

public class loopTrigger : MonoBehaviour
{
    AudioSource audio;
    public AudioClip[] clips;
    int currentClip;
    // Use this for initialization
    void Start()
    {
        clock.OnBar += everyBar;
        audio = GetComponent<AudioSource>();
    }

    void everyBar(int barNum)
    {
        if (barNum % 4 == 0)
        {
            double time = AudioSettings.dspTime;
            double triggerTime = time + 8; //audio.clip.length;
            audio.PlayScheduled(triggerTime);
            //audio.PlayOneShot(clips[0], 0.75f);
            //currentClip++;
        }
    }
}
