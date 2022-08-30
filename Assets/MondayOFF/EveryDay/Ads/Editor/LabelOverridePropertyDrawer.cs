using UnityEngine;
using UnityEditor;

namespace MondayOFF {
    [CustomPropertyDrawer(typeof(LabelOverrideAttribute))]
    public class LabelOverridePropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            try {
                var propertyAttribute = this.attribute as LabelOverrideAttribute;
                if (!IsArray(property)) {
                    label.text = propertyAttribute.displayName;
                } else {
                    Debug.LogWarningFormat(
                        "{0}(\"{1}\") doesn't support arrays ",
                        typeof(LabelOverrideAttribute).Name,
                        propertyAttribute.displayName
                    );
                }
                EditorGUI.PropertyField(position, property, label);
            } catch (System.Exception ex) { Debug.LogException(ex); }
        }

        bool IsArray(SerializedProperty property) {
            string path = property.propertyPath;
            int idot = path.IndexOf('.');
            if (idot == -1) return false;
            string propName = path.Substring(0, idot);
            SerializedProperty p = property.serializedObject.FindProperty(propName);
            return p.isArray;
        }
    }
}