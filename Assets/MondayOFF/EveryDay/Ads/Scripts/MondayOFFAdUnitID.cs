using UnityEngine;

namespace MondayOFF {
    // [CreateAssetMenu(menuName = "adunit", fileName = "AdUnitIDs", order = 10)]
    public partial class MondayOFFAdUnitID : ScriptableObject {
#pragma warning disable CS0414
        [Header("====== iOS ======")]
        [SerializeField][LabelOverride("Interstitial (IS)")] bool iOS_has_IS = true;
        [SerializeField][DrawIf("iOS_has_IS", "Ad Unit ID")] string iOS_IS_AdUnitID = "";
        [SerializeField][LabelOverride("Rewarded (RV)")] bool iOS_has_RV = true;
        [SerializeField][DrawIf("iOS_has_RV", "Ad Unit ID")] string iOS_RV_AdUnitID = "";
        [SerializeField][LabelOverride("Banner (BN)")] bool iOS_has_BN = true;
        [SerializeField][DrawIf("iOS_has_BN", "Ad Unit ID")] string iOS_BN_AdUnitID = "";


        [Header("====== Android ======")]
        [SerializeField][LabelOverride("Interstitial (IS)")] bool AOS_has_IS = true;
        [SerializeField][DrawIf("AOS_has_IS", "Ad Unit ID")] string AOS_IS_AdUnitID = "";
        [SerializeField][LabelOverride("Rewarded (RV)")] bool AOS_has_RV = true;
        [SerializeField][DrawIf("AOS_has_RV", "Ad Unit ID")] string AOS_RV_AdUnitID = "";
        [SerializeField][LabelOverride("Banner (BN)")] bool AOS_has_BN = true;
        [SerializeField][DrawIf("AOS_has_BN", "Ad Unit ID")] string AOS_BN_AdUnitID = "";
#pragma warning restore CS0414


#if UNITY_IOS
        public string interstitialAdUnitId => iOS_IS_AdUnitID;
        public string rewardedAdUnitId => iOS_RV_AdUnitID;
        public string bannerAdUnitId => iOS_BN_AdUnitID;

        private bool hasInterstitial => iOS_has_IS;
        private bool hasRewarded => iOS_has_RV;
        private bool hasBanner => iOS_has_BN;
#else
        public string interstitialAdUnitId => AOS_IS_AdUnitID;
        public string rewardedAdUnitId => AOS_RV_AdUnitID;
        public string bannerAdUnitId => AOS_BN_AdUnitID;

        private bool hasInterstitial => AOS_has_IS;
        private bool hasRewarded => AOS_has_RV;
        private bool hasBanner => AOS_has_BN;
#endif


        public bool HasInterstitialAdUnitID() {
            return hasInterstitial && !string.IsNullOrEmpty(interstitialAdUnitId);
        }

        public bool HadRewardedAdUnitID() {
            return hasRewarded && !string.IsNullOrEmpty(rewardedAdUnitId);
        }

        public bool HasBannerAdUnitID() {
            return hasBanner && !string.IsNullOrEmpty(bannerAdUnitId);
        }

#if UNITY_EDITOR
        public bool HasValidAdUnitIDs() {
            bool isValid = true;
            if (hasInterstitial) {
                isValid |= !string.IsNullOrEmpty(interstitialAdUnitId);
            }
            if (hasRewarded) {
                isValid |= !string.IsNullOrEmpty(rewardedAdUnitId);
            }
            if (hasBanner) {
                isValid |= !string.IsNullOrEmpty(bannerAdUnitId);
            }

            return isValid;
        }
#endif
    }

    // PlayOn
    public partial class MondayOFFAdUnitID : ScriptableObject {
        [Space(50, order = 0)]
        [Header("====== PlayOn (Odeeo) ======", order = 1)]
        [SerializeField][LabelOverride("Has PlayOn (Odeeo) Ad")] bool has_PlayOn = true;
        [SerializeField][DrawIf("has_PlayOn", "Android Game Key")] string AOS_PlayOnGameKey = "";
        [SerializeField][DrawIf("has_PlayOn", "iOS Game Key")] string iOS_PlayOnGameKey = "";
        [SerializeField][DrawIf("has_PlayOn", "iOS Store ID")] string iOS_store_id = "";
        [DrawIf("has_PlayOn", "Auto show Logo Ad")]
        public bool shouldShowLogoAfterInterstitial = false;
        [DrawIf("shouldShowLogoAfterInterstitial", "After N Interstitials")]
        public int interstitialCount = -1;


        [Tooltip("Anchor position of logo ad")]
        [Space(15, order = 0)]
        [DrawIf("has_PlayOn", "Logo Anchor Position", order = 2)] public PlayOnSDK.Position playOnLogoAnchor = PlayOnSDK.Position.TopLeft;
        [Tooltip("Offset from anchor")]
        [DrawIf("has_PlayOn", "Logo Offset")] public Vector2Int playOnLogoOffset = new Vector2Int(0, 150);
        [Tooltip("Logo ad size")]
        [DrawIf("has_PlayOn", "Logo Size")] public int playOnLogoSize = 64;
        [Tooltip("Set it do debug if you need to see debug logs")]

        [Space(15, order = 0)]
        [DrawIf("has_PlayOn", "Log Level", order = 2)] public PlayOnSDK.LogLevel playOnLogLevel = PlayOnSDK.LogLevel.None;

        public bool hasPlayOn => has_PlayOn;
#if UNITY_IOS
        public string playOnKey => iOS_PlayOnGameKey;
        public string storeID => iOS_store_id;
#else
        public string playOnKey => AOS_PlayOnGameKey;
#endif

#if UNITY_EDITOR
        private void OnValidate() {
            if (!has_PlayOn) {
                shouldShowLogoAfterInterstitial = false;
            }
        }
#endif
    }
}