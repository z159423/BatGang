using UnityEngine;
using MondayOFF;
using MondayOFF.IAP;

public class AdTestScript : MonoBehaviour {
    public void OnClickIS() {
        var isSuccess = TimeInterstitialShower.instance?.CheckTimeAndShowInterstitial();
        if (!isSuccess.HasValue || !isSuccess.Value) {
            Debug.Log("Calling interstital too frequently, try again later");
        }
    }

    public void OnClickRV() {
        AdsManager.instance?.ShowRewardedVideo(null);
    }

    public void OnClickBanner() {
        AdsManager.instance?.ShowBanner();
    }

    public void TryStage(int stageNum) {
        EventsManager.instance?.TryStage(stageNum);
    }

    public void ClearStage(int stageNum) {
        EventsManager.instance?.ClearStage(stageNum);
    }

    public void PurchaseItem() {
#if UNITY_PURCHASING
        var productSO = Resources.FindObjectsOfTypeAll<MondayOFFProducts>();
        if (productSO == null || productSO.Length <= 0 || productSO[0].products.Length <= 0) {
            Debug.Log("No valid product found!");
            return;
        }
        var productID = productSO[0].products[0].productID;
        IAPManager.instance.RegisterProducts(productID, () => Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@ Grant reward for " + productID));
        IAPManager.instance.PurchaseProduct(productID);
#else
        Debug.Log("Purchasing is not enabled");
#endif
    }
}
