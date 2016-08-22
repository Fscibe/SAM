using UnityEngine;
using UnityEngine.Audio;

namespace Sam
{
    /// <summary>
    /// Player type.
    /// </summary>
    public enum PlayerType
    {
        Random = 0,
        Loop = 1
    }

    /// <summary>
    /// Represents an ambiance's layer. Describes how sounds must be played.
    /// </summary>
    [System.Serializable]
    public class LayerData
    {
        public string name = "";
        public PlayerType playerType = PlayerType.Random;
        public float minVol = 1.0f;
        public float maxVol = 1.0f;
        public float minPitch = 1.0f;
        public float maxPitch = 1.0f;
        public float minPan = 0.0f;
        public float maxPan = 0.0f;
        public int minCount = 1;
        public int maxCount = 1;
        public float period = 10.0f;
        public int noRepeatCount = 0;
        public float silence = 0.0f;
        public AudioMixerGroup mixerGroup = null;
        public AudioClip[] clips = new AudioClip[1] { null };
        public bool mute = false;
        public bool solo = false;

#if UNITY_EDITOR
        /// <summary>
        /// If TRUE, means the Player type changed.
        /// </summary>
        [System.NonSerialized]
        public bool isPlayerTypeDirty = false;
        /// <summary>
        /// If TRUE, means the Player data changed so Player must be reinitialized.
        /// </summary>
        [System.NonSerialized]
        public bool isPlayerDataDirty = false;
#endif

        /// <summary>
        /// Make sure every property is valid.
        /// </summary>
        public void Validate()
        {
            minVol = Mathf.Max(minVol, 0);
            maxVol = Mathf.Max(maxVol, 0);
            minPitch = Mathf.Max(minPitch, 0);
            maxPitch = Mathf.Max(maxPitch, 0);
            minCount = Mathf.Max(minCount, 0);
            maxCount = Mathf.Max(maxCount, 0);
            period = Mathf.Max(period, 1.0f);
            noRepeatCount = Mathf.Max(noRepeatCount, 0);
            silence = Mathf.Max(silence, 0.0f);
        }

        /// <summary>
        /// Add an empty (null) source slot in sources array.
        /// </summary>
        public void AddSource(int index)
        {
            System.Array.Resize<AudioClip>(ref clips, clips.Length + 1);
            System.Array.Copy(clips, index, clips, index + 1, clips.Length - index - 1);
            clips[index] = null;
#if UNITY_EDITOR
            isPlayerDataDirty = true;
#endif
        }

        /// <summary>
        /// Remove a source slot from sources array.
        /// </summary>
        public void RemoveSource(int index)
        {
            System.Array.Copy(clips, index + 1, clips, index, clips.Length - index - 1);
            System.Array.Resize<AudioClip>(ref clips, clips.Length - 1);
#if UNITY_EDITOR
            isPlayerDataDirty = true;
#endif
        }
    }
}