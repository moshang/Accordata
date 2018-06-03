// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class ToggleParameter : MonoBehaviour
    {
        public HelmController controller;
        public Param param;
        public float onValue = 1.0f;
        public float offValue = 0.0f;

        bool isOn = false;

        void Start()
        {
            controller.SetParameterValue(param, offValue);
        }

        public void Toggle()
        {
            isOn = !isOn;
            if (isOn)
                controller.SetParameterValue(param, onValue);
            else
                controller.SetParameterValue(param, offValue);
        }
    }
}
