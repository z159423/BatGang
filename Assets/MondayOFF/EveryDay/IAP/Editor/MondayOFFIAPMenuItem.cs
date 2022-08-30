#if UNITY_PURCHASING
using UnityEngine;
using UnityEditor;

namespace MondayOFF {
    public class MondayOFFMenuItems {
        [MenuItem("MondayOFF/Set In-App Purchase Products", false, 300)]
        static void FocusProducts() {
            var products = AssetDatabase.FindAssets("t:MondayOFFProducts");

            if (products.Length != 1) {
                Debug.LogError("There are zero or more than two Objects! " + products.Length);
                return;
            }

            UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath<MondayOFFProducts>(AssetDatabase.GUIDToAssetPath(products[0]));
        }
    }
}
#endif