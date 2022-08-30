#if UNITY_PURCHASING && UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace MondayOFF.IAP {
    internal class StoreListener : IStoreListener {
        internal IStoreController storeController = null;
        internal IExtensionProvider extensionProvider = null;

        internal bool isInitialized => true;
        internal System.Action onInitialized { get; set; }
        internal System.Action hideLoading { private get; set; }
        internal Dictionary<string, ProductData> productDict { private get; set; }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            Debug.Log("Initialized Store Listener");

            storeController = controller;
            extensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.Log("Failed to initialize Store Listener");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            Debug.Log($"Failed purchasing : {product.definition.id}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
            var purchasedProductID = purchaseEvent.purchasedProduct.definition.id;

            Debug.Assert(productDict.ContainsKey(purchasedProductID), $"How is {purchasedProductID} not found in the catalogue??");

            productDict[purchasedProductID].CompletePurchase();

            hideLoading?.Invoke();
            return PurchaseProcessingResult.Complete;
        }
    }
}
#endif