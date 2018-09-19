using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccordataStyle;

public class quickStep : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app
        StyleNameEng = "QuickStep";
        StyleNameChTw = "QuickStep";
        ComposerNameEng = "MoShang";
        ComposerNameChTw = "莫尚";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 175;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 1; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
    }

    // Style Specific Variables

    public override void initStyle(int newSeqBar)
    {
        seq.setBPM(bpm);
        seqGen.scale = scale;
        newSeqAtBar = newSeqBar;

        // style specific initializations
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {
        if (barNum != newSeqAtBar)
            return;

        seq.clear();

        Debug.Log("Bar:" + barNum);
        Debug.Log("AqiVal:" + aqiVal);
        int vol = 0;

        if (tempVal > 0)
            seq.setBPM((int)utils.map(tempVal, 0, 40, 80, 180));

        for (int i = 0; i < seqLength; i++)
        {
            if (uiCtrl.isActiveAqi) // check if the value is active or muted in the ui
            {
                // value range 0 - 250
                //seq.addNote(0, 0, 0);
                if (i % 4 == 0)
                    vol = Random.Range(90, 100);
                else
                    vol = 85 - i;
                seq.addNote(i, 60 + (i * ((aqiVal / 50) + 1)), vol);
            }
            if (uiCtrl.isActiveTemp) // check if the value is active or muted in the ui
            {
                // value range 0 - 40
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveWind) // check if the value is active or muted in the ui
            {
                // value range 0 - 10
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveHumidity) // check if the value is active or muted in the ui
            {
                // value range  0 - 100
                //seq.addNote(0, 0, 0);
            }
            if (uiCtrl.isActiveRain) // check if the value is active or muted in the ui
            {
                // value range 0 - 60
                //seq.addNote(0, 0, 0);
            }
            //}
        }

        newSeqAtBar += newSeqEveryXBars;
    }
}
