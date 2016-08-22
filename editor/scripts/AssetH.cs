using UnityEngine;
using UnityEditor;

namespace Sam
{
    /// <summary>
    /// General helpers for assets.
    /// </summary>
    public static class AssetH
    {
        /// <summary>
        /// Find and load an asset by name.
        /// </summary>
        public static Texture2D FindAndLoad(string name)
        {
            Texture2D ret = null;
            string[] guids = AssetDatabase.FindAssets(name);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                ret = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            }
            else
            {
                Debug.LogError("Texture " + name + " not found.");
            }
            return ret;
        }
    }
}
