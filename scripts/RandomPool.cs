using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Pool providing random numbers and avoiding repetitions.
    /// </summary>
    public class RandomPool
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RandomPool(int totalCount, int noRepeatCount)
        {
            this._indexArray = new int[totalCount];
            SetNoRepeatCount(noRepeatCount);
            ArrayH.FillIndices(this._indexArray);
            ArrayH.Shuffle<int>(this._indexArray);
        }

        /// <summary>
        /// Set the minimal number of times required to get the same value twice using GetValue().
        /// </summary>
        public void SetNoRepeatCount(int count)
        {
            this._noRepeatCount = count;
            this._pickRange = this._indexArray.Length - _noRepeatCount;
            if (this._pickRange < 1) this._pickRange = 1;
        }

        /// <summary>
        /// Get a random value.
        /// </summary>
        public int GetValue()
        {
            // Pick
            int pickedIndex = Random.Range(0, _pickRange);
            int value = _indexArray[pickedIndex];

            // Rotate values
            if (_noRepeatCount > 0)
            {
                System.Array.Copy(_indexArray, pickedIndex + 1, _indexArray, pickedIndex, _indexArray.Length - 1 - pickedIndex);
                _indexArray[_indexArray.Length - 1] = value;
            }
            return value;
        }

        /// <summary>
        /// Array of indices to pick.
        /// </summary>
        private int[] _indexArray;

        /// <summary>
        /// Range in which indices can be picked.
        /// </summary>
        private int _pickRange;

        /// <summary>
        /// Minimum number of time GetValue() must be called before we can get the same value again.
        /// ex: noRepeatCount=1 means a value can not be picked twice.
        /// </summary>
        private int _noRepeatCount;
    }
}
