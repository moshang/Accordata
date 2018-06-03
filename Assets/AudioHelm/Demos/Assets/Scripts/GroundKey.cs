// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class GroundKey : MonoBehaviour
    {
        public Renderer keyLight;
        bool noteOn = false;

        public bool IsInside(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            localPosition.y = 0.0f;
            return Mathf.Abs(localPosition.x) < 0.5f && Mathf.Abs(localPosition.z) < 0.5f;
        }

        public bool IsOn()
        {
            return noteOn;
        }

        public void SetOn(bool isOn)
        {
            noteOn = isOn;
        }

        void Update()
        {
            if (noteOn)
            {
                MaterialPulse pulse = GetComponent<MaterialPulse>();
                if (pulse)
                    pulse.Pulse(1.0f);
            }
        }
    }
}
