#if !FIREBASE_ENABLED || UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace MondayOFF {
    public class EventTracker {
        public void initialize() {
            Debug.Log("Initialze event tracker");
        }

        public void TryStage(int stageNum, string stageName = "Stage") {
            Debug.Log($"Trying {stageName} {stageNum}");
        }

        public void ClearStage(int stageNum, string stageName = "Stage") {
            Debug.Log($"Cleared {stageName} {stageNum}");
        }

        // Stringify prameter values
        public void LogCustomEvent(string eventName, Dictionary<string, string> parameters = null) {
            if (parameters == null) {
                Debug.Log($"{eventName} logged without any parameters");
            } else {
                string paramString = "\n";
                foreach (var item in parameters) {
                    paramString += $"{item.Key} : {item.Value}\n";
                }

                Debug.Log($"{eventName} logged with parameters: {paramString}");
            }
        }
    }
}
#endif