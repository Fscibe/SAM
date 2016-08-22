using UnityEngine;

namespace Sam
{
    /// <summary>
    /// General purpose editor helpers.
    /// </summary>
    public static class EditorH
    {
        /// <summary>
        /// Clamp between min, max and makes sure newMin is never superior to newMax.
        /// </summary>
        public static void MinMaxClamp(float newMin, float newMax, ref float min, ref float max)
        {
            if (min != newMin) newMax = Mathf.Max(newMin, newMax);
            if (max != newMax) newMin = Mathf.Min(newMin, newMax);
            min = newMin;
            max = newMax;
        }

        /// <summary>
        /// Assign and set the dirty flag to TRUE if value changed.
        /// </summary>
        public static void Assign<T>(ref T destination, T value, ref bool dirtyFlag) where T : Object
        {
            dirtyFlag |= (destination != value);
            destination = value;
        }

        /// <summary>
        /// Assign and set the dirty flag to TRUE if value changed.
        /// </summary>
        public static void Assign(ref int destination, int value, ref bool dirtyFlag)
        {
            dirtyFlag |= (destination != value);
            destination = value;
        }

        /// <summary>
        /// Assign and set the dirty flag to TRUE if value changed.
        /// </summary>
        public static void Assign(ref float destination, float value, ref bool dirtyFlag)
        {
            dirtyFlag |= (destination != value);
            destination = value;
        }
    }
}
