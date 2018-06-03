// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioHelm
{
    public class KeyzoneEditorUI
    {
        enum MouseMode
        {
            kWaiting,
            kDraggingRoot,
            kDraggingRange,
            kDraggingRangeStart,
            kDraggingRangeEnd,
        }
        MouseMode mouseMode = MouseMode.kWaiting;
        Keyzone currentKeyzone;
        Vector2 keyboardScrollPosition;
        float keyWidth = minKeyWidth;
        float lastRectWidth = 0.0f;
        int pressOffset = 0;
        int scrollWidth = 15;

        Color lightenColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        Color zoneDivisionColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        Color keylaneBackground = new Color(0.6f, 0.6f, 0.6f);
        Color rootNoteColor = new Color(1.0f, 0.2f, 0.2f);
        Color activeKeyColor = new Color(1.0f, 0.2f, 0.2f);
        Color rootHandleColor = new Color(1.0f, 0.8f, 0.8f);
        Color keyzoneRangeColor = new Color(1.0f, 0.6f, 0.2f);
        const int keyboardHeight = 18;
        const int rowHeight = 32;
        const int keyzoneWidth = 150;
        const int minKeyWidth = 6;
        const int buttonBuffer = 17;
        const int resizeHandleWidth = 5;
        const int rootHandleKeyRadius = 2;

        public KeyzoneEditorUI(int scroll)
        {
            scrollWidth = scroll;
        }

        Vector2 GetScrollPosition(Sampler sampler, float windowWidth, float keyWidth)
        {
            int minNote = Utils.kMidiSize - 1;
            int maxNote = 0;
            foreach (Keyzone keyzone in sampler.keyzones)
            {
                minNote = Mathf.Min(minNote, keyzone.minKey, keyzone.rootKey);
                maxNote = Mathf.Max(maxNote, keyzone.maxKey, keyzone.rootKey);
            }

            int center = (maxNote + minNote) / 2;
            float position = center * keyWidth;
            position = Mathf.Clamp(position, 0.0f, Utils.kMidiSize * keyWidth - windowWidth);
            return new Vector2(position, 0);
        }

        Vector2 GetKeyzonePosition(Rect rect, Vector2 mousePosition)
        {
            float row = (mousePosition.y - keyboardHeight - rect.y) / (rowHeight / 2.0f);
            float key = (mousePosition.x - keyzoneWidth - rect.x + keyboardScrollPosition.x) / keyWidth;
            return new Vector2(key, row);
        }

        void MouseUp(float key, int row, Sampler sampler)
        {
            currentKeyzone = null;
        }

        void MouseDown(float key, int row, Sampler sampler)
        {
            currentKeyzone = null;
            if (row < 0 || row >= sampler.keyzones.Count * 2)
                return;

            int keyzoneIndex = row / 2;
            bool rootRow = row % 2 == 0;

            if (keyzoneIndex < 0 || keyzoneIndex >= sampler.keyzones.Count)
                return;

            Keyzone keyzone = sampler.keyzones[keyzoneIndex];

            if (rootRow)
            {
                if (keyzone.rootKey <= key + rootHandleKeyRadius && keyzone.rootKey + 1 >= key - rootHandleKeyRadius)
                {
                    Undo.RecordObject(sampler, "Change Keyzone Root");

                    mouseMode = MouseMode.kDraggingRoot;
                    currentKeyzone = keyzone;
                    pressOffset = Mathf.FloorToInt(key) - keyzone.rootKey;
                }
            }
            else if (keyzone.minKey <= key && keyzone.maxKey + 1 >= key)
            {
                currentKeyzone = keyzone;

                int pixelsFromStart = (int)((key - keyzone.minKey) * keyWidth);
                int pixelsFromEnd = (int)((keyzone.maxKey + 1 - key) * keyWidth);
                if (pixelsFromStart <= resizeHandleWidth && pixelsFromStart < pixelsFromEnd)
                {
                    Undo.RecordObject(sampler, "Change Keyzone Start Note");
                    mouseMode = MouseMode.kDraggingRangeStart;
                }
                else if (pixelsFromEnd <= resizeHandleWidth)
                {
                    Undo.RecordObject(sampler, "Change Keyzone End Note");
                    mouseMode = MouseMode.kDraggingRangeEnd;
                }
                else
                {
                    Undo.RecordObject(sampler, "Drag Keyzone Range");
                    mouseMode = MouseMode.kDraggingRange;
                }

                pressOffset = Mathf.FloorToInt(key) - currentKeyzone.minKey;
            }
        }

        void MouseDrag(float key, int row, Sampler sampler, SerializedProperty keyzones)
        {
            if (currentKeyzone == null)
                return;

            SerializedProperty serializedKeyzone = null;
            int index = sampler.keyzones.IndexOf(currentKeyzone);
            if (index >= 0)
                serializedKeyzone = keyzones.GetArrayElementAtIndex(index);

            int roundedKey = Mathf.Clamp(Mathf.RoundToInt(key), 0, Utils.kMidiSize - 1);
            int flooredKey = Mathf.Clamp(Mathf.FloorToInt(key), 0, Utils.kMidiSize - 1);
            if (mouseMode == MouseMode.kDraggingRangeEnd)
            {
                Undo.RecordObject(sampler, "Change Keyzone End Note");
                currentKeyzone.maxKey = Mathf.Max(roundedKey - 1, currentKeyzone.minKey);
                if (serializedKeyzone != null)
                    serializedKeyzone.FindPropertyRelative("maxKey").intValue = currentKeyzone.maxKey;
            }
            else if (mouseMode == MouseMode.kDraggingRangeStart)
            {
                Undo.RecordObject(sampler, "Change Keyzone Start Note");
                currentKeyzone.minKey = Mathf.Min(roundedKey, currentKeyzone.maxKey);
                if (serializedKeyzone != null)
                    serializedKeyzone.FindPropertyRelative("minKey").intValue = currentKeyzone.minKey;
            }
            else if (mouseMode == MouseMode.kDraggingRoot)
            {
                Undo.RecordObject(sampler, "Change Keyzone Root");
                currentKeyzone.rootKey = flooredKey - pressOffset;
                if (serializedKeyzone != null)
                    serializedKeyzone.FindPropertyRelative("rootKey").intValue = currentKeyzone.rootKey;
            }
            else if (mouseMode == MouseMode.kDraggingRange)
            {
                Undo.RecordObject(sampler, "Drag Keyzone Range");
                int range = currentKeyzone.maxKey - currentKeyzone.minKey;
                int min = Mathf.Clamp(flooredKey - pressOffset, 0, Utils.kMidiSize - 1 - range);
                currentKeyzone.minKey = min;
                currentKeyzone.maxKey = min + range;
                if (serializedKeyzone != null)
                {
                    serializedKeyzone.FindPropertyRelative("minKey").intValue = currentKeyzone.minKey;
                    serializedKeyzone.FindPropertyRelative("maxKey").intValue = currentKeyzone.maxKey;
                }
            }
        }

        public bool DoKeyzoneEvents(Rect rect, Sampler sampler, SerializedProperty keyzones)
        {
            Event evt = Event.current;
            if (!evt.isMouse)
                return false;

            Vector2 keyzonePosition = GetKeyzonePosition(rect, evt.mousePosition);
            float key = keyzonePosition.x;
            int row = Mathf.FloorToInt(keyzonePosition.y);

            if (evt.type == EventType.MouseUp && currentKeyzone != null)
            {
                MouseUp(key, row, sampler);
                return true;
            }

            Rect ignoreScrollRect = new Rect(rect);

            if (evt.type == EventType.MouseDown && ignoreScrollRect.Contains(evt.mousePosition))
                MouseDown(key, row, sampler);
            else if (evt.type == EventType.MouseDrag && currentKeyzone != null)
                MouseDrag(key, row, sampler, keyzones);
            return true;
        }

        void DrawKeyboard(float height)
        {
            GUIStyle cStyle = new GUIStyle();
            cStyle.fontSize = 9;
            cStyle.alignment = TextAnchor.MiddleLeft;
            cStyle.clipping = TextClipping.Clip;
            cStyle.padding = new RectOffset(1, 1, 0, 0);

            int keyHeight = keyboardHeight / 2;
            int labelHeight = keyboardHeight - keyHeight;
            int activeKey = -1;

            if (currentKeyzone != null)
            {
                if (mouseMode == MouseMode.kDraggingRange || mouseMode == MouseMode.kDraggingRangeStart)
                    activeKey = currentKeyzone.minKey;
                else if (mouseMode == MouseMode.kDraggingRangeEnd)
                    activeKey = currentKeyzone.maxKey;
                else if (mouseMode == MouseMode.kDraggingRoot)
                    activeKey = currentKeyzone.rootKey;
            }
            for (int i = 0; i < Utils.kMidiSize; ++i)
            {
                float x = i * keyWidth;

                Rect rect = new Rect(x, labelHeight, keyWidth, keyHeight);
                Color keyColor = Color.white;
                if (i == activeKey)
                    keyColor = activeKeyColor;
                else if (Utils.IsBlackKey(i))
                    keyColor = Color.black;

                if (Utils.IsC(i))
                {
                    Rect labelRect = new Rect(x, 0, Utils.kNotesPerOctave * keyWidth, labelHeight);
                    GUI.Label(labelRect, "C" + Utils.GetOctave(i), cStyle);
                }

                EditorGUI.DrawRect(rect, keyColor);

                if (!Utils.IsBlackKey(i))
                {
                    Rect keyLane = new Rect(x, keyboardHeight, keyWidth, height - keyboardHeight);
                    EditorGUI.DrawRect(keyLane, lightenColor);
                    if (!Utils.IsBlackKey(i + 1))
                    {
                        if (Utils.IsC(i + 1))
                            EditorGUI.DrawRect(new Rect(x + keyWidth - 1, 0, 1, keyboardHeight), Color.black);
                        else
                            EditorGUI.DrawRect(new Rect(x + keyWidth - 1, labelHeight, 1, keyHeight), Color.black);
                    }
                }
            }
        }

        void DrawClips(Sampler sampler, SerializedProperty keyzones)
        {
            int height = rowHeight / 2;
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding = new RectOffset(0, 0, 0, 0);
            style.fontSize = height - 4;
            int y = keyboardHeight;

            Keyzone remove = null;

            foreach (Keyzone keyzone in sampler.keyzones)
            {
                Rect buttonRect = new Rect(0, y, height, height);
                Rect clipRect = new Rect(buttonRect.xMax, y, keyzoneWidth - buttonRect.width, height);
                Rect mixerRect = new Rect(buttonRect.xMax, y + height, keyzoneWidth - buttonRect.width, height);

                if (GUI.Button(buttonRect, "X", style))
                    remove = keyzone;

                AudioClip clip = EditorGUI.ObjectField(clipRect, keyzone.audioClip, typeof(AudioClip), false)
                                 as AudioClip;
                AudioMixerGroup mixer = EditorGUI.ObjectField(mixerRect, keyzone.mixer, typeof(AudioMixerGroup), false)
                                        as AudioMixerGroup;

                if (clip != keyzone.audioClip)
                {
                    if (clip == null)
                        Undo.RecordObject(sampler, "Remove AudioClip from Keyzone");
                    else
                        Undo.RecordObject(sampler, "Change AudioClip in Keyzone");
                    keyzone.audioClip = clip;
                }
                if (mixer != keyzone.mixer)
                {
                    if (mixer == null)
                        Undo.RecordObject(sampler, "Remove AudioMixerGroup from Keyzone");
                    else
                        Undo.RecordObject(sampler, "Change AudioMixerGroup in Keyzone");
                    keyzone.mixer = mixer;
                }
                y += rowHeight;
            }

            if (remove != null)
            {
                Undo.RecordObject(sampler, "Delete Keyzone");
                int index = sampler.RemoveKeyzone(remove);
                if (index >= 0)
                    keyzones.DeleteArrayElementAtIndex(index);
            }
        }

        Rect ShrinkRect(Rect original, int amount)
        {
            return new Rect(original.x + amount, original.y + amount,
                            original.width - 2 * amount, original.height - 2 * amount);
        }

        void DrawKeyzoneRanges(Sampler sampler)
        {
            int y = keyboardHeight;
            GUIStyle rangeLabelStyle = new GUIStyle();
            rangeLabelStyle.fontSize = 9;
            rangeLabelStyle.alignment = TextAnchor.MiddleCenter;
            rangeLabelStyle.clipping = TextClipping.Clip;
            rangeLabelStyle.padding = new RectOffset(3, 3, 0, 0);

            foreach (Keyzone keyzone in sampler.keyzones)
            {
                int range = keyzone.maxKey - keyzone.minKey + 1;
                float rangeX = keyzone.minKey * keyWidth;
                float width = range * keyWidth;
                float height = (rowHeight - 1) / 2.0f;

                Rect rootRect = new Rect(keyzone.rootKey * keyWidth, y, keyWidth, height + 1);
                float rootHandleHeight = 3.0f * height / 5.0f;
                Rect rootRectHandle = new Rect((keyzone.rootKey - rootHandleKeyRadius) * keyWidth, y + height - rootHandleHeight,
                                               (1 + 2 * rootHandleKeyRadius) * keyWidth, rootHandleHeight + 1);

                Rect rangeRect = new Rect(rangeX, y + height, width, height);

                EditorGUI.DrawRect(rootRect, Color.black);
                EditorGUI.DrawRect(rootRectHandle, Color.black);
                EditorGUI.DrawRect(rangeRect, Color.black);
                EditorGUI.DrawRect(ShrinkRect(rootRect, 1), rootNoteColor);
                EditorGUI.DrawRect(ShrinkRect(rootRectHandle, 1), rootHandleColor);
                EditorGUI.DrawRect(ShrinkRect(rangeRect, 1), keyzoneRangeColor);

                GUI.Label(rootRectHandle, "Root", rangeLabelStyle);
                GUI.Label(rangeRect, "Range", rangeLabelStyle);

                Rect leftResizeRect = new Rect(rangeX, y + height, resizeHandleWidth, height);
                Rect rightResizeRect = new Rect(rangeX + width - resizeHandleWidth, y + height, resizeHandleWidth, height);
                EditorGUIUtility.AddCursorRect(leftResizeRect, MouseCursor.SplitResizeLeftRight);
                EditorGUIUtility.AddCursorRect(rightResizeRect, MouseCursor.SplitResizeLeftRight);

                y += rowHeight;
                EditorGUI.DrawRect(new Rect(0, y - 1, keyWidth * Utils.kMidiSize, 1), zoneDivisionColor);
            }
        }

        public int GetHeight(Sampler sampler)
        {
            return keyboardHeight + rowHeight * sampler.keyzones.Count + scrollWidth;
        }

        void AddKeyzone(Sampler sampler, SerializedProperty keyzones)
        {
            Keyzone keyzone = sampler.AddKeyzone();
            if (sampler.keyzones.Count >= 2)
            {
                Keyzone lastKeyzone = sampler.keyzones[sampler.keyzones.Count - 2];
                int min = lastKeyzone.maxKey + 1;
                int root = min + lastKeyzone.rootKey - lastKeyzone.minKey;
                int max = min + lastKeyzone.maxKey - lastKeyzone.minKey;
                if (max < Utils.kMidiSize && root < Utils.kMidiSize)
                {
                    keyzone.minKey = min;
                    keyzone.maxKey = max;
                    keyzone.rootKey = root;
                }
                keyzone.mixer = lastKeyzone.mixer;
            }

            keyzones.arraySize++;
            SerializedProperty newKeyzone = keyzones.GetArrayElementAtIndex(keyzones.arraySize - 1);
            newKeyzone.FindPropertyRelative("audioClip").objectReferenceValue = keyzone.audioClip;
            newKeyzone.FindPropertyRelative("rootKey").intValue = keyzone.rootKey;
            newKeyzone.FindPropertyRelative("minKey").intValue = keyzone.minKey;
            newKeyzone.FindPropertyRelative("maxKey").intValue = keyzone.maxKey;
            newKeyzone.FindPropertyRelative("minVelocity").floatValue = keyzone.minVelocity;
            newKeyzone.FindPropertyRelative("maxVelocity").floatValue = keyzone.maxVelocity;
        }

        public void DrawKeyzones(Rect rect, Sampler sampler, SerializedProperty keyzones)
        {
            int numKeyzones = 0;
            float scrollableHeight = Mathf.Max(rect.height, keyboardHeight + numKeyzones * rowHeight);
            float computedKeyWidth = (1.0f * rect.width - keyzoneWidth) / Utils.kMidiSize;
            keyWidth = Mathf.Max(computedKeyWidth, minKeyWidth);
            float scrollableWidth = Utils.kMidiSize * keyWidth;

            GUI.BeginGroup(rect);
            DrawClips(sampler, keyzones);
            Rect buttonRect = new Rect(0, 0, keyzoneWidth - buttonBuffer, keyboardHeight);
            if (GUI.Button(buttonRect, "Add Keyzone"))
            {
                Undo.RecordObject(sampler, "Add Keyzone");
                AddKeyzone(sampler, keyzones);
            }

            Rect keySection = new Rect(keyzoneWidth, 0, rect.width - keyzoneWidth, rect.height);
            Rect keyboardScroll = new Rect(0, 0, scrollableWidth, rect.height - scrollWidth);

            if (lastRectWidth != rect.width)
            {
                lastRectWidth = rect.width;
                keyboardScrollPosition = GetScrollPosition(sampler, rect.width, keyWidth);
            }

            keyboardScrollPosition = GUI.BeginScrollView(keySection, keyboardScrollPosition, keyboardScroll, true, false);

            EditorGUI.DrawRect(new Rect(0, 0, keyboardScroll.width, keyboardScroll.height), keylaneBackground);

            DrawKeyboard(scrollableHeight);
            DrawKeyzoneRanges(sampler);
            GUI.EndScrollView();
            GUI.EndGroup();
        }
    }
}
