using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccordataStyle;

public class styleTemplate : Style
{
    // -> ACCORDATA <-
    private void Reset() // override the values in the parent class
    {
        // Info about the style to display in the app
        StyleNameEng = "";
        StyleNameChTw = "";
        ComposerNameEng = "";
        ComposerNameChTw = "";
        StyleInfoEng = "";
        StyleInfoChTw = "";
        // Style settings
        bpm = 90;
        scale = Scale.pentatonic;
        seqLength = 16; // the length of a single bar of the sequence in 16ths
        newValuesEveryXBars = 2; // switch to the next set of values every x bars (either values from the next site, or the next hour for the same site)
        newSeqEveryXBars = 1;           /*  how often should we generate a new sequence?
                                            this value needs to be smaller than the newValuesEveryXBars value, or we will skip over new data
                                            set to 1 to regenerate the sequence with the current data every bar, even if the data hasn't changed
                                            set to more than 1 to repeat the previously generated bar */
    }

    // Style Specific Variables

    public override void initStyle()
    {
        seq.setBPM(bpm);
        seqGen.scale = scale;

        // style specific initializations
    }

    public override void makeSeq(int barNum, int aqiVal, float tempVal, float windVal, float humidityVal, float rainVal)
    {

        // schedule the next bar at which to create a new sequence
        newSeqAtBar += newSeqEveryXBars;
    }
}
