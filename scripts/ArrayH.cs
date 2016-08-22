using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Array helper methods.
    /// </summary>
    public class ArrayH
    {
        /// <summary>
        /// Fill each item with its index.
        /// </summary>
        public static void FillIndices(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                array[i] = i;
            }
        }

        /// <summary>
        /// Shuffle array content.
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int r = Random.Range(0, i);
                T tmp = array[i];
                array[i] = array[r];
                array[r] = tmp;
            }
        }

        /// <summary>
        /// Push element at the end.
        /// </summary>
        public static void Push<T>(ref T[] array, T element)
        {
            int index = array.Length;
            System.Array.Resize<T>(ref array, index + 1);
            array[index] = element;
        }

        /// <summary>
        /// Insert element at a specific index.
        /// </summary>
        public static void Insert<T>(ref T[] array, int index, T element)
        {
            System.Array.Resize<T>(ref array, array.Length + 1);
            System.Array.Copy(array, index, array, index + 1, array.Length - index - 1);
            array[index] = element;
        }

        /// <summary>
        /// Remove element at a specific index and shrink array.
        /// </summary>
        public static void Remove<T>(ref T[] array, int index)
        {
            if (index < array.Length - 1)
            {
                int len = array.Length - index - 1;
                System.Array.Copy(array, index + 1, array, index, len);
            }
            System.Array.Resize<T>(ref array, array.Length - 1);
        }

        /// <summary>
        /// Resize array to 0.
        /// </summary>
        public static void Clear<T>(ref T[] array)
        {
            System.Array.Resize<T>(ref array, 0);
        }

        /// <summary>
        /// Swap two elements.
        /// </summary>
        public static void Swap<T>(ref T[] array, int firstIndex, int secondIndex)
        {
            T tmpSecond = array[secondIndex];
            array[secondIndex] = array[firstIndex];
            array[firstIndex] = tmpSecond;
        }
    }
}
