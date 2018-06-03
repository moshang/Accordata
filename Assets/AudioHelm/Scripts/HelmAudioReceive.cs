// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    /// <summary>
    /// ## [Switch to Manual](../manual/class_audio_helm_1_1_helm_audio_receive.html)<br>
    /// Receives an audio stream from a synthesizer instance for use in a spatialization plugin.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Audio Helm/Helm Audio Receive")]
    [HelpURL("http://tytel.org/audiohelm/manual/class_audio_helm_1_1_helm_audio_receive.html")]
    public class HelmAudioReceive : MonoBehaviour
    {
        public int channel = 0;

        void OnAudioFilterRead(float[] data, int audioChannels)
        {
            Native.HelmGetBufferData(channel, data, data.Length / audioChannels, audioChannels);
        }
    }
}
