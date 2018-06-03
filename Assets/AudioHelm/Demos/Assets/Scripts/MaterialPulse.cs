// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.UI;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class MaterialPulse : MonoBehaviour
    {
        public Material defaultMaterial;
        public Material pulseMaterial;
        public float decay = 2.0f;

        float progress = 0.0f;
        Renderer render;
        RawImage rawImage;

        public void Pulse(float amount)
        {
            progress = Mathf.Max(progress, Mathf.Clamp(amount, 0.0f, 1.0f));
            render = GetComponentInChildren<Renderer>();
            rawImage = GetComponent<RawImage>();
        }

        void Update()
        {
            if (progress == 0.0f)
                return;

            float t = Mathf.Clamp(decay * Time.deltaTime, 0.0f, 1.0f);
            progress = Mathf.Lerp(progress, 0.0f, t);
            if (render)
                render.material.Lerp(defaultMaterial, pulseMaterial, progress);
            if (rawImage)
                rawImage.color = Color.Lerp(defaultMaterial.color, pulseMaterial.color, progress);
        }
    }
}
