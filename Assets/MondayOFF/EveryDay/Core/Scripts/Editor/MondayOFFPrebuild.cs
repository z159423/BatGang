using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace MondayOFF {
    class MondayOFFPrebuild : IPreprocessBuildWithReport {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report) {
            // Make sure Sprite Atlasing is enabled?
            if ((int)UnityEditor.EditorSettings.spritePackerMode < (int)SpritePackerMode.BuildTimeOnlyAtlas) {
                UnityEditor.EditorSettings.spritePackerMode = SpritePackerMode.BuildTimeOnlyAtlas;
            }

            UnityEditor.PlayerSettings.SplashScreen.animationMode = UnityEditor.PlayerSettings.SplashScreen.AnimationMode.Static;
            UnityEditor.PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
            UnityEditor.PlayerSettings.SplashScreen.backgroundColor = new Color32(0x1E, 0x1E, 0x1E, 0xff);

        }
    }
}