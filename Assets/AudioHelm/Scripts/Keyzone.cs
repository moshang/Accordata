// Copyright 2017 Matt Tytel

using UnityEngine;
using UnityEngine.Audio;

namespace AudioHelm
{
    /// <summary>
    /// A single keyzone in a Sampler.
    /// A keyzone has a single AudioClip that it should play if a key between minKey and maxKey
    /// and the velocity is played between minVelocity and maxVelocity.
    /// When played, the pitch of the AudioClip is shifted by (theKeyPlayed - rootKey) semitones.
    /// </summary>
    [System.Serializable]
    public class Keyzone
    {
        /// <summary>
        /// The audio clip to play for this keyzone.
        /// </summary>
        public AudioClip audioClip;

        /// <summary>
        /// The mixer to send the audio clip to when played.
        /// </summary>
        public AudioMixerGroup mixer;

        /// <summary>
        /// The MIDI key to pitch shift this note from.
        /// If a higher note is played, the sample is pitched up.
        /// If a lower note is played, the sample is pitched down.
        /// </summary>
        public int rootKey = Utils.kMiddleC;

        /// <summary>
        /// The minimum MIDI key this Keyzone is valid for.
        /// </summary>
        public int minKey = 0;

        /// <summary>
        /// The maximum MIDI key this Keyzone is valid for.
        /// </summary>
        public int maxKey = Utils.kMidiSize - 1;

        /// <summary>
        /// The minimum velocity this Keyzone is valid for. [0.0, 1.0f]
        /// </summary>
        public float minVelocity = 0.0f;

        /// <summary>
        /// The maximum velocity this Keyzone is valid for. [0.0, 1.0f]
        /// </summary>
        public float maxVelocity = 1.0f;

        /// <summary>
        /// Checks if the keyzone will play for the current note.
        /// </summary>
        /// <returns><c>true</c>, if for note is within the kezone, <c>false</c> otherwise.</returns>
        /// <param name="note">The note to check.</param>
        public bool ValidForNote(int note)
        {
            return note <= maxKey && note >= minKey && audioClip != null;
        }

        /// <summary>
        /// Checks if the keyzone will play for the current note and velocity.
        /// </summary>
        /// <returns><c>true</c>, if for note and velocity are within the kezone, <c>false</c> otherwise.</returns>
        /// <param name="note">The note to check.</param>
        /// <param name="velocity">The velocity to check.</param>
        public bool ValidForNote(int note, float velocity)
        {
            return ValidForNote(note) && velocity >= minVelocity && velocity <= maxVelocity;
        }
    }
}
