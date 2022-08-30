/*
 _ _ _                             __    _____   _____
' ) ) )             /             / ')    /  '    /  '
 / / /  __ ____  __/  __.  __  , /  /  ,-/-,   ,-/-,  
/ ' (_ (_)/ / <_(_/_ (_/|_/ (_/_(__/  (_/     (_/     
                             /                        
                            '                         

Referrer.cs, v 1.0.0
Copyright(c) 2019 Monday OFF, http://mondayoff.me
*/
using UnityEngine;


namespace MondayOFF {
    internal class Referrer {
        bool isReferrerReady;
        internal bool IsReferrerReady {
            get {
                return isReferrerReady;
            }
        }
        internal delegate void OnReferrerReady();
        internal OnReferrerReady onReferrerReady;

        internal Network? Network {
            get {
                if (!PlayerPrefs.HasKey(referrer)) return null;
                var network = PlayerPrefs.GetInt(referrer);
                return (Network)network;
            }

            set {
                if (value == null) return;
                PlayerPrefs.SetInt(referrer, (int)value);
            }
        }

        internal string ReferrerContent = default;

        readonly string referrer = "PREF_REFERRER";
        // readonly float findReferTimeOut = 30f;


        internal Referrer(OnReferrerReady onReferrerReady) {
            this.onReferrerReady = onReferrerReady;
        }

        internal void initialize() {
            CompleteFind();
            return;

            // ! Not using Facebook referrer anymore
            /*
            if (AlreadyHasReferrer()) {
                Debug.Log("No refer");
                CompleteFind();
            } else {
                // StartCoroutine(FindRefer());
            }
            */
        }

        bool AlreadyHasReferrer() {
            return Network != null;
        }

        void CompleteFind() {
            isReferrerReady = true;
            onReferrerReady?.Invoke();
        }

        // ! Not using Facebook referrer anymore

        /*
                IEnumerator FindRefer() {
                    yield return FindFacebookRefer();
                    // There will be
                    // more networks
                    // .....
                    CompleteFind();
                }

                IEnumerator FindFacebookRefer() {
                    var timeOut = findReferTimeOut;
                    Debug.Log("[ Invoke ] " + GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    while (!Facebook.Unity.FB.IsInitialized) {
                        timeOut -= 1f;
                        if (timeOut < 0f) {
                            yield break;
                        }
                        yield return new WaitForSecondsRealtime(1f);
                    }
                    Facebook.Unity.FB.Mobile.FetchDeferredAppLinkData(FetchAppLinkCallback);
                }

                void FetchAppLinkCallback(Facebook.Unity.IAppLinkResult result) {
                    if (!string.IsNullOrEmpty(result.TargetUrl)) {
                        Network = MondayOFF.Network.Facebook;
                        ReferrerContent = result.TargetUrl;
                    }
                }
        */
    }
}
