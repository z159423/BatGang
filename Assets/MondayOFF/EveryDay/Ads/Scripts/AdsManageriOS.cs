using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
namespace MondayOFF {
    public partial class AdsManager : MonoBehaviour {
        private System.Collections.IEnumerator CheckConsentStatus(MaxSdkBase.SdkConfiguration sdkConfiguration) {
            var consentStatus = sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized;

            // MAX
            MaxSdk.SetHasUserConsent(consentStatus);
            MaxSdk.SetDoNotSell(!consentStatus);

            // FB
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(consentStatus);
            if (consentStatus) {
                AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { });
            } else {
                AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { "LDU" }, 0, 0);
            }

            // US privacy string
            char[] privacyCharacters = new char[4];
            privacyCharacters[0] = '1';
            if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies) {
                privacyCharacters[1] = 'Y';
                if (consentStatus) {
                    privacyCharacters[2] = 'N';
                } else {
                    privacyCharacters[2] = 'Y';
                }
            } else {
                privacyCharacters[1] = '-';
                privacyCharacters[2] = '-';
            }
            privacyCharacters[3] = '-';

            privacyString = new string(privacyCharacters);

            OnPrivacyStringSet?.Invoke(privacyString);

            yield return DelayedAdLoad();
        }
    }
}
#endif

/*

US PRIVACY STUFF


String Component    Expected Values Definition
Specification Version   
Number

(1 char in string)

The version of this string specification used to encode the string
Notice/Opportunity to Opt Out   
ENUM

(N = No, Y = Yes, - = Not Applicable)

Have both of the following been provided: (i) notice pursuant to 1798.100(b) of the CCPA and (ii) the opportunity to opt out of the "sale" of personal information pursuant to 1798.120 and 1798.135 of the CCPA
Opt-Out Sale    
ENUM

(N = No, Y = Yes, - = Not Applicable. For use ONLY when CCPA does not apply.)

Has user opted-out of the sale of his or her personal information pursuant to 1798.120 and 1798.135 of the CCPA? If CCPA applies, only Y (yes) or N (no) can be used.
LSPA Covered Transaction    
ENUM

(N = No, Y = Yes, - = Not Applicable)

Publisher is a signatory to the IAB Limited Service Provider Agreement(LSPA) and the publisher declares that the transaction is covered as a “Covered Opt Out Transaction” or a “Non Opt Out Transaction” as those terms are defined in the Agreement.

*/