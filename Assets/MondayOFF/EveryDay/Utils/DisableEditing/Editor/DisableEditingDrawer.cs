using UnityEngine;
using UnityEditor;

namespace MondayOFF {
    [CustomPropertyDrawer(typeof(DisableEditing))]
    public class DisableEditingDrawer : PropertyDrawer {
        public override void OnGUI(Rect rect,
                                    SerializedProperty property,
                                    GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(rect, property, label);
            GUI.enabled = true;
        }
    }
}