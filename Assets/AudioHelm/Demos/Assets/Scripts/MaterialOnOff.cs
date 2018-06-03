// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class MaterialOnOff : MonoBehaviour
    {
        public Material offMaterial;
        public Material onMaterial;

        RawImage rawImage;
        float progress = 0.0f;
        bool on = false;

        void Start()
        {
            rawImage = GetComponent<RawImage>();
        }

        public void On()
        {
            on = true;
            progress = 1.0f;
            rawImage.color = onMaterial.color;
        }

        public void Off()
        {
            on = false;
        }

        void Update()
        {
            if (progress == 0.0f)
                return;

            if (on)
            {
                float t = Mathf.Clamp(8.0f * Time.deltaTime, 0.0f, 1.0f);
                progress = Mathf.Lerp(progress, 0.3f, t);
            }
            else
            {
                float t = Mathf.Clamp(12.0f * Time.deltaTime, 0.0f, 1.0f);
                progress = Mathf.Lerp(progress, 0.0f, t);
            }

            rawImage.color = Color.Lerp(offMaterial.color, onMaterial.color, progress);
        }
    }
}
