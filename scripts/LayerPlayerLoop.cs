using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Loop a sound.
    /// </summary>
    public class LayerPlayerLoop : LayerPlayer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LayerPlayerLoop(LayerData layer, AudioSource source) : base(layer, source)
        {
            source.loop = true;
            source.clip = layer.clips[0];
        }

        /// <summary>
        /// Start playing.
        /// </summary>
        public override void Play()
        {
            _source.Play();
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        public override void Stop()
        {
            _source.Stop();
        }

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(float dt)
        {
#if UNITY_EDITOR
            EditorUpdate();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Handle data changes for runtime edition.
        /// </summary>
        private void EditorUpdate()
        {
            if(_layer.isPlayerDataDirty)
            {
                _source.clip = _layer.clips[0];
                _source.Play();
            }

            _source.volume = _layer.minVol;
            _source.pitch = _layer.minPitch;
            _source.panStereo = _layer.minPan;
            _source.outputAudioMixerGroup = _layer.mixerGroup;
        }
#endif
    }
}
