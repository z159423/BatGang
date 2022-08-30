using UnityEngine;

#if UNITY_EDITOR 
namespace MondayOFF {
    public partial class AdsManager : MonoBehaviour {
        private System.Collections.IEnumerator CheckConsentStatus(MaxSdkBase.SdkConfiguration sdkConfiguration) {
            yield return DelayedAdLoad();
        }
    }
}
#endif