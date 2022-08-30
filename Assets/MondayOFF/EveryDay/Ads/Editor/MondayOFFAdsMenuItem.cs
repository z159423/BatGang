using UnityEngine;
using UnityEditor;

namespace MondayOFF {
    public class MondayOFFAdsMenuItems {
        [MenuItem("MondayOFF/Set Ad Unit IDs", false, 100)]
        static void FocusAdUnitIDs() {
            var adUnitIDs = AssetDatabase.FindAssets("t:MondayOFFAdUnitID");

            if (adUnitIDs.Length != 1) {
                Debug.LogError("There are zero or more than two Objects! " + adUnitIDs.Length);
                return;
            }

            UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath<MondayOFFAdUnitID>(AssetDatabase.GUIDToAssetPath(adUnitIDs[0]));
        }
    }
}