using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
namespace MondayOFF {
    public partial class AdsManager : MonoBehaviour {
        private System.Collections.IEnumerator CheckConsentStatus(MaxSdkBase.SdkConfiguration sdkConfiguration) {
            // if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies) {
            //             MaxSdk.UserService.ShowConsentDialog();
            //             MaxSdkCallbacks.OnSdkConsentDialogDismissedEvent += () => {
            //                 
            //             };
            //         }

            // MAX
            MaxSdk.SetHasUserConsent(true);
            MaxSdk.SetDoNotSell(false);

            // FB
            AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { });

            // US privacy string
            privacyString = "1---";

            OnPrivacyStringSet?.Invoke(privacyString);

            yield return DelayedAdLoad();
        }
    }
}
#endif