using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Stack rectangles horizontally.
    /// </summary>
    public class RowH
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RowH(Rect controlRect, float margin = 0.0f, bool leftToRight = true)
        {
            this._controlRect = controlRect;
            this._currentX = leftToRight ? controlRect.x : controlRect.x + controlRect.width;
            this._margin = margin;
            this._leftToRight = leftToRight;
        }

        /// <summary>
        /// Add fixed size rectangle.
        /// </summary>
        public Rect Grow(float size)
        {
            float x = _leftToRight ? _currentX : _currentX - size;
            float dx = size + _margin;
            this._currentX += _leftToRight ? dx : -dx;
            return new Rect(x, _controlRect.y, size, _controlRect.height);
        }

        /// <summary>
        /// Add rectangle filling remaining space.
        /// </summary>
        public Rect Fill()
        {
            float size = _leftToRight ? _controlRect.xMax - _currentX : _controlRect.xMin - _currentX;
            return Grow(size);
        }

        private Rect _controlRect;
        private float _currentX;
        private float _margin;
        private bool _leftToRight;
    }
}
