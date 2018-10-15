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

namespace AccordataStyle
{
    /*
     * -> ACCORDATA <-
     * 
     * The parent class that all styles derive from
     */
    public class Style : MonoBehaviour
    {
        [Header("DEBUG ONLY - assign values in script ", order = 0)]
        [Space(-10, order = 1)]
        [Header("Use gear icon and 'Reset' after changes", order = 2)]
        [Space(-10, order = 3)]
        [Header("", order = 4)]
        // Info about the style to display in the app
        public string StyleNameEng;
        public string StyleNameChTw;
        public string ComposerNameEng;
        public string ComposerNameChTw;
        public string StyleInfoEng;
        public string StyleInfoChTw;
        // Style settings
        public int bpm;
        public Scale scale;
        public int seqLength = 16; // the length of a single bar of the sequence in 16ths
        public int newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        public int newSeqEveryXBars;    /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
        public seqGenerator seqGen;
        public uiController uiCtrl;
        public int newSeqAtBar = 0;
        private AudioReverbFilter reverb;
        private AudioEchoFilter delay;

        public bool useReverb = true;
        public AudioReverbPreset revrbPreset = AudioReverbPreset.Arena;
        public bool useDelay = false;
        public int delayTimeMS = 500;
        public float delayDecay = 0.5f;
        public float delayMix = 0.8f;


        private void OnEnable()
        {
            seqGen = GetComponentInParent<seqGenerator>();
            uiCtrl = seqGen.uiCtrl;
            GameObject accoPlayer = GameObject.Find("AccoPlayer");
            reverb = accoPlayer.GetComponent<AudioReverbFilter>();
            delay = accoPlayer.GetComponent<AudioEchoFilter>();
        }

        public virtual void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
        {
        }

        public virtual void initStyle(int newSeqBar)
        {
        }

        public virtual void doEveryBeat()
        {
        }

        public void setVol(string param, float newVal, float time)
        {
            StartCoroutine(adjustVol(param, newVal, time));
        }

        IEnumerator adjustVol(string param, float newVal, float time)
        {
            float startTime = Time.time;
            float endTime = startTime + time;
            float oldVal;
            seqGen.mixer.GetFloat(param, out oldVal);
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / time;
                float currentVal = Mathf.Lerp(oldVal, newVal, progress);
                seqGen.mixer.SetFloat(param, currentVal);
                yield return null;
            }
            seqGen.mixer.SetFloat(param, newVal);
        }

        public void resetSFX()
        {
            if (useReverb)
            {
                reverb.enabled = true;
                reverb.reverbPreset = revrbPreset;
            }
            else
                reverb.enabled = false;

            if (useDelay)
            {
                delay.enabled = true;
                delay.delay = delayTimeMS;
                delay.wetMix = delayMix;
                delay.decayRatio = delayDecay;
            }
            else
                delay.enabled = false;

            // temperature
            setVol("tempLightVol", -80, 1);
            setVol("tempMediumVol", -80, 1);
            setVol("tempHeavyVol", -80, 1);
            // wind
            setVol("windLightVol", -80, 1);
            setVol("windMediumVol", -80, 1);
            setVol("windHeavyVol", -80, 1);
            // humidity
            setVol("humLightVol", -80, 1);
            setVol("humMediumVol", -80, 1);
            setVol("humHeavyVol", -80, 1);
            // rain
            setVol("rainLightVol", -80, 1);
            setVol("rainMediumVol", -80, 1);
            setVol("rainHeavyVol", -80, 1);

            seqGen.resetSfxClips();
        }

        public void killCoroutines()
        {
            StopAllCoroutines();
        }
    }
}
