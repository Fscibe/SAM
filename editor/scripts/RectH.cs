using UnityEngine;

namespace Sam
{
    /// <summary>
    /// General helpers for manipulating Rect.
    /// </summary>
    public static class RectH
    {
        /// <summary>
        /// Get rect inside another rect.
        /// </summary>
        public static Rect Inset(Rect model, float margin)
        {
            return new Rect(model.x + margin, model.y + margin, model.width - 2 * margin, model.height - 2 * margin);
        }

        /// <summary>
        /// Get rect inside another rect.
        /// </summary>
        public static Rect Inset(Rect model, float topMargin, float rightMargin, float bottomMargin, float leftMargin)
        {
            return new Rect(model.x + leftMargin, model.y + topMargin, model.width - (leftMargin + rightMargin), model.height - (topMargin + bottomMargin));
        }

        /// <summary>
        /// Extend a given rect at the bottom.
        /// </summary>
        public static Rect ExtendBottom(Rect model, float height)
        {
            return new Rect(model.x, model.y + model.height, model.width, height);
        }
    }
}
