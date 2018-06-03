// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;

namespace AudioHelm
{
    public class SequencerPositionUI
    {
        float leftPadding = 0.0f;
        float rightPadding = 0.0f;
        Color windowColor = new Color(0.4f, 0.4f, 0.4f);
        Color tickColor = new Color(1.0f, 0.6f, 0.2f);

        public SequencerPositionUI(float left, float right)
        {
            leftPadding = left;
            rightPadding = right;
        }

        public void DrawSequencerPosition(Rect rect, Sequencer sequencer, float startWindow, float endWindow)
        {
            Rect activeArea = new Rect(rect);
            activeArea.x += leftPadding;
            activeArea.width -= leftPadding + rightPadding;

            float loopPosition = sequencer.currentIndex;
            float relativePostition = sequencer.GetDivisionLength() * loopPosition / sequencer.length;
            float positionWidth = sequencer.GetDivisionLength() * activeArea.width / sequencer.length;
            positionWidth = Mathf.Max(2.0f, positionWidth);

            EditorGUI.DrawRect(activeArea, Color.gray);
            Rect position = new Rect(relativePostition * activeArea.width + activeArea.x,
                         activeArea.y, positionWidth, activeArea.height);

            float x = activeArea.width * startWindow + activeArea.x;
            float width = Mathf.Round(activeArea.width * (endWindow - startWindow));
            Rect window = new Rect(x, activeArea.y, width, activeArea.height);
            EditorGUI.DrawRect(window, windowColor);
            EditorGUI.DrawRect(new Rect(x, activeArea.y, 1, activeArea.height), Color.black);
            EditorGUI.DrawRect(new Rect(x + width, activeArea.y, 1, activeArea.height), Color.black);

            if (sequencer.isActiveAndEnabled && Application.isPlaying)
                EditorGUI.DrawRect(position, tickColor);
        }
    }
}
