// Copyright 2017 Matt Tytel

using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioHelm
{
    public class HelmParameterListUI
    {
        const int rowHeight = 24;
        const int buttonHeight = 15;
        const int buttonBuffer = 17;
        const int addButtonHeight = 20;
        const int parameterWidth = 120;
        const int scaleWidth = 60;
        const int sliderBuffer = 10;

        void DrawParameterList(HelmController controller, SerializedObject serialized, float width)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding = new RectOffset(0, 0, 0, 0);
            style.fontSize = buttonHeight - 4;
            int y = addButtonHeight + sliderBuffer;
            int extra_y = (rowHeight - buttonHeight) / 3;

            HelmParameter remove = null;

            int paramIndex = 0;
            foreach (HelmParameter synthParameter in controller.synthParameters)
            {
                Rect buttonRect = new Rect(0, y + extra_y, buttonHeight, buttonHeight);
                Rect paramRect = new Rect(buttonRect.xMax, y + extra_y, parameterWidth, buttonHeight);
                Rect scaleRect = new Rect(paramRect.xMax, y + extra_y, scaleWidth, buttonHeight);
                Rect sliderRect = new Rect(scaleRect.xMax + sliderBuffer, y + extra_y,
                                           width - scaleRect.xMax - sliderBuffer, buttonHeight);

                if (GUI.Button(buttonRect, "X", style))
                    remove = synthParameter;

                Param param = (Param)EditorGUI.EnumPopup(paramRect, synthParameter.parameter);
                HelmParameter.ScaleType scale = (HelmParameter.ScaleType)EditorGUI.EnumPopup(scaleRect, synthParameter.scaleType);

                if (param != synthParameter.parameter)
                {
                    Undo.RecordObject(controller, "Change Parameter Control");
                    synthParameter.parameter = param;

                    if (scale == HelmParameter.ScaleType.kByPercent)
                        controller.SetParameterAtIndex(paramIndex, controller.GetParameterPercent(param));
                    else
                        controller.SetParameterAtIndex(paramIndex, controller.GetParameterValue(param));
                }

                if (scale != synthParameter.scaleType)
                {
                    Undo.RecordObject(controller, "Change Parameter Scale Type");
                    synthParameter.scaleType = scale;
                    float min = synthParameter.GetMinimumRange();
                    float max = synthParameter.GetMaximumRange();
                    float val = controller.GetParameterAtIndex(paramIndex);

                    if (synthParameter.scaleType == HelmParameter.ScaleType.kByPercent)
                        val = Mathf.Clamp((val - min) / (max - min), 0.0f, 1.0f);
                    else
                        val = Mathf.Lerp(min, max, val);

                    controller.SetParameterAtIndex(paramIndex, val);
                }

                SerializedProperty paramProperty = serialized.FindProperty("synthParamValue" + paramIndex);

                if (synthParameter.scaleType == HelmParameter.ScaleType.kByPercent)
                    EditorGUI.Slider(sliderRect, paramProperty, 0.0f, 1.0f, "");
                else
                {
                    EditorGUI.Slider(sliderRect, paramProperty,
                                     synthParameter.GetMinimumRange(), synthParameter.GetMaximumRange(), "");
                }

                y += rowHeight;

                paramIndex++;
            }

            if (remove != null)
            {
                Undo.RecordObject(controller, "Remove Parameter Control");
                controller.RemoveParameter(remove);
            }

            controller.UpdateAllParameters();
        }

        public int GetHeight(HelmController controller)
        {
            return addButtonHeight + rowHeight * controller.synthParameters.Count + sliderBuffer;
        }

        public void DrawParameters(Rect rect, HelmController controller, SerializedObject serialized)
        {
            GUI.BeginGroup(rect);
            DrawParameterList(controller, serialized, rect.width);
            Rect buttonRect = new Rect(rect.width / 4, 0, rect.width / 2, addButtonHeight);
            if (GUI.Button(buttonRect, "Add Parameter Control"))
            {
                Undo.RecordObject(controller, "Add Parameter Control");
                controller.AddEmptyParameter();
            }

            GUI.EndGroup();
        }
    }
}
