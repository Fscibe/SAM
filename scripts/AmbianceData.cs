using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Represents a sound ambiance asset. Agregates LayerData.
    /// </summary>
    [CreateAssetMenu(fileName = "SAMAmbiance", menuName = "SAM/Ambiance", order = 1)]
    public class AmbianceData : ScriptableObject
    {
#if UNITY_EDITOR
        public int selectedLayerIndex = -1;
#endif

        /// <summary>
        /// Layers part of this ambiance.
        /// </summary>
        public LayerData[] layers = new LayerData[0];

        /// <summary>
        /// Add layer at the of of layers list.
        /// </summary>
        public void AddLayer()
        {
            ArrayH.Push<LayerData>(ref layers, new LayerData());
        }

        /// <summary>
        /// Remove layer by index.
        /// </summary>
        public void RemoveLayer(int index)
        {
            ArrayH.Remove<LayerData>(ref layers, index);
        }

        /// <summary>
        /// Swap two layers. Do not perform bound check.
        /// </summary>
        public void SwapLayer(int firstIndex, int secondIndex)
        {
            ArrayH.Swap<LayerData>(ref layers, firstIndex, secondIndex);
        }
    }
}
