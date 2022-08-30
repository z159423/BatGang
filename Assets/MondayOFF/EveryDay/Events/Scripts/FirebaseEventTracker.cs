#if FIREBASE_ENABLED && !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

namespace MondayOFF {
    public class EventTracker {
        System.Type adInfoType = null;
        bool _isFirebaseReady = true;
#if UNITY_ANDROID
        Firebase.FirebaseApp _app = null;
#endif

        public void initialize() {
#if UNITY_ANDROID
            _isFirebaseReady = false;
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    OnFirebaseInitialized();

                } else {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is NOT safe to use here.
                }
            });
#else
            OnFirebaseInitialized();
#endif
        }

        public void TryStage(int stageNum, string stageName = "Stage") {
            if (!_isFirebaseReady) { return; }

            FirebaseAnalytics.LogEvent("Try",
                new Parameter(stageName, $"{stageName} {stageNum:000}")
            );
        }

        public void ClearStage(int stageNum, string stageName = "Stage") {
            if (!_isFirebaseReady) { return; }

            FirebaseAnalytics.LogEvent("Clear",
                new Parameter(stageName, $"{stageName} {stageNum:000}")
            );
        }

        // Stringify prameter values
        public void LogCustomEvent(string eventName, Dictionary<string, string> parameters) {
            if (!_isFirebaseReady) { return; }

            if (parameters == null) {
                FirebaseAnalytics.LogEvent(eventName);
            } else {
                var eventParams = new Parameter[parameters.Count];
                int i = 0;
                foreach (var item in parameters) {
                    eventParams[i++] = new Parameter(item.Key, item.Value);
                }
                FirebaseAnalytics.LogEvent(eventName, eventParams);
            }
        }

        private void OnFirebaseInitialized() {
            _isFirebaseReady = true;

            try {
                var maxAssembly = System.Reflection.Assembly.Load("MaxSdk.Scripts");
                if (maxAssembly != null) {
                    adInfoType = maxAssembly.GetType("MaxSdkBase+AdInfo");
                    var trackAdRevenueMi = this.GetType().GetMethod(nameof(TrackAdRevenue), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).MakeGenericMethod(typeof(string), adInfoType);

                    var interstitialCallbackType = maxAssembly.GetType("MaxSdkCallbacks+Interstitial");
                    var onAdRevenuePaidEventProperty = interstitialCallbackType.GetEvent("OnAdRevenuePaidEvent");

                    var trackAdRevenueDelegate = trackAdRevenueMi.CreateDelegate(onAdRevenuePaidEventProperty.EventHandlerType, this);

                    onAdRevenuePaidEventProperty.AddEventHandler(this, trackAdRevenueDelegate);

                    var rewardedCallbackType = maxAssembly.GetType("MaxSdkCallbacks+Rewarded");
                    onAdRevenuePaidEventProperty = rewardedCallbackType.GetEvent("OnAdRevenuePaidEvent");

                    onAdRevenuePaidEventProperty.AddEventHandler(this, trackAdRevenueDelegate);

                    var bannerCallbackType = maxAssembly.GetType("MaxSdkCallbacks+Banner");
                    onAdRevenuePaidEventProperty = bannerCallbackType.GetEvent("OnAdRevenuePaidEvent");

                    onAdRevenuePaidEventProperty.AddEventHandler(this, trackAdRevenueDelegate);
                } else {
                    Debug.LogWarning("Max SDK not integrated");
                }
            } catch (System.Exception e) {
                Debug.LogException(e);
            }

            FirebaseAnalytics.SetUserId(EveryDay.CustomUserID);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
        }

        private void TrackAdRevenue<T1, T2>(T1 adUnitID, T2 adInfo) {
            if (!_isFirebaseReady) { return; }

            string networkName = (string)adInfoType.GetProperty("NetworkName", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(adInfo);
            string adFormat = (string)adInfoType.GetProperty("AdFormat", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(adInfo);
            double revenue = (double)adInfoType.GetProperty("Revenue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(adInfo);

            Parameter[] AdParameters = {
                    new Parameter("ad_platform", "MAX"),
                    new Parameter("ad_source", networkName),
                    new Parameter("ad_format", adFormat),
                    new Parameter("currency","USD"),
                    new Parameter("value", revenue)
                };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }
}
#endif