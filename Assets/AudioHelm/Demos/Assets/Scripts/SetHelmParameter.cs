// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Slider))]
    public class SetHelmParameter : MonoBehaviour
    {
        public int parameterIndex;
        public HelmController controller;

        public void SetPercent()
        {
            controller.SetParameterAtIndex(parameterIndex, GetComponent<Slider>().value);
        }

        void Start()
        {
            SetPercent();
        }
    }
}
