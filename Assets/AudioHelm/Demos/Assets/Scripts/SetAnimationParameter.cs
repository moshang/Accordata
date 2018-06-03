// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Slider))]
    public class SetAnimationParameter : MonoBehaviour
    {
        public string parameter;
        public Animator animator;

        public void SetValue()
        {
            animator.SetFloat(parameter, GetComponent<Slider>().value);
        }

        void Start()
        {
            SetValue();
        }
    }
}
