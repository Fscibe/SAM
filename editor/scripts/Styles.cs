using UnityEngine;
using UnityEditor;

namespace Sam
{
    /// <summary>
    /// LayerWidget custom styles.
    /// </summary>
    public class LayerWidgetStyle
    {
        public GUIStyle title;
        public GUIStyle header;
        public GUIStyle footer;
        public GUIStyle activeButton;

        private Color _activeHeaderColorFree = new Color(0.79f, 0.79f, 0.79f);
        private Color _activeHeaderColorPro = new Color(0.79f, 0.79f, 0.79f);
        public Color activeHeaderColor { get { return EditorGUIUtility.isProSkin ? _activeHeaderColorPro : _activeHeaderColorFree; } }

        private Color _activeButtonColorFree = new Color(0.4f, 0.4f, 0.4f);
        private Color _activeButtonColorPro = new Color(0.4f, 0.4f, 0.4f);
        public Color activeButtonColor { get { return EditorGUIUtility.isProSkin ? _activeButtonColorPro : _activeButtonColorFree; } }

        private Color _footerColorFree = new Color(0.59f, 0.59f, 0.59f);
        private Color _footerColorPro = new Color(0.35f, 0.35f, 0.35f);
        public Color footerColor { get { return EditorGUIUtility.isProSkin ? _footerColorPro : _footerColorFree; } }

        private Color _headerColorFree = new Color(0.72f, 0.72f, 0.72f);
        private Color _headerColorPro = new Color(0.62f, 0.62f, 0.62f);
        public Color headerColor { get { return EditorGUIUtility.isProSkin ? _headerColorPro : _headerColorFree; } }

        public void Init()
        {
            Texture2D headerTexture = AssetH.FindAndLoad("sam_editor_header");
            Texture2D footerTexture = AssetH.FindAndLoad("sam_editor_footer");
            
            activeButton = new GUIStyle(GUI.skin.button);
            activeButton.normal.textColor = new Color(0.34f, 0.59f, 0.98f, 1.0f);
            activeButton.fontStyle = FontStyle.Bold;
            title = new GUIStyle(GUI.skin.label);
            title.alignment = TextAnchor.MiddleCenter;
            header = new GUIStyle(GUI.skin.box);
            header.border = new RectOffset(3, 3, 3, 3);
            header.normal.background = headerTexture;
            header.alignment = TextAnchor.MiddleLeft;
            header.fontStyle = FontStyle.Bold;
            footer = new GUIStyle(GUI.skin.box);
            footer.border = new RectOffset(3, 3, 3, 3);
            footer.normal.background = footerTexture;
        }
    }

    /// <summary>
    /// Editor custom styles, colors and icons.
    /// <summary>
    public class Styles
    {
        private static bool _initialized = false;

        // Colors
        public static Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);
        public static Color sidebarBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
        public static Color selectionColor = new Color(0.24f, 0.49f, 0.90f, 1.0f);

        // Styles
        public static GUIStyle titleStyle;
        public static GUIStyle shadowStyle;
        public static GUIStyle selectionStyle;
        public static GUIStyle miniButtonStyle;
        public static LayerWidgetStyle layerWidgetStyle;

        // UTF String Icons
        public static string utfUp = "\u25b2";
        public static string utfDown = "\u25bc";
        public static string utfDelete = "\u2715";

        /// <summary>
        /// Creates all styles.
        /// </summary>
        public static void Init()
        {
            if(!_initialized)
            {
                // Specific styles
                layerWidgetStyle = new LayerWidgetStyle();
                layerWidgetStyle.Init();

                // General styles
                Texture2D shadowTexture = AssetH.FindAndLoad("sam_editor_dot_soft");
                Texture2D selectionTexture = AssetH.FindAndLoad("sam_editor_dot_hard");
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontStyle = FontStyle.Bold;
                shadowStyle = new GUIStyle(GUI.skin.box);
                shadowStyle.border = new RectOffset(3, 3, 3, 3);
                shadowStyle.normal.background = shadowTexture;
                selectionStyle = new GUIStyle(GUI.skin.box);
                selectionStyle.border = new RectOffset(3, 3, 3, 3);
                selectionStyle.normal.background = selectionTexture;
                miniButtonStyle = new GUIStyle(GUI.skin.button);
                miniButtonStyle.fontSize = 10;
                miniButtonStyle.alignment = TextAnchor.MiddleCenter;
                miniButtonStyle.padding = new RectOffset(0, 0, 0, 0);

                // Initialization flag
                _initialized = true;
            }
        }
    }
}
