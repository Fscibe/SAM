using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Stack rectangles vertically.
    /// </summary>
    public class ColumnH
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ColumnH(Rect controlRect, float margin = 0.0f)
        {
            this._controlRect = controlRect;
            this._currentY = controlRect.y;
            this._margin = margin;
        }

        /// <summary>
        /// Add fixed height rectangle.
        /// </summary>
        public Rect Grow(float height)
        {
            Rect ret = new Rect(_controlRect.x, _currentY, _controlRect.width, height);
            this._currentY += height + _margin;
            return ret;
        }

        /// <summary>
        /// Add rectangle filling remaining space.
        /// </summary>
        public Rect Fill()
        {
            float height = _controlRect.yMax - _currentY;
            return Grow(height);
        }

        private Rect _controlRect;
        private float _currentY;
        private float _margin;
    }
}
