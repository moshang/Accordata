﻿using System.Collections;
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
        [Header("Debug Only - better to assign in script")]
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
        // Debug
        public seqGenerator seqGen;
        public int newSeqAtBar = 0;

        private void OnEnable()
        {
            seqGen = GetComponentInParent<seqGenerator>();
        }

        public virtual void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
        {
        }
    }
}
