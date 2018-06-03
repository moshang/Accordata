// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AudioHelm
{
    public class SequencerVelocityUI
    {
        const float velocityMeterWidth = 3.0f;
        const float velocityHandleWidth = 7.0f;
        const float velocityHandleGrabWidth = 13.0f;

        Note currentNote;
        SerializedProperty currentNoteSerialized = null;

        float leftPadding = 0.0f;
        float rightPadding = 0.0f;
        Color activeAreaColor = new Color(0.6f, 0.6f, 0.6f);
        Color background = new Color(0.5f, 0.5f, 0.5f);
        Color velocityColor = new Color(1.0f, 0.3f, 0.3f);
        Color velocityActiveColor = new Color(0.6f, 1.0f, 1.0f);

        float sixteenthWidth = 1.0f;
        float height = 1.0f;
        float startTime = 0.0f;
        float endTime = 1.0f;

        public SequencerVelocityUI(float left, float right)
        {
            leftPadding = left;
            rightPadding = right;
        }

        public bool MouseActive()
        {
            return currentNote != null;
        }

        void MouseUp()
        {
            currentNote = null;
            currentNoteSerialized = null;
        }

        void MouseDown(Rect rect, Sequencer sequencer, Vector2 mousePosition, SerializedProperty allNotes)
        {
            currentNote = null;
            float closest = 2.0f  * velocityHandleGrabWidth;
            float mouseX = mousePosition.x - rect.x - leftPadding;
            float mouseY = mousePosition.y - rect.y;

            List<Note> noteOns = sequencer.GetAllNoteOnsInRange(startTime, endTime);
            foreach (Note note in noteOns)
            {               
                float x = sixteenthWidth * (note.start - startTime);
                float yInv = note.velocity * (rect.height - velocityHandleWidth) + velocityHandleWidth / 2.0f;
                float y = rect.height - yInv;
                float xDiff = Mathf.Abs(x - mouseX);
                float yDiff = Mathf.Abs(y - mouseY);
                float diffTotal = xDiff + yDiff;

                if (xDiff <= velocityHandleGrabWidth && yDiff <= velocityHandleGrabWidth && diffTotal < closest)
                {
                    closest = diffTotal;
                    currentNote = note;

                    SerializedProperty serializedNoteRow = allNotes.GetArrayElementAtIndex(currentNote.note);
                    SerializedProperty serializedNoteList = serializedNoteRow.FindPropertyRelative("notes");
                    List<Note> noteList = sequencer.allNotes[currentNote.note].notes;
                    int index = noteList.IndexOf(currentNote);
                    if (index >= 0)
                        currentNoteSerialized = serializedNoteList.GetArrayElementAtIndex(index);
                }
            }

            if (currentNote != null)
                Undo.RegisterCompleteObjectUndo(sequencer, "Set Note Velocity");
        }

        void MouseDrag(float velocity, SerializedProperty allNotes)
        {
            if (currentNote == null)
                return;

            currentNote.velocity = velocity;
            currentNoteSerialized.FindPropertyRelative("velocity_").floatValue = velocity;
        }

        public bool DoVelocityEvents(Rect rect, Sequencer sequencer, SerializedProperty allNotes)
        {
            Event evt = Event.current;

            float velocityMovementHeight = rect.height - velocityHandleWidth;
            float minVelocityY = rect.y + velocityHandleWidth;
            float velocity = 1.0f - (evt.mousePosition.y - minVelocityY) / velocityMovementHeight;
            velocity = Mathf.Clamp(velocity, 0.001f, 1.0f);

            if (evt.type == EventType.MouseUp && MouseActive())
            {
                MouseUp();
                return true;
            }

            if (evt.type == EventType.MouseDown)
            {
                MouseDown(rect, sequencer, evt.mousePosition, allNotes);
                MouseDrag(velocity, allNotes);
            }
            else if (evt.type == EventType.MouseDrag && MouseActive())
                MouseDrag(velocity, allNotes);
            return true;
        }

        void DrawNote(float x, float velocity, Color color)
        {
            float h = velocity * (height - velocityHandleWidth) + velocityHandleWidth / 2.0f;
            float y = height - h;

            EditorGUI.DrawRect(new Rect(x - velocityMeterWidth / 2.0f, y, velocityMeterWidth, h), color);
            EditorGUI.DrawRect(new Rect(x - velocityHandleWidth / 2.0f, y - velocityHandleWidth / 2.0f,
                                        velocityHandleWidth, velocityHandleWidth), color);
        }

        void DrawNoteVelocities(Sequencer sequencer, float start, float end, float width)
        {
            if (sequencer.allNotes == null)
                return;

            List<Note> notes = sequencer.GetAllNoteOnsInRange(start, end);
            foreach (Note note in notes)
            {
                float percent = (note.start - start) / (end - start);
                DrawNote(percent * width, note.velocity, velocityColor);
            }
        }

        public void DrawTextMeasurements(Rect rect)
        {
            const int barWidth = 4;

            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.padding = new RectOffset(0, barWidth, 0, 0);
            style.alignment = TextAnchor.UpperRight;
            GUI.Label(rect, "127", style);
            style.alignment = TextAnchor.LowerRight;
            GUI.Label(rect, "1", style);

            Rect bar = new Rect(rect.x + rect.width - 1, rect.y, 1, rect.height);
            Rect top = new Rect(rect.x + rect.width - barWidth, rect.y, barWidth, 1);
            Rect bottom = new Rect(rect.x + rect.width - barWidth, rect.y + rect.height - 1, barWidth, 1);
            Rect middle = new Rect(rect.x + rect.width - barWidth / 2.0f, rect.y + rect.height / 2.0f, barWidth / 2.0f, 1);
            EditorGUI.DrawRect(bar, Color.black);
            EditorGUI.DrawRect(top, Color.black);
            EditorGUI.DrawRect(bottom, Color.black);
            EditorGUI.DrawRect(middle, Color.black);
        }

        public void DrawSequencerVelocities(Rect rect, Sequencer sequencer, float start, float end)
        {
            Rect activeArea = new Rect(rect);
            activeArea.x += leftPadding;
            activeArea.width -= leftPadding + rightPadding;

            startTime = start;
            endTime = end;
            sixteenthWidth = activeArea.width / (end - start);
            height = activeArea.height;

            EditorGUI.DrawRect(rect, background);
            EditorGUI.DrawRect(activeArea, activeAreaColor);

            Rect leftBufferArea = new Rect(rect);
            leftBufferArea.width = leftPadding;
            DrawTextMeasurements(leftBufferArea);

            GUI.BeginGroup(activeArea);
            DrawNoteVelocities(sequencer, start, end, activeArea.width);
            if (currentNote != null)
            {
                float percent = (currentNote.start - start) / (end - start);
                DrawNote(percent * activeArea.width, currentNote.velocity, velocityActiveColor);
            }

            GUI.EndGroup();
        }
    }
}
