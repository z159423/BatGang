using System;
#if UNITY_IOS
using System.Net.Http;
#endif
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace MondayOFF {
    class MondayOFFAdsPrebuild : IPreprocessBuildWithReport {
        public int callbackOrder => 1;
        public void OnPreprocessBuild(BuildReport report) {
            var appId = GetAppIdFromAppLovinSettings(
#if UNITY_IOS
                "AdMobIosAppId"
#else
                "AdMobAndroidAppId"
#endif
                );

            // Log error if the App ID is not set.
            if (string.IsNullOrEmpty(appId) || !appId.StartsWith("ca-app-pub-")) {
                if (!EditorUtility.DisplayDialog("Missing Google AdMob ID", "Admob ID is missing!\nAre you sure you want to continue build?", "Continue", "Cancel")) {
                    AppLovinMax.Scripts.IntegrationManager.Editor.AppLovinIntegrationManagerWindow.ShowManager();
                    throw new UnityEditor.Build.BuildFailedException("[AppLovin MAX] AdMob App ID is not set. Please enter a valid app ID within the AppLovin Integration Manager window.");
                }
            }


            // Check Ad Unit
            var adUnitIDs = AssetDatabase.FindAssets("t:MondayOFFAdUnitID");

            if (adUnitIDs.Length != 1) {
                throw new UnityEditor.Build.BuildFailedException("There should only be 1 ad unit ID object");
            }

            var adUnitID = AssetDatabase.LoadAssetAtPath<MondayOFFAdUnitID>(AssetDatabase.GUIDToAssetPath(adUnitIDs[0]));

            if (!adUnitID.HasValidAdUnitIDs()) {
                if (!EditorUtility.DisplayDialog("Missing Ad Unit ID", "Some of ad placements are checked but missing Ad Unit ID!\nAre you sure you want to continue build?", "Continue", "Cancel")) {
                    UnityEditor.Selection.activeObject = adUnitID;
                    throw new UnityEditor.Build.BuildFailedException("Please check Ad Unit IDs.");
                }
            }

            ValidatePlayOn(adUnitID);

        }

        private string GetAppIdFromAppLovinSettings(string property) {
            var settingsFileName = GetAppLovinSettingsAssetPath();
            if (!File.Exists(settingsFileName)) {
                Debug.LogError("[AppLovin MAX] The current plugin version is incompatible with the AdMob adapter. Please update the AppLovin MAX plugin to version 2.4.0 or higher.");
                return null;
            }

            var instance = AssetDatabase.LoadAssetAtPath(settingsFileName, Type.GetType("AppLovinSettings, MaxSdk.Scripts.IntegrationManager.Editor"));
            if (instance == null) {
                Debug.LogError("[AppLovin MAX] The current plugin version is incompatible with the AdMob adapter. Please update the AppLovin MAX plugin to version 2.4.15 or higher");
                return null;
            }

            var adMobAppIdProperty = instance.GetType().GetProperty(property);
            return adMobAppIdProperty == null ? null : adMobAppIdProperty.GetValue(instance, null).ToString();
        }


        private string GetAppLovinSettingsAssetPath() {
            const string AppLovinSettingsExportPath = "MaxSdk/Resources/AppLovinSettings.asset";
            // Since the settings asset is generated during compile time, the asset label will contain platform specific path separator. So, use platform specific export path.  
            var assetLabel = "l:al_max_export_path-" + AppLovinSettingsExportPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var guids = AssetDatabase.FindAssets(assetLabel);

            var defaultPath = Path.Combine("Assets", AppLovinSettingsExportPath);

            return guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : defaultPath;
        }


        /*********************************************************************
            It tests validity of PlayOn keys
                1. Checks if game key is UUID with regex.
                2. (iOS only) Get Appstore ID using bundle ID and match it
        **********************************************************************/
        // [MenuItem("MondayOFF/Validate PlayOn Key", false, 100)]
        public static void ValidatePlayOn(MondayOFFAdUnitID adUnitID) {
            /*
            Tested with Like a Pizza(iOS)
            Bundle ID: com.mondayoff.pizza
            PlayOn Key: 3e983bd9-090e-4139-b3a5-7c50ee131b2d
            Appstore ID: 1621988938
            */

            if (adUnitID.hasPlayOn) {
                bool isValid = true;

                string playOnKey = adUnitID.playOnKey;
                System.Text.RegularExpressions.Regex uuidRegex = new System.Text.RegularExpressions.Regex("[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}");
                isValid &= uuidRegex.IsMatch(playOnKey);
                Debug.Log("PlayOn Key passed: " + isValid + " (" + playOnKey + ")");
#if UNITY_IOS
                if (isValid) {
                    bool hasValidStoreID = false;
                    string storeID = adUnitID.storeID;

                    using (var client = new HttpClient()) {
                        var bundleID = Application.identifier;
                        // bundleID = "com.mondayoff.pizza";
                        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"http://itunes.apple.com/lookup?bundleId={bundleID}");

                        var task = System.Threading.Tasks.Task.Run(() => client.SendAsync(getRequest));
                        task.Wait();
                        var response = task.Result;
                        if (response.IsSuccessStatusCode) {
                            var readTask = System.Threading.Tasks.Task.Run(() => response.Content.ReadAsStringAsync());
                            readTask.Wait();
                            var content = readTask.Result;
                            // ! DANGER! 50 is an arbitary number 
                            if (content.Length < 50) {
                                throw new UnityEditor.Build.BuildFailedException("This app is NOT publishined on Apple AppStore! Try diabling 'has PlayOn' in AdUnits.\nIf the app is already published, please check bundle ID");
                            }
                            if (content.Contains($":{storeID},")) {
                                hasValidStoreID = true;
                            } else {
                                var keywordToSearch = "trackId\":";
                                var indexOfTrackID = content.IndexOf(keywordToSearch) + keywordToSearch.Length;
                                var indexOfEndingComma = content.IndexOf(',', indexOfTrackID);
                                var trackID = content.Substring(indexOfTrackID, indexOfEndingComma - indexOfTrackID);

                                if (EditorUtility.DisplayDialog("Incorrect AppStore ID", $"Appstore ID {storeID} does NOT match with {trackID} for bundle id {bundleID}!\nDo you want to change it?", "Open AdUnitID", "Cancel")) {
                                    UnityEditor.Selection.activeObject = adUnitID;
                                }
                                throw new UnityEditor.Build.BuildFailedException($"Try setting store ID to <color=yellow>{trackID}</color>");

                            }
                            Debug.Log("PlayOn store ID passed: " + hasValidStoreID + " (" + storeID + ")");
                        }
                        isValid &= hasValidStoreID;
                    }
                }
#endif
                if (!isValid) {
                    throw new UnityEditor.Build.BuildFailedException("Incorrect Key or StoreID enterd for PlayOn!  Try diabling 'has PlayOn' in AdUnits.");
                }
            }
        }
    }
}