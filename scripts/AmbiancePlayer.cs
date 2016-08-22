using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Controls an ambiance asset playback.
    /// </summary>
    public class AmbiancePlayer : MonoBehaviour
    {
        /// <summary>
        /// If TRUE, Play() will be automatically called when component awakes.
        /// </summary>
        public bool playOnStart = true;

        /// <summary>
        /// The ambiance asset to be played.
        /// </summary>
        public AmbianceData ambiance = null;

        /// <summary>
        /// Players for each layer.
        /// </summary>
        private LayerPlayer[] _players = new LayerPlayer[0];

        /// <summary>
        /// Play state. Serialized for C# hot reload in play mode.
        /// </summary>
#if UNITY_EDITOR
        [HideInInspector]
        [SerializeField]
#endif
        private bool _isPlaying = false;

#if UNITY_EDITOR
        /// <summary>
        /// Access a LayerPlayer by index.
        /// </summary>
        public LayerPlayer GetPlayer(int index)
        {
            LayerPlayer player = null;
            if (_players != null && _players.Length > index)
            {
                player = _players[index];
            }
            return player;
        }
#endif

        /// <summary>
        /// Create players at start.
        /// </summary>
        void Start()
        {
            CreatePlayers();
        }

        /// <summary>
        /// On enabled, resume playback if it was disabled while playing.
        /// </summary>
        void OnEnable()
        {
            if (_isPlaying)
            {
                Play();
            }
        }

        /// <summary>
        /// Stop all players but keep 'isPlaying' flag at current state, so OnEnable can resume playback.
        /// </summary>
        void OnDisable()
        {
            for (int i = 0; i < _players.Length; ++i)
            {
                _players[i].Stop();
            }
        }

        /// <summary>
        /// Player creation.
        /// </summary>
        private LayerPlayer CreatePlayer(LayerData layer, AudioSource source)
        {
            switch(layer.playerType)
            {
                case PlayerType.Random:
                    return new LayerPlayerRandom(layer, source);

                default:
                case PlayerType.Loop:
                    return new LayerPlayerLoop(layer, source);
            }
        }

        /// <summary>
        /// Player destruction.
        /// </summary>
        private void DestroyPlayer(LayerPlayer player)
        {
            Destroy(player.GetSource());
        }

        /// <summary>
        /// Creates players array. One player for each LayerData.
        /// </summary>
        private void CreatePlayers()
        {
            if (ambiance)
            {
                LayerData[] layers = ambiance.layers;
                System.Array.Resize<LayerPlayer>(ref _players, layers.Length);
                for (int i = 0; i < _players.Length; ++i)
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    _players[i] = CreatePlayer(layers[i], source);
                }

                if (playOnStart)
                {
                    Play();
                }
            }
        }

        /// <summary>
        /// Update players.
        /// </summary>
        void Update()
        {
            if(!ambiance)
            {
                return;
            }

#if UNITY_EDITOR
            EditorUpdate();
#endif

            if (_isPlaying)
            {
                float dt = Time.deltaTime;
                for (int i = 0; i < _players.Length; ++i)
                {
                    _players[i].Update(dt);
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Handles changes in data at runtime, so data can be edited in play mode.
        /// </summary>
        private void EditorUpdate()
        {
            // If PlayerType changed, create a new LayerPlayer.
            for (int i = 0; i < _players.Length; ++i)
            {
                LayerData layer = _players[i].GetLayer();
                if(layer.isPlayerTypeDirty)
                {
                    _players[i] = CreatePlayer(layer, _players[i].GetSource());
                    if (_isPlaying)
                    {
                        _players[i].Play();
                    }
                }
            }

            // If layer added by editor, add player for it.
            int layerCount = ambiance.layers.Length;
            for (int i = 0; i < layerCount; ++i)
            {
                if (i >= _players.Length || ambiance.layers[i] != _players[i].GetLayer())
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    LayerPlayer player = CreatePlayer(ambiance.layers[i], source);
                    ArrayH.Insert<LayerPlayer>(ref _players, i, player);
                    if (_isPlaying)
                    {
                        player.Play();
                    }
                }
            }

            // If layer removed by editor, remove its player.
            if (layerCount < _players.Length)
            {
                for (int i = layerCount; i < _players.Length; ++i)
                {
                    DestroyPlayer(_players[i]);
                }
                System.Array.Resize<LayerPlayer>(ref _players, layerCount);
            }

            // Update Solo & Mute states
            bool someSolo = false;
            for (int i = 0; i < _players.Length; ++i)
            {
                if (_players[i].GetLayer().solo)
                {
                    someSolo = true;
                    break;
                }
            }
            for (int i = 0; i < _players.Length; ++i)
            {
                LayerPlayer player = _players[i];
                LayerData data = _players[i].GetLayer();
                AudioSource source = player.GetSource();                
                bool isMuted = (!data.solo && (data.mute || someSolo));
                source.mute = isMuted;
            }
        }
#endif

        /// <summary>
        /// Play all.
        /// </summary>
        public void Play()
        {
            for (int i = 0; i < _players.Length; ++i)
            {
                _players[i].Play();
            }
            _isPlaying = true;
        }

        /// <summary>
        /// Stop all.
        /// </summary>
        public void Stop()
        {
            for (int i = 0; i < _players.Length; ++i)
            {
                _players[i].Stop();
            }
            _isPlaying = false;
        }
    }
}
