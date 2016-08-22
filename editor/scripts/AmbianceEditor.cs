using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

namespace Sam
{
    /// <summary>
    /// Edit ambiance assets.
    /// </summary>
    public class AmbianceEditor : EditorWindow
    {
		public const int MAJOR_VERSION = 1;
        public const int MINOR_VERSION = 0;

        private AmbianceData _currentAmbiance;
        private string _currentAmbianceGUID = "";
        private AmbianceData _ambianceToOpen = null;

        private Vector2 _scrollPosition = Vector2.zero;
        private Vector2 _propertiesScrollPosition = Vector2.zero;
        private static bool _mustRepaint = false;
        
        private int _wantedSelectedLayerIndex = -1;
        private int _wantedRemoveLayerIndex = -1;
        private int _swapFrom = -1;
        private int _swapTo = -1;

        /// <summary>
        /// Create editor window.
        /// </summary>
        [MenuItem("Window/SAM")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(AmbianceEditor), false, "SAM " + MAJOR_VERSION.ToString() + "." + MINOR_VERSION.ToString());
        }

        /// <summary>
        /// Undo callback.
        /// </summary>
        private static void OnUndoPerformed()
        {
            AmbianceEditor._mustRepaint = true;
        }

        /// <summary>
        /// Enable.
        /// </summary>
        void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        /// <summary>
        /// Disable.
        /// </summary>
        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }

        /// <summary>
        /// Low rate update. Repaint editor if needed.
        /// </summary>
        void OnInspectorUpdate()
        {
            if(AmbianceEditor._mustRepaint)
            {
                Repaint();
                AmbianceEditor._mustRepaint = false;
            }
        }

        /// <summary>
        /// Changes current layer. Delayed to next 'Layout' event.
        /// </summary>
        void SetSelectedLayer(int index)
        {
            index = Mathf.Clamp(index, 0, _currentAmbiance.layers.Length - 1);
            _wantedSelectedLayerIndex = index;
            GUI.FocusControl(null);
            Repaint();
        }

        /// <summary>
        /// Remove layer and repaint.
        /// </summary>
        void RemoveLayer(int index)
        {
            _wantedRemoveLayerIndex = index;
            Repaint();
        }

        /// <summary>
        /// Main method.
        /// </summary>
        void OnGUI()
        {
            UpdateAssetDrop();
            ApplyDelayedActions();
            DrawEditor();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_currentAmbiance);
            }
        }

        /// <summary>
        /// Handle asset drag & drop.
        /// </summary>
        private void UpdateAssetDrop()
        {
            if (DragAndDrop.paths.Length > 0)
            {
                string assetPath = DragAndDrop.paths[0];
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        if ((AmbianceData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(AmbianceData)) != null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                        }
                        break;

                    case EventType.DragPerform:
                        AmbianceData newAmbiance = (AmbianceData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(AmbianceData));
                        if (newAmbiance != null)
                        {
                            _ambianceToOpen = newAmbiance;
                            DragAndDrop.AcceptDrag();
                        }
                        break;
                }
            }            
        }

        /// <summary>
        /// Apply delayed actions of last frame (select layer, remove layer, move layer, change current ambiance).
        /// (Because of immediate gui mode, the UI must stay the same until the next 'Layout' event).
        /// </summary>
        private void ApplyDelayedActions()
        {            
            if (Event.current.type == EventType.Layout)
            {
                if (_currentAmbiance)
                {
                    // Layer move
                    if (_swapFrom >= 0)
                    {
                        int length = _currentAmbiance.layers.Length;
                        _swapFrom = Mathf.Clamp(_swapFrom, 0, length - 1);
                        _swapTo = Mathf.Clamp(_swapTo, 0, length - 1);
                        if (_swapFrom != _swapTo)
                        {
                            _currentAmbiance.SwapLayer(_swapFrom, _swapTo);
                            _wantedSelectedLayerIndex = _swapTo;
                        }
                        _swapFrom = _swapTo = -1;
                    }

                    // Layer selection
                    if(_wantedSelectedLayerIndex >=0)
                    {
                        _currentAmbiance.selectedLayerIndex = _wantedSelectedLayerIndex;
                        _wantedSelectedLayerIndex = -1;
                    }

                    // Layer deletion
                    if (_wantedRemoveLayerIndex >= 0)
                    {
                        // if we delete the selected layer, select the previous
                        if (_wantedRemoveLayerIndex <= _currentAmbiance.selectedLayerIndex)
                        {
                            --_currentAmbiance.selectedLayerIndex;
                            GUI.FocusControl(null);
                        }

                        _currentAmbiance.RemoveLayer(_wantedRemoveLayerIndex);
                        _wantedRemoveLayerIndex = -1;
                    }
                }

                // Asset change
                if (_ambianceToOpen)
                {
                    if (_currentAmbiance)
                    {
                        Undo.ClearUndo(_currentAmbiance);
                    }
                    _currentAmbiance = _ambianceToOpen;
                    _currentAmbiance.selectedLayerIndex = _currentAmbiance.layers.Length > 0 ? 0 : -1;
                    _currentAmbianceGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_currentAmbiance));
                    _ambianceToOpen = null;
                }
            }
        }

        /// <summary>
        /// Draw editor UI.
        /// </summary>
        private void DrawEditor()
        {
            if (_currentAmbiance)
            {
                // Initialize styles
                Styles.Init();

                // Clean dirty flags
                for (int i = 0; i < _currentAmbiance.layers.Length; ++i)
                {
                    LayerData layer = _currentAmbiance.layers[i];
                    layer.isPlayerDataDirty = false;
                    layer.isPlayerTypeDirty = false;
                }

                // Undo system
                Undo.RecordObject(_currentAmbiance, "Edit ambiance");

                // Show ambiance editor            
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    GUILayout.Label(AssetDatabase.GUIDToAssetPath(_currentAmbianceGUID), EditorStyles.label);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        DrawLayersSidebar(_currentAmbiance);
                        DrawLayerProperties(_currentAmbiance);
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Drag & Drop an ambiance file here.");
            }
        }

        /// <summary>
        /// Draw sidebar containing the list of layers.
        /// </summary>
        private void DrawLayersSidebar(AmbianceData data)
        {
            // Sound layers bar: where we create/select layer to edit
            Color lastBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Styles.sidebarBackgroundColor;
            GUIStyle style = new GUIStyle();
            style.normal.background = EditorGUIUtility.whiteTexture;
            GUILayout.BeginVertical(style, GUILayout.Width(200));
            {
                GUI.backgroundColor = lastBackgroundColor;
                this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition, false, true);
                {
                    LayerData[] layers = data.layers;
                    for (int i = 0; i < layers.Length; ++i)
                    {
                        LayerData layer = layers[i];
                        string name = (layer.name.Length > 0) ? layer.name : "unnamed";
                        LayerWidget.Result result = LayerWidget.Draw(name, i == _currentAmbiance.selectedLayerIndex, layer.mute, layer.solo, GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(58.0f));
                        switch(result)
                        {
                            case LayerWidget.Result.SELECT:
                                SetSelectedLayer(i);
                                break;

                            case LayerWidget.Result.MOVE_UP:
                                _swapFrom = i;
                                _swapTo = i - 1;
                                break;

                            case LayerWidget.Result.MOVE_DOWN:
                                _swapFrom = i;
                                _swapTo = i + 1;
                                break;

                            case LayerWidget.Result.DELETE:
                                RemoveLayer(i);
                                break;

                            case LayerWidget.Result.MUTE:
                                layer.mute = !layer.mute;
                                break;

                            case LayerWidget.Result.SOLO:
                                layer.solo = !layer.solo;
                                break;
                        }
                    }
                    if (GUILayout.Button("add layer"))
                    {
                        data.AddLayer();
                        SetSelectedLayer(data.layers.Length - 1);
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUI.backgroundColor = lastBackgroundColor;
        }

        /// <summary>
        /// Draw properties of selected layer.
        /// </summary>
        private void DrawLayerProperties(AmbianceData data)
        {
            if (_currentAmbiance.selectedLayerIndex < 0)
            {
                GUILayout.Label("Please select a layer in the left panel.");
            }
            else
            {
                LayerData layer = data.layers[_currentAmbiance.selectedLayerIndex];
                GUILayout.BeginVertical();
                {
                    this._propertiesScrollPosition = GUILayout.BeginScrollView(this._propertiesScrollPosition, false, true);

                    // General
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("General", Styles.titleStyle);
                    layer.name = EditorGUILayout.TextField("Name", layer.name);
                    layer.mixerGroup = (AudioMixerGroup)EditorGUILayout.ObjectField("Mixer Group", layer.mixerGroup, typeof(AudioMixerGroup), false);
                    PlayerType newPlayerType = (PlayerType)EditorGUILayout.EnumPopup("Type", layer.playerType);
                    layer.isPlayerTypeDirty = layer.playerType != newPlayerType;
                    layer.playerType = newPlayerType;

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Variation", Styles.titleStyle);

                    switch (layer.playerType)
                    {
                        case PlayerType.Random:
                            float newMinVol = EditorGUILayout.Slider(new GUIContent("Volume min", "Minimum value for random volume."), layer.minVol, 0.0f, 1.0f);
                            float newMaxVol = EditorGUILayout.Slider(new GUIContent("Volume max", "Maximum value for random volume."), layer.maxVol, 0.0f, 1.0f);
                            EditorH.MinMaxClamp(newMinVol, newMaxVol, ref layer.minVol, ref layer.maxVol);

                            float newMinPitch = EditorGUILayout.Slider(new GUIContent("Pitch min", "Minimum value for random pitch."), layer.minPitch, 0.0f, 10.0f);
                            float newMaxPitch = EditorGUILayout.Slider(new GUIContent("Pitch max", "Maximum value for random pitch."), layer.maxPitch, 0.0f, 10.0f);
                            EditorH.MinMaxClamp(newMinPitch, newMaxPitch, ref layer.minPitch, ref layer.maxPitch);

                            float newMinPan = EditorGUILayout.Slider(new GUIContent("Pan min", "Minimum value for random pan."), layer.minPan, -1.0f, 1.0f);
                            float newMaxPan = EditorGUILayout.Slider(new GUIContent("Pan max", "Maximum value for random pan."), layer.maxPan, -1.0f, 1.0f);
                            EditorH.MinMaxClamp(newMinPan, newMaxPan, ref layer.minPan, ref layer.maxPan);
                            break;

                        case PlayerType.Loop:
                            layer.minVol = EditorGUILayout.Slider("Volume", layer.minVol, 0.0f, 1.0f);
                            layer.maxVol = Mathf.Max(layer.minVol, layer.maxVol);
                            layer.minPitch = EditorGUILayout.Slider("Pitch", layer.minPitch, 0.0f, 10.0f);
                            layer.maxPitch = Mathf.Max(layer.minPitch, layer.maxPitch);
                            layer.minPan = EditorGUILayout.Slider("Pan", layer.minPan, -1.0f, 1.0f);
                            layer.maxPan = Mathf.Max(layer.minPan, layer.maxPan);
                            break;
                    }

                    // Play data
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Clips", Styles.titleStyle);
                    switch (layer.playerType)
                    {
                        case PlayerType.Random:
                            {
                                int minCount = EditorGUILayout.DelayedIntField(new GUIContent("Count min", "Minimum number of sounds triggered in time period."), layer.minCount);
                                int maxCount = EditorGUILayout.DelayedIntField(new GUIContent("Count max", "Maximum number of sounds triggered in time period."), layer.maxCount);
                                minCount = Mathf.Clamp(minCount, 0, RandomSoundPlanner.MAX_COUNT);
                                maxCount = Mathf.Clamp(maxCount, 0, RandomSoundPlanner.MAX_COUNT);
                                if (minCount != layer.minCount) maxCount = Mathf.Max(minCount, maxCount);
                                if (maxCount != layer.maxCount) minCount = Mathf.Min(minCount, maxCount);
                                EditorH.Assign(ref layer.minCount, minCount, ref layer.isPlayerDataDirty);
                                EditorH.Assign(ref layer.maxCount, maxCount, ref layer.isPlayerDataDirty);

                                float period = EditorGUILayout.DelayedFloatField(new GUIContent("Period (seconds)", "Period during which N sounds will be triggerd. With 'Count min' <= N <= 'Count max'."), layer.period);
                                EditorH.Assign(ref layer.period, period, ref layer.isPlayerDataDirty);

                                int noRepeatCount = EditorGUILayout.DelayedIntField(new GUIContent("No repeat", "Avoid repeating the last N sounds played."), layer.noRepeatCount);
                                EditorH.Assign(ref layer.noRepeatCount, noRepeatCount, ref layer.isPlayerDataDirty);

                                float silence = EditorGUILayout.DelayedFloatField(new GUIContent("Silence (seconds)", "Impose a silence after each sound."), layer.silence);
                                EditorH.Assign(ref layer.silence, silence, ref layer.isPlayerDataDirty);
                                                                
                                for (int i = 0; i < layer.clips.Length; ++i)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        layer.clips[i] = (AudioClip)EditorGUILayout.ObjectField("Clip " + i.ToString(), layer.clips[i], typeof(AudioClip), false);
                                        GUI.enabled = (layer.clips.Length > 1);
                                        if (GUILayout.Button("-", Styles.miniButtonStyle, GUILayout.Width(16.0f), GUILayout.Height(16.0f)))
                                        {
                                            layer.RemoveSource(i);
                                        }
                                        GUI.enabled = true;
                                        if (GUILayout.Button("+", Styles.miniButtonStyle, GUILayout.Width(16.0f), GUILayout.Height(16.0f)))
                                        {
                                            layer.AddSource(i+1);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                break;
                            }
                            
                        case PlayerType.Loop:
                            {
                                AudioClip newClip = (AudioClip)EditorGUILayout.ObjectField("Clip 0", layer.clips[0], typeof(AudioClip), false);
                                EditorH.Assign(ref layer.clips[0], newClip, ref layer.isPlayerDataDirty);
                                break;
                            }
                    }

                    EditorGUILayout.Space();
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                layer.Validate();
            }
        }
    }
}
