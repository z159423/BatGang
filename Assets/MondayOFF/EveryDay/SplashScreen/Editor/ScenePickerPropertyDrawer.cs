using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
public class ScenePickerPropertyDrawer : PropertyDrawer {
    string[] options = new string[]
    {
     "Option1", "Option2", "Option3",
    };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        string[] sceneNames = new string[sceneNumber];

        for (int i = 0; i < sceneNumber; i++) {
            if (i == currentSceneIndex) {
                sceneNames[i] = "-- Current Scene --";
            } else {
                sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }
        }

        var currentSelection = property.intValue;
        property.intValue = EditorGUI.Popup(position, "Scene to transfer", currentSelection, sceneNames);

        if (property.intValue == currentSceneIndex) {
            if (sceneNumber > 1) {
                property.intValue = (property.intValue + 1) % sceneNumber;
            } else {
                property.intValue = -1;
            }
        }
    }
}
