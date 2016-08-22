using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Layer widget with name, move, delete, mute and solo buttons.
    /// </summary>
    public static class LayerWidget
    {
        /// <summary>
        /// LayerWidget events.
        /// </summary>
        public enum Result
        {
            NONE,
            MUTE,
            SOLO,
            DELETE,
            MOVE_UP,
            MOVE_DOWN,
            SELECT
        }

        /// <summary>
        /// Draw a layer widget.
        /// </summary>
        public static Result Draw(string text, bool selected, bool mute, bool solo, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, style, options);
            return Draw(position, text, selected, mute, solo);
        }

        /// <summary>
        /// Draw a layer widget.
        /// </summary>
        public static Result Draw(Rect controlRect, string text, bool selected, bool mute, bool solo)
        {
            Result result = Result.NONE;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            LayerWidgetStyle style = Styles.layerWidgetStyle;

            // store initial colors
            Color prevColor = GUI.color;
            Color prevBackgroundColor = GUI.backgroundColor;

            // draw shadow
            GUI.color = Styles.shadowColor;
            float SHADOW_DY = 4.0f;
            Rect shadowRect = RectH.Inset(controlRect, SHADOW_DY, 0, 0, 0);
            GUI.Box(shadowRect, "", Styles.shadowStyle);

            // draw selection halo
            if(selected)
            {
                GUI.color = Styles.selectionColor;
                GUI.Box(controlRect, "", Styles.selectionStyle);
            }

            // draw header
            ColumnH contentColumn = new ColumnH(RectH.Inset(controlRect, 2.0f));
            GUI.color = selected ? style.activeHeaderColor : style.headerColor;
            float HEADER_HEIGHT = 24.0f;
            Rect headerRect = contentColumn.Grow(HEADER_HEIGHT);
            GUI.Box(headerRect, text, style.header);

            // draw footer
            GUI.color = style.footerColor;
            Rect footerRect = contentColumn.Fill();
            GUI.Box(footerRect, "", style.footer);

            // draw minibuttons (up, down, delete)
            GUI.color = style.headerColor;
            float MINI_BTN_MARGIN = 5.0f;
            float MINI_BTN_SIZE = headerRect.height - 2.0f * MINI_BTN_MARGIN;
            RowH headerRow = new RowH(RectH.Inset(headerRect, MINI_BTN_MARGIN), MINI_BTN_MARGIN, false);
            
            if (GUI.Button(headerRow.Grow(MINI_BTN_SIZE), Styles.utfDelete, Styles.miniButtonStyle))
            {
                result = Result.DELETE;
            }
            if (GUI.Button(headerRow.Grow(MINI_BTN_SIZE), Styles.utfDown, Styles.miniButtonStyle))
            {
                result = Result.MOVE_DOWN;
            }
            if (GUI.Button(headerRow.Grow(MINI_BTN_SIZE), Styles.utfUp, Styles.miniButtonStyle))
            {
                result = Result.MOVE_UP;
            }

            // draw solo/mute buttons
            GUI.color = style.footerColor;
            float TOOL_BTN_MARGIN = 5.0f;
            float TOOL_BTN_SIZE = footerRect.height - 2.0f * TOOL_BTN_MARGIN;
            RowH footerRow = new RowH(RectH.Inset(footerRect, TOOL_BTN_MARGIN), TOOL_BTN_MARGIN, false);
            
            GUI.color = Color.white;
            GUI.backgroundColor = mute ? style.activeButtonColor : style.headerColor;
            Rect muteBtnRect = footerRow.Grow(TOOL_BTN_SIZE);
            if (GUI.Button(muteBtnRect, new GUIContent("M"), mute ? style.activeButton : GUI.skin.button))
            {
                result = Result.MUTE;
            }

            GUI.color = Color.white;
            GUI.backgroundColor = solo ? style.activeButtonColor : style.headerColor;
            Rect soloBtnRect = footerRow.Grow(TOOL_BTN_SIZE);
            if (GUI.Button(soloBtnRect, "S", solo ? style.activeButton : GUI.skin.button))
            {
                result = Result.SOLO;
            }

            // select if no button inside was hit
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (result == Result.NONE &&
                        controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        result = Result.SELECT;
                    }
                    break;
            }

            // restore initial colors
            GUI.backgroundColor = prevBackgroundColor;
            GUI.color = prevColor;
            
            return result;
        }
    }
}
