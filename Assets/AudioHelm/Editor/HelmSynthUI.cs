// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace AudioHelm
{
    public class HelmSynthUI : IAudioEffectPluginGUI, NoteHandler
    {
        const string extension = ".helm";
        KeyboardUI keyboard = new KeyboardUI();
        PatchBrowserUI folderBrowser = new PatchBrowserUI(true, "");
        PatchBrowserUI patchBrowser = new PatchBrowserUI(false, extension);
        bool showOptions = false;
        int channel = 0;

        public override string Name
        {
            get { return "Helm"; }
        }

        public override string Description
        {
            get { return "Audio plugin for live synthesis in Unity"; }
        }

        public override string Vendor
        {
            get { return "Matt Tytel"; }
        }

        public void NoteOn(int note, float velocity = 1.0f)
        {
            Native.HelmNoteOn(channel, note, velocity);
        }

        public void NoteOff(int note)
        {
            Native.HelmNoteOff(channel, note);
        }

        public void AllNotesOff()
        {
            Native.HelmAllNotesOff(channel);
        }

        void LoadPatch(IAudioEffectPlugin plugin, string path)
        {
            string patchText = File.ReadAllText(path);
            HelmPatchFormat patch = JsonUtility.FromJson<HelmPatchFormat>(patchText);

            FieldInfo[] fields = typeof(HelmPatchSettings).GetFields();

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsArray && !field.IsLiteral)
                {
                    float val = (float)field.GetValue(patch.settings);
                    string name = HelmPatchSettings.ConvertToPlugin(field.Name);
                    plugin.SetFloatParameter(name, val);
                }
            }

            for (int i = 0; i < HelmPatchSettings.kMaxModulations; ++i)
                plugin.SetFloatParameter("mod" + i + "value", 0.0f);

            int modulationIndex = 0;
            foreach (HelmModulationSetting modulation in patch.settings.modulations)
            {
                if (modulationIndex >= HelmPatchSettings.kMaxModulations)
                {
                    Debug.LogWarning("Only 16 modulations are currently supported in the Helm Unity plugin.");
                    break;
                }
                string prefix = "mod" + modulationIndex;

                float source = HelmPatchSettings.GetSourceIndex(modulation.source);
                plugin.SetFloatParameter(prefix + "source", source);
                float dest = HelmPatchSettings.GetDestinationIndex(modulation.destination);
                plugin.SetFloatParameter(prefix + "dest", dest);
                plugin.SetFloatParameter(prefix + "value", modulation.amount);

                modulationIndex++;
            }
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            if (plugin == null || keyboard == null || folderBrowser == null || patchBrowser == null)
                return false;

            Color prev_color = GUI.backgroundColor;

            GUILayout.Space(5.0f);
            Rect keyboardRect = GUILayoutUtility.GetRect(200, 60, GUILayout.ExpandWidth(true));

            keyboard.DoKeyboardEvents(keyboardRect, this);
            keyboard.DrawKeyboard(keyboardRect);

            GUI.backgroundColor = prev_color;
            GUILayout.Space(5.0f);

            GUIStyle titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontSize = 12;
            titleStyle.fontStyle = FontStyle.Bold;
            Rect titleRect = GUILayoutUtility.GetRect(200, PatchBrowserUI.rowHeight, GUILayout.ExpandWidth(true));
            titleRect.width = titleRect.width / 2 - PatchBrowserUI.rightPadding;
            GUI.Label(titleRect, "Folder", titleStyle);
            titleRect.x = titleRect.xMax + PatchBrowserUI.rightPadding;
            GUI.Label(titleRect, "Patch", titleStyle);

            Rect browserRect = GUILayoutUtility.GetRect(200, 120, GUILayout.ExpandWidth(true));
            browserRect.x -= 14.0f;
            browserRect.width += 18.0f;

            Rect folderRect = new Rect(browserRect);
            folderRect.width = (int)(browserRect.width / 2);

            Rect patchRect = new Rect(folderRect);
            patchRect.x = folderRect.xMax;

            if (folderBrowser.DoBrowserEvents(plugin, folderRect))
            {
                patchBrowser.folder = folderBrowser.selected;
                plugin.SetFloatParameter("oscmix", Random.Range(0.0f, 0.1f));
            }

            folderBrowser.DrawBrowser(folderRect);

            if (patchBrowser.DoBrowserEvents(plugin, patchRect))
                LoadPatch(plugin, patchBrowser.selected);
            patchBrowser.DrawBrowser(patchRect);

            GUILayout.Space(5.0f);
            GUI.backgroundColor = prev_color;

            float pluginChannel = 0.0f;
            plugin.GetFloatParameter("Channel", out pluginChannel);
            channel = (int)pluginChannel;
            float newChannel = EditorGUILayout.IntSlider("Channel", channel, 0, Utils.kMaxChannels - 1);
            showOptions = EditorGUILayout.Toggle("Show All Options", showOptions);


            if (newChannel != channel)
                plugin.SetFloatParameter("Channel", newChannel);

            return showOptions;
        }
    }
}
