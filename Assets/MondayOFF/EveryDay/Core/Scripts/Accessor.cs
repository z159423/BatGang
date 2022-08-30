/*
 _ _ _                             __    _____   _____
' ) ) )             /             / ')    /  '    /  '
 / / /  __ ____  __/  __.  __  , /  /  ,-/-,   ,-/-,  
/ ' (_ (_)/ / <_(_/_ (_/|_/ (_/_(__/  (_/     (_/     
                             /                        
                            '                         

Accessor.cs, v 1.0.0
Copyright(c) 2019 Monday OFF, http://mondayoff.me
*/
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
namespace MondayOFF {
    internal class Accessor {

        [System.Serializable]
        internal struct AccessData {
            [SerializeField] internal string cc;
            [SerializeField] internal string platform;
            [SerializeField] internal string packageName;
            [SerializeField] internal string cuid;
            [SerializeField] internal string idfv;
            [SerializeField] internal string idfa;
            [SerializeField] internal string accessType;
        }

        string CountryCode {
            get {
                return PreciseLocale.GetRegion();
            }
        }

        string Platform {
            get {
                if (Application.platform == RuntimePlatform.Android) { return "android"; }
                if (Application.platform == RuntimePlatform.IPhonePlayer) { return "ios"; }
                return "unknown";
            }
        }

        const string LAST_ACCESS = "PREF_LAST_ACCESS_DATE";
        const int RETRY_DELAY_MILLISECOND = 3000;

        readonly int RETRY_THRESHOLD = 3;
        readonly string URL;

        bool isInstall = false;
        int retryCount = 0;
        byte[] bodyBuffer = null;

        internal Accessor() {
            Debug.Log("[EVERYDAY] ACCESSOR");
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                URL = "https://api.mondayoff.me/client/access";
            } else {
                URL = "http://127.0.0.1:8888/client/access";

                if (PlayerPrefs.HasKey(LAST_ACCESS)) {
                    PlayerPrefs.SetString(LAST_ACCESS, "TEST");
                }
            }
        }

        internal void Access() {
            Debug.Log("[EVERYDAY] Did Access Today? " + DidAccessToday());
            if (!DidAccessToday()) {
                CreateFormAndPostV2();
            }
        }

        void CreateFormAndPostV2() {
            Debug.Log("[EVERYDAY] Posting V2 " + URL);

            var jsonData = JsonUtility.ToJson(new AccessData() {
                cc = CountryCode,
                platform = Platform,
                packageName = Application.identifier,
                cuid = EveryDay.identifier.CustomUser,
                idfv = EveryDay.identifier.ForVendor,
                idfa = EveryDay.identifier.TrackingEnabled ? EveryDay.identifier.ForAdvertising : null,
                accessType = isInstall ? "install" : "access"
            });
            bodyBuffer = new System.Text.UTF8Encoding().GetBytes(jsonData);

            PostAccessV2();
        }

        void PostAccessV2() {
            UnityWebRequest webRequest = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbPOST);

            webRequest.uploadHandler = new UploadHandlerRaw(bodyBuffer);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            var asyncOperation = webRequest.SendWebRequest();
            asyncOperation.completed += onWebRequestComplete;
        }

        async void onWebRequestComplete(AsyncOperation asyncOp) {
            var result = (asyncOp as UnityWebRequestAsyncOperation);
            Debug.Assert(result != null, "Failed to cast to UnityWebRequestAsyncOperation");

            var www = result.webRequest;
            if (www.isNetworkError || www.isHttpError) {
                if (retryCount < RETRY_THRESHOLD) {
                    ++retryCount;
                    Debug.Log("[EVERYDAY] ERROR: " + www.responseCode);
                    await Task.Delay(RETRY_DELAY_MILLISECOND * retryCount);
                    PostAccessV2();
                }
            } else {
                Debug.Log("[EVERYDAY] SUCCESSFUL: " + www.responseCode);
                PlayerPrefs.SetString(LAST_ACCESS, System.DateTime.UtcNow.ToShortDateString());
                bodyBuffer = null;
            }

            www.Dispose();
        }

        bool DidAccessToday() {
            var lastAccessDate = PlayerPrefs.GetString(LAST_ACCESS, "new");
            isInstall = (lastAccessDate == "new");
            if (System.DateTime.UtcNow.ToShortDateString() == lastAccessDate) return true;
            return false;
        }
    }
}
