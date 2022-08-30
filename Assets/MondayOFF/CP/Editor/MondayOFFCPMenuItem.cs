using UnityEngine;
using UnityEditor;

namespace MondayOFF {
    public class MondayOFFCPMenuItem {
        [MenuItem("MondayOFF/Set CP Videos and Url", false, 200)]
        static void FocusCPVideoList() {
            var cpVidList = AssetDatabase.FindAssets("t:CPVideoList");

            if (cpVidList.Length != 1) {
                Debug.LogError("There are zero or more than two Objects! " + cpVidList.Length);
                return;
            }

            UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath<CPVideoList>(AssetDatabase.GUIDToAssetPath(cpVidList[0]));
        }
    }
}