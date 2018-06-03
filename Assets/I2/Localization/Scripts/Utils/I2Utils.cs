using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace I2.Loc
{
    public static class I2Utils
    {
        public static string ReverseText(string source)
        {
            var len = source.Length;
            var output = new char[len];
            for (var i = 0; i < len; i++)
            {
                output[(len - 1) - i] = source[i];
            }
            return new string(output);
        }

        public static string RemoveNonASCII(string text, bool allowCategory = false)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return new string(text.ToCharArray().Select(c => (char.IsControl(c) || (c == '\\' && !allowCategory)) ? ' ' : c).ToArray());
        }

        public static string SplitLine(string line, int maxCharacters)
        {
            if (maxCharacters <= 0 || line.Length < maxCharacters)
                return line;

            var chars = line.ToCharArray();
            bool insideOfLine = true;
            bool allowNewLine = false;
            for (int i = 0, nCharsInLine = 0; i < chars.Length; ++i)
            {
                if (insideOfLine)
                {
                    nCharsInLine++;
                    if (chars[i] == '\n')
                    {
                        nCharsInLine = 0;
                    }
                    if (nCharsInLine >= maxCharacters && char.IsWhiteSpace(chars[i]))
                    {
                        chars[i] = '\n';
                        insideOfLine = false;
                        allowNewLine = false;
                    }
                }
                else
                {
                    if (!char.IsWhiteSpace(chars[i]))
                    {
                        insideOfLine = true;
                        nCharsInLine = 0;
                    }
                    else
                    {
                        if (chars[i] != '\n')
                        {
                            chars[i] = (char)0;
                        }
                        else
                        {
                            if (!allowNewLine)
                                chars[i] = (char)0;
                            allowNewLine = true;
                        }
                    }
                }
            }

            return new string(chars.Where(c => c != (char)0).ToArray());
        }

        public static bool FindNextTag(string line, int iStart, out int tagStart, out int tagEnd)
        {
            tagStart = -1;
            tagEnd = -1;
            int len = line.Length;

            // Find where the tag starts
            for (tagStart = iStart; tagStart < len; ++tagStart)
                if (line[tagStart] == '[' || line[tagStart] == '(' || line[tagStart] == '{')
                    break;

            if (tagStart == len)
                return false;

            bool isArabic = false;
            for (tagEnd = tagStart + 1; tagEnd < len; ++tagEnd)
            {
                char c = line[tagEnd];
                if (c == ']' || c == ')' || c == '}')
                {
                    if (isArabic) return FindNextTag(line, tagEnd + 1, out tagStart, out tagEnd);
                    else return true;
                }
                if (c > 255) isArabic = true;
            }

            // there is an open, but not close character
            return false;
        }

        public static bool IsPlaying()
        {
            if (Application.isPlaying)
                return true;
            #if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
            #else
                return false;
            #endif
        }

        public static string GetPath(this Transform tr)
        {
            var parent = tr.parent;
            if (tr == null)
                return tr.name;
            return parent.GetPath() + "/" + tr.name;
        }

#if UNITY_5_3_OR_NEWER
        public static Transform FindObject(string objectPath)
        {
            return FindObject(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), objectPath);
        }


        public static Transform FindObject(UnityEngine.SceneManagement.Scene scene, string objectPath)
        {
            //var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            var roots = scene.GetRootGameObjects();
            for (int i=0; i<roots.Length; ++i)
            {
                var root = roots[i].transform;
                if (root.name == objectPath)
                    return root;

                if (!objectPath.StartsWith(root.name + "/"))
                    continue;

                return FindObject(root, objectPath.Substring(root.name.Length + 1));
            }
            return null;
        }

        public static Transform FindObject(Transform root, string objectPath)
        {
            for (int i=0; i<root.childCount; ++i)
            {
                var child = root.GetChild(i);
                if (child.name == objectPath)
                    return child;

                if (!objectPath.StartsWith(child.name + "/"))
                    continue;

                return FindObject(child, objectPath.Substring(child.name.Length + 1));
            }
            return null;
        }
#endif

        public static H FindInParents<H>(Transform tr) where H : Component
        {
            if (!tr)
                return null;

            H comp = tr.GetComponent<H>();
            while (!comp && tr)
            {
                comp = tr.GetComponent<H>();
                tr = tr.parent;
            }
            return comp;
        }

    }
}