using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Play sounds choosen randomly, at random times.
    /// </summary>
    public class LayerPlayerRandom : LayerPlayer
    {
        /// <summary>
        /// Duration of each clip. Used for random timing generation.
        /// </summary>
        private float[] _clipDurations = null;

        /// <summary>
        /// Pool providing random values.
        /// </summary>
        private RandomPool _randomPool = null;

        /// <summary>
        /// Elapsed time since last random timings generation.
        /// </summary>
        private float _elapsedTime = 0.0f;

        /// <summary>
        /// Index hint for triggering sound events.
        /// </summary>
        private int _nextEventIndex = 0;

        /// <summary>
        /// Number of remaining events to trigger.
        /// </summary>
        private int _eventCount = 0;

        /// <summary>
        /// Planned events.
        /// </summary>
        private RandomSoundPlanner.Result[] _events = new RandomSoundPlanner.Result[RandomSoundPlanner.MAX_COUNT];

        /// <summary>
        /// Constructor.
        /// </summary>
        public LayerPlayerRandom(LayerData layer, AudioSource source) : base(layer, source)
        {
            source.loop = false;
            CreateBuffers();
            ComputeNextEvents();
        }

        /// <summary>
        /// Don't do anything, source.Play() will be called from Update().
        /// </summary>
        public override void Play()
        {
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        public override void Stop()
        {
            _source.Stop();
        }

        /// <summary>
        /// Trigger new sounds.
        /// </summary>
        public override void Update(float dt)
        {
#if UNITY_EDITOR
            EditorUpdate();
#endif

            // If reached the end of the "planned" period, compute next one.
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _layer.period)
            {
                _elapsedTime -= _layer.period;
                ComputeNextEvents();
            }

            // Trigger next event if needed.
            if (_eventCount > 0 && _nextEventIndex < _eventCount)
            {
                float nextTime = _events[_nextEventIndex].startTime;
                if (nextTime <= _elapsedTime)
                {
                    PlayEvent(_events[_nextEventIndex].id, _events[_nextEventIndex].pitch);
                    ++_nextEventIndex;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Handle data changes to allow runtime edition.
        /// </summary>
        private void EditorUpdate()
        {
            // If a clip has been added/deleted
            if (_layer.clips.Length != _clipDurations.Length)
            {
                CreateBuffers(); // Re-generate buffers
            }
            else // Because a clip can change, we still need to update cached durations
            {
                CacheSourceDurations();
            }

            // Data changed
            if (_layer.isPlayerDataDirty)
            {
                Stop();
                _elapsedTime = 0.0f;
                _randomPool.SetNoRepeatCount(_layer.noRepeatCount);
                ComputeNextEvents();
            }

            // mixer
            _source.outputAudioMixerGroup = _layer.mixerGroup;
        }
#endif

        /// <summary>
        /// Creates clip's duration cache and a RandomPool.
        /// </summary>
        private void CreateBuffers()
        {
            // Extract clip durations
            _clipDurations = new float[_layer.clips.Length];
            CacheSourceDurations();

            // Init random pool
            _randomPool = new RandomPool(_layer.clips.Length, _layer.noRepeatCount);
        }

        /// <summary>
        /// Creates clip's duration cache.
        /// </summary>
        private void CacheSourceDurations()
        {
            for (int i = 0; i < _layer.clips.Length; ++i)
            {
                float d = (_layer.clips[i] != null) ? _layer.clips[i].length : 0.0f;
                _clipDurations[i] = d;
            }
        }

        /// <summary>
        /// Compute next sound events.
        /// </summary>
        private void ComputeNextEvents()
        {
            int count = Random.Range(_layer.minCount, _layer.maxCount + 1);
            _eventCount = RandomSoundPlanner.Compute(count, _layer.silence, _layer.minPitch, _layer.maxPitch, _layer.period, _clipDurations, _randomPool, _events);
            _nextEventIndex = 0;
        }

        /// <summary>
        /// Play an AudioClip by index with a custom pitch.
        /// </summary>
        private void PlayEvent(int index, float pitch)
        {
            _source.pitch = pitch;
            _source.volume = Random.Range(_layer.minVol, _layer.maxVol);
            _source.panStereo = Random.Range(_layer.minPan, _layer.maxPan);
            _source.PlayOneShot(_layer.clips[index]);
        }
    }
}
