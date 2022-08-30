#if UNITY_PURCHASING
using UnityEngine;

namespace MondayOFF {
    // [CreateAssetMenu(menuName = "products", fileName = "Products", order = 10)]
    public class MondayOFFProducts : ScriptableObject {
        [Header("Please enter Product IDs here")] public ProductData[] products = default;
    }

    [System.Serializable]
    public class ProductData {
        public string productID;
        public UnityEngine.Purchasing.ProductType productType;

        public System.Action onPurchase { private get; set; }
        public int purchaseCount { get; set; }
        public bool isRegistered => onPurchase != null && onPurchase.GetInvocationList().Length > 0;

        public void CompletePurchase() {
            if (isRegistered) {
                onPurchase.Invoke();
            } else {
                if (productType == UnityEngine.Purchasing.ProductType.Consumable) {
                    ++purchaseCount;
                } else {
                    purchaseCount = 1;
                }
            }
        }
    }
}

#endif