using UnityEngine;

public class DontDestroyObj : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
