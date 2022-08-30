using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!ShowMe(property))
            return 0f;

        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (ShowMe(property)) {
            var drawIf = this.attribute as DrawIfAttribute;
            if (!string.IsNullOrEmpty(drawIf.displayName)) {
                label.text = $"\t{drawIf.displayName}";
            }
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private bool ShowMe(SerializedProperty property) {
        var drawIf = this.attribute as DrawIfAttribute;
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;

        var comparedField = property.serializedObject.FindProperty(path);

        if (comparedField == null) {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        return comparedField.boolValue;
    }
}
