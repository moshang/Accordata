using UnityEngine;
using UnityEditor;

namespace I2.Loc
{
    using System.Collections.Generic;

	[CustomPropertyDrawer (typeof (TermsPopup))]
	public class TermsPopup_Drawer : PropertyDrawer 
	{
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var filter = ((TermsPopup)this.attribute).Filter;
            ShowGUI(position, property, label, null, filter);
        }


        public static bool ShowGUI(Rect position, SerializedProperty property, GUIContent label, LanguageSource source, string filter = "")
		{
            label = EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck ();

			var Terms = (source==null ? LocalizationManager.GetTermsList() : source.GetTermsList());

            if (string.IsNullOrEmpty(filter) == false)
            {
                Terms = Filter(Terms, filter);
            }

			Terms.Sort(System.StringComparer.OrdinalIgnoreCase);
            Terms.Add("");
            Terms.Add("<inferred from text>");
            Terms.Add("<none>");
            var index = (property.stringValue == "-" || property.stringValue == "" ? Terms.Count - 1 : 
                        (property.stringValue == " " ? Terms.Count - 2 : 
                        Terms.IndexOf(property.stringValue)));
            var newIndex = EditorGUI.Popup(position, label, index, DisplayOptions(Terms));

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = (newIndex < 0 || newIndex == Terms.Count - 1) ? string.Empty : Terms[newIndex];
                if (newIndex == Terms.Count - 1)
                    property.stringValue = "-";
                else
                if (newIndex < 0 || newIndex == Terms.Count - 2)
                    property.stringValue = string.Empty;
                else
                    property.stringValue = Terms[newIndex];

                EditorGUI.EndProperty();
                return true;
            }

            EditorGUI.EndProperty();
            return false;
		}

        private static List<string> Filter(List<string> terms, string filter)
        {
            var filtered = new List<string>();
            for (var i = 0; i < terms.Count; i++)
            {
                var term = terms[i];
                if (term.Contains(filter))
                {
                    filtered.Add(term);
                }
            }

            return filtered;
        }

        private static GUIContent[] DisplayOptions(IList<string> terms)
        {
            var options = new GUIContent[terms.Count];
            for (var i = 0; i < terms.Count; i++)
            {
                options[i] = new GUIContent(terms[i]);
            }

            return options;
        }
	}

    [CustomPropertyDrawer(typeof(LocalizedString))]
    public class LocalizedStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var termRect = rect;    termRect.xMax -= 50;
            var termProp = property.FindPropertyRelative("mTerm");
            TermsPopup_Drawer.ShowGUI(termRect, termProp, label, null);

            var maskRect = rect;    maskRect.xMin = maskRect.xMax - 30;
            var termIgnoreRTL       = property.FindPropertyRelative("mRTL_IgnoreArabicFix");
            var termConvertNumbers  = property.FindPropertyRelative("mRTL_ConvertNumbers");
            int mask = (termIgnoreRTL.boolValue ? 0 : 1) + 
                       (termConvertNumbers.boolValue ? 0 : 2);

            int newMask = EditorGUI.MaskField(maskRect, mask, new string[] { "Arabic Fix", "Ignore Numbers in RTL" });
            if (newMask != mask)
            {
                termIgnoreRTL.boolValue      = (newMask & 1) == 0;
                termConvertNumbers.boolValue = (newMask & 2) == 0;
            }

			var showRect = rect;    showRect.xMin = termRect.xMax; showRect.xMax=maskRect.xMin;
			bool enabled = GUI.enabled;
			GUI.enabled = enabled & (!string.IsNullOrEmpty (termProp.stringValue) && termProp.stringValue!="-");
			if (GUI.Button (showRect, "?")) 
			{
				var source = LocalizationManager.GetSourceContaining(termProp.stringValue);
				LocalizationEditor.mKeyToExplore = termProp.stringValue;
				Selection.activeObject = source;
			}
			GUI.enabled = enabled;
        }
    }
}
