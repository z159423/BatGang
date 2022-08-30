#if UNITY_EDITOR && UNITY_ANDROID
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class MondayOFFPrebuildAndroid : IPreprocessBuildWithReport {

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report) {
        // WARNING! Sets Keystore password
        PlayerSettings.keystorePass = "MondayOFF!@";
        PlayerSettings.keyaliasPass = "MondayOFF!@";
    }
}
#endif
