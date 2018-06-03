// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace AudioHelm
{
    public class PatchBrowserUI
    {
        Vector2 scrollPosition = Vector2.zero;
        Vector2 mousePosition = Vector2.zero;
        GUIStyle rowStyle;
        FileSystemInfo[] files;
        int lastSelectedIndex = -1;
        public string selected = "";

        public const float rowHeight = 22.0f;
        public const int rightPadding = 15;

        string filter;
        public string folder;
        bool directories;

        public PatchBrowserUI(bool browseDirectories, string extension)
        {
            directories = browseDirectories;
            filter = "*" + extension;
            folder = GetFullPatchesPath();

            rowStyle = new GUIStyle();
            rowStyle.alignment = TextAnchor.MiddleLeft;
            rowStyle.padding = new RectOffset(10, 10, 0, 0);
            rowStyle.border = new RectOffset(11, 11, 2, 2);

            files = GetAllFiles();
        }

        public static string GetFullPatchesPath()
        {
            const string patchesPath = "/AudioHelm/Presets/";
            return Application.dataPath + patchesPath;
        }

        public static DirectoryInfo[] GetAllPatchFolders()
        {
            DirectoryInfo directory = new DirectoryInfo(GetFullPatchesPath());
            return directory.GetDirectories();
        }

        public static FileInfo[] GetFilesInDirectory(string directory, string pattern)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            return directoryInfo.GetFiles(pattern);
        }

        FileSystemInfo[] GetAllFiles()
        {
            if (directories)
                return GetAllPatchFolders();
            DirectoryInfo directory = new DirectoryInfo(folder + "/");
            return directory.GetFiles(filter);
        }

        void ReloadPatches()
        {
            FileSystemInfo[] newFiles = GetAllFiles();
            if (newFiles.Length != files.Length)
                lastSelectedIndex = -1;
            files = newFiles;
        }

        public bool DoBrowserEvents(IAudioEffectPlugin plugin, Rect rect)
        {
            Event evt = Event.current;
            mousePosition = evt.mousePosition;
            bool newSelected = false;
            if (evt.type == EventType.MouseDown && rect.Contains(mousePosition))
            {
                FileSystemInfo[] files = GetAllFiles();
                int index = GetPatchIndex(rect, evt.mousePosition);
                if (files.Length > index && index >= 0)
                {
                    lastSelectedIndex = index;
                    selected = files[index].FullName;
                    newSelected = true;
                }
            }

            ReloadPatches();
            return newSelected;
        }

        int GetPatchIndex(Rect guiRect, Vector2 mousePosition)
        {
            Rect rect = new Rect(guiRect);
            rect.width -= rightPadding;
            if (!rect.Contains(mousePosition))
                return -1;
            Vector2 localPosition = mousePosition - guiRect.position + scrollPosition;
            return (int)Mathf.Floor((localPosition.y / rowHeight));
        }

        public void DrawBrowser(Rect rect)
        {
            Color previousColor = GUI.color;
            Color colorEven = new Color(0.8f, 0.8f, 0.8f);
            Color colorOdd = new Color(0.9f, 0.9f, 0.9f);

            float rowWidth = rect.width - rightPadding;
            Rect scrollableArea = new Rect(0, 0, rowWidth, files.Length * rowHeight);
            scrollPosition = GUI.BeginScrollView(rect, scrollPosition, scrollableArea, false, true);

            float y = 0.0f;
            int index = 0;
            foreach (FileSystemInfo file in files)
            {
                Color color = colorOdd;
                if (index % 2 == 0)
                    color = colorEven;

                if (lastSelectedIndex == index)
                    rowStyle.fontStyle = FontStyle.Bold;
                else
                    rowStyle.fontStyle = FontStyle.Normal;

                string name = Path.GetFileNameWithoutExtension(file.Name);

                Rect rowRect = new Rect(0, y, rowWidth, rowHeight + 1);
                EditorGUI.DrawRect(rowRect, color);
                GUI.Label(rowRect, name, rowStyle);
                y += rowHeight;
                index++;
            }
            GUI.EndScrollView();
            GUI.color = previousColor;
        }
    }
}
