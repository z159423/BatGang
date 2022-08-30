using UnityEngine;
using MondayOFF;

[ExecuteInEditMode]
public class PlayOnTestScript : MonoBehaviour {
    [Header("Screen Scale Factor for DPI and stuff..")]
    [Tooltip("3 is scale factor for newer iPhones but 4 seems to reflect correct presentation")]
    [SerializeField][Range(1, 6)] float _scaleFactor = 4;
    MondayOFFAdUnitID _adUnitIDs = default;

    private void OnGUI() {
        if (_adUnitIDs.hasPlayOn) {
            var anchor = _adUnitIDs.playOnLogoAnchor;
            float anchorSize = 20f;
            Vector2 anchorOffset = new Vector2(0, 0);

            var position = (Vector2)_adUnitIDs.playOnLogoOffset * _scaleFactor;
            var logoSize = _adUnitIDs.playOnLogoSize * _scaleFactor;

            switch (anchor) {
                // skip if top left
                // case PlayOnSDK.Position.TopLeft:
                // break;
                case PlayOnSDK.Position.TopCenter:
                    position.x += Screen.width / 2 - logoSize / 2;
                    anchorOffset.x = logoSize / 2 - anchorSize / 2;
                    break;
                case PlayOnSDK.Position.TopRight:
                    position.x += Screen.width - logoSize;
                    anchorOffset.x = logoSize - anchorSize;
                    break;
                case PlayOnSDK.Position.CenterLeft:
                    position.y += Screen.height / 2 - logoSize / 2;
                    anchorOffset.y = logoSize / 2 - anchorSize / 2;
                    break;
                case PlayOnSDK.Position.Centered:
                    position.x += Screen.width / 2 - logoSize / 2;
                    position.y += Screen.height / 2 - logoSize / 2;
                    anchorOffset.x = logoSize / 2 - anchorSize / 2;
                    anchorOffset.y = logoSize / 2 - anchorSize / 2;
                    break;
                case PlayOnSDK.Position.CenterRight:
                    position.x += Screen.width - logoSize;
                    position.y += Screen.height / 2 - logoSize / 2;
                    anchorOffset.x = logoSize - anchorSize;
                    anchorOffset.y = logoSize / 2 - anchorSize / 2;
                    break;
                case PlayOnSDK.Position.BottomLeft:
                    position.y += Screen.height - logoSize;
                    anchorOffset.y = logoSize - anchorSize;
                    break;
                case PlayOnSDK.Position.BottomCenter:
                    position.x += Screen.width - logoSize;
                    position.y += Screen.height - logoSize;
                    anchorOffset.x = logoSize - anchorSize;
                    anchorOffset.y = logoSize - anchorSize;
                    break;
                case PlayOnSDK.Position.BottomRight:
                    position.x += Screen.width - logoSize;
                    position.y += Screen.height - logoSize;
                    anchorOffset.x = logoSize - anchorSize;
                    anchorOffset.y = logoSize - anchorSize;
                    break;
            }
            var rect = new Rect(position, new Vector2(logoSize, logoSize));
            GUI.Box(rect, "");

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 32;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(rect, "PlayOn Logo Preview", labelStyle);

            rect.size = Vector2.one * anchorSize;

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = Texture2D.whiteTexture;
            rect.position += anchorOffset;
            GUI.Box(rect, "", boxStyle);
        }
    }
#if UNITY_EDITOR
    private void OnValidate() {
        if (_adUnitIDs == null) {
            var sos = UnityEditor.AssetDatabase.FindAssets("t:MondayOFFAdUnitID");

            if (sos == null || sos.Length <= 0) {
                Debug.Log("Ad Unit ScriptableObject not found");
                return;
            }

            _adUnitIDs = UnityEditor.AssetDatabase.LoadAssetAtPath<MondayOFFAdUnitID>(UnityEditor.AssetDatabase.GUIDToAssetPath(sos[0]));
        }
    }
#endif
}
