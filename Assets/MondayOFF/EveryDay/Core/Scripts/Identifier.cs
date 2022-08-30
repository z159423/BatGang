/*
 _ _ _                             __    _____   _____
' ) ) )             /             / ')    /  '    /  '
 / / /  __ ____  __/  __.  __  , /  /  ,-/-,   ,-/-,  
/ ' (_ (_)/ / <_(_/_ (_/|_/ (_/_(__/  (_/     (_/     
                             /                        
                            '                         

Identifier.cs, v 1.0.0
Copyright(c) 2019 Monday OFF, http://mondayoff.me
*/
using UnityEngine;
namespace MondayOFF {
    internal class Identifier {
        bool isIdentifierReady;
        internal bool IsIdentifierReady {
            get {
                return isIdentifierReady;
            }
        }
        internal delegate void OnIdentifierReady();
        internal OnIdentifierReady onIdentifierReady;

        internal string ForVendor {
            get {
                return UnityEngine.SystemInfo.deviceUniqueIdentifier;
            }
        }

        string forAdvertising;
        internal string ForAdvertising {
            get {
                return forAdvertising;
            }
        }

        internal string CustomUser {
            get {
                if (!PlayerPrefs.HasKey(Idcu)) {
#if NET_4_6
                    var timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#else
                    System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                    var timestamp = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
#endif
                    PlayerPrefs.SetString(Idcu, HashMd5(ForVendor + timestamp));
                }
                return PlayerPrefs.GetString(Idcu);
            }
        }

        internal bool TrackingEnabled {
            get {
                return PlayerPrefs.GetInt(trackingEnabled, 1) == 1 ? true : false;
            }
            set {
                PlayerPrefs.SetInt(trackingEnabled, value == true ? 1 : 0);
            }
        }

        const string Idcu = "PREF_IDCU";
        const string trackingEnabled = "PREF_TRACKING_ENABLED";


        string HashMd5(string strToEncrypt) {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++) {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        internal Identifier(OnIdentifierReady onIdentifierReady) {
            this.onIdentifierReady = onIdentifierReady;
        }

        internal void initialize() {
            if (!Application.RequestAdvertisingIdentifierAsync(RequestAdvertisingIDCallback)) {
                RequestAdvertisingIDCallback(string.Empty, false, string.Format("{0} does not support Advertising Identifiers.", System.Enum.GetName(typeof(RuntimePlatform), Application.platform)));
            }
        }

        void RequestAdvertisingIDCallback(string advertisingId, bool trackingEnabled, string errorMsg) {
            forAdvertising = advertisingId;
            TrackingEnabled = trackingEnabled;
            isIdentifierReady = true;
            onIdentifierReady?.Invoke();
        }
    }
}
