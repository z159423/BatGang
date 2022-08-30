using System.Collections.Generic;
using UnityEngine;
// FB is a MUST!
using Facebook.Unity;

namespace MondayOFF {
    public class EventsManager : MonoBehaviour {
        public static EventsManager instance { get; private set; }

        EventTracker _eventTracker = new EventTracker();
        public void TryStage(int stageNum, string stageName = "Stage") => _eventTracker.TryStage(stageNum, stageName);
        public void ClearStage(int stageNum, string stageName = "Stage") => _eventTracker.ClearStage(stageNum, stageName);
        public void LogCustomEvent(string eventName, Dictionary<string, string> parameters = null) => _eventTracker.LogCustomEvent(eventName, parameters);

        private void Awake() {
            if (instance != null) {
                Debug.Assert(false, "Duplicate EventsManager found");
                DestroyImmediate(this.gameObject);
                return;
            }
            instance = this;

            // Facebook
            if (!FB.IsInitialized) {
                try {
                    FB.Init(OnFBInitialization);
                } catch (System.Exception e) {
                    Debug.LogException(e);
                    Debug.LogWarning("Failed to initialize Facebook SDK");
                }
            } else {
                FB.ActivateApp();
            }

            _eventTracker.initialize();

            // Initialize Game Analytics if implemented
            var unityAssembly = System.Reflection.Assembly.Load("Assembly-CSharp");
            var gaType = unityAssembly.GetType("GameAnalyticsSDK.GameAnalytics");
            if (gaType == null) {
                Debug.LogWarning("GameAnalytics not integrated");
                return;
            }
            var initializeMethodInfo = gaType.GetMethod("Initialize");
            Debug.Log(initializeMethodInfo);
            initializeMethodInfo.Invoke(null, null);
        }

        private void OnFBInitialization() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else {
                Debug.LogWarning("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnApplicationPause(bool pauseStatus) {
            if (!pauseStatus) {
                if (FB.IsInitialized) {
                    OnFBInitialization();
                }
            }
        }
    }
}