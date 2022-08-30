using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class AsyncSceneTransfer : MonoBehaviour {
    [SerializeField][ScenePicker] int sceneIndex = 0;
    AsyncOperation asyncOp = null;

    public void Transfer() {
        if (asyncOp == null) {
            return;
        }
        asyncOp.allowSceneActivation = true;
    }

    private void loadSceneAsync() {
        asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
        if (asyncOp == null) {
            Debug.LogWarning($"Failed to find scene index {sceneIndex}");
            return;
        }
        asyncOp.allowSceneActivation = false;
    }

    private void Start() {
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        if (sceneIndex < 0 || sceneIndex >= sceneCount) {
            Debug.LogException(new System.Exception("Invalid scene is selected!"));
            return;
        }

        loadSceneAsync();
    }
}
