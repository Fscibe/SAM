using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Base class for LayerData playback controllers.
    /// </summary>
    public abstract class LayerPlayer
    {
        /// <summary>
        /// Associated LayerData.
        /// </summary>
        protected LayerData _layer = null;

        /// <summary>
        /// Associated AudioSource.
        /// </summary>
        protected AudioSource _source = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LayerPlayer(LayerData layer, AudioSource source)
        {
            this._layer = layer;
            this._source = source;
            source.playOnAwake = false;
            source.spatialBlend = 0.0f;
            source.outputAudioMixerGroup = layer.mixerGroup;
        }

        /// <summary>
        /// Play.
        /// </summary>
        abstract public void Play();

        /// <summary>
        /// Stop.
        /// </summary>
        abstract public void Stop();

        /// <summary>
        /// Update.
        /// </summary>
        abstract public void Update(float dt);

#if UNITY_EDITOR
        /// <summary>
        /// Access LayerData.
        /// </summary>
        public LayerData GetLayer()
        {
            return _layer;
        }

        /// <summary>
        /// Access AudioSource.
        /// </summary>
        public AudioSource GetSource()
        {
            return _source;
        }
#endif
    }
}
