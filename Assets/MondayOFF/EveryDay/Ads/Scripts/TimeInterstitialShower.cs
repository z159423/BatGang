using UnityEngine;

public class TimeInterstitialShower : MonoBehaviour {
    public static TimeInterstitialShower instance = default;
    [SerializeField] float interval = default;
    float lastShown = default;

    public void SetInterval(float newInterval) {
        interval = newInterval;
    }

    /// <summary>
    /// Show Interstitial Ad if an interval has passed.
    /// </summary>
    public bool CheckTimeAndShowInterstitial() {
        // Check No-Ads

        // Check timer interval
        if ((interval < Time.realtimeSinceStartup - lastShown)) {
            MondayOFF.AdsManager.instance.ShowInterstitial();
            // Debug.Log("====Interval  " + (Time.realtimeSinceStartup - lastShown).ToString());
            return true;
        }
        return false;
    }

    public void ResetTime() {
        lastShown = Time.realtimeSinceStartup;
    }

    private void ResetTime(string adUnitID, MaxSdkBase.AdInfo adInfo) {
        ResetTime();
    }

    private void ResetTime(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) {
        ResetTime();
    }

    private void Awake() {
        if (TimeInterstitialShower.instance == null) {
            TimeInterstitialShower.instance = this;
        } else {
            Debug.Assert(false, "Multiple instance of TimeInterstitialShower found!");
            DestroyImmediate(this.gameObject);
        }
    }

    private void Start() {
        ResetTime();

        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += ResetTime;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += ResetTime;
    }
}