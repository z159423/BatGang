using UnityEngine;
namespace MondayOFF {
    public partial class AdsManager : MonoBehaviour {
        AdUnit _adUnit = default;
        int _adCount = 0;

        public bool CustomShowAudioAd() {
#if UNITY_EDITOR
            return false;
#endif
            if (PlayOnSDK.IsInitialized() && _adUnit.IsAdAvailable()) {
                _adUnit.ShowAd();
                return true;
            }

            return false;
        }

        public void CustomCloseAudioAd() {
#if UNITY_EDITOR
            return;
#endif
            _adUnit.CloseAd();
        }

        private void InitializePlayOnSDK() {
#if UNITY_EDITOR
            return;
#endif

            PlayOnSDK.OnInitializationFinished += OnPlayOnInitialized;

            PlayOnSDK.Initialize(adUnitIDs.playOnKey
#if UNITY_IOS
              , adUnitIDs.storeID
#endif
         );
        }

        private void OnPlayOnInitialized() {
#if UNITY_IOS
        if (MaxSdk.HasUserConsent())
#endif
            {
                PlayOnSDK.SetGdprConsent(true);
                PlayOnSDK.SetIABUSPrivacyString(privacyString);
            }

            PlayOnSDK.AdUnitType adType = PlayOnSDK.AdUnitType.AudioLogoAd;
            _adUnit = new AdUnit(adType);

            if (adUnitIDs.shouldShowLogoAfterInterstitial && adUnitIDs.interstitialCount > 0) {
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (a, aa) => {
                    if (_adCount++ % adUnitIDs.interstitialCount == 0) {
                        CustomShowAudioAd();
                    }
                };
            }

            PlayOnSDK.SetLogLevel(adUnitIDs.playOnLogLevel);
            _adUnit.SetLogo(adUnitIDs.playOnLogoAnchor, adUnitIDs.playOnLogoOffset.x, adUnitIDs.playOnLogoOffset.y, adUnitIDs.playOnLogoSize);
        }

        private void OnApplicationPause(bool pauseStatus) {
#if UNITY_EDITOR
            return;
#endif
            PlayOnSDK.onApplicationPause(pauseStatus);
        }
    }
}
