#if UNITY_PURCHASING && !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace MondayOFF.IAP {
    public class StoreListener : IStoreListener {
        enum ValidationStatus : int {
            NoValidation = 0,
            ReceiptValidation = 1,
        }
        internal IStoreController storeController = null;
        internal IExtensionProvider extensionProvider = null;

        internal System.Action onInitialized = default;

        readonly byte[] googleTangleData = null;
        readonly byte[] appleTangleData = null;
        readonly System.Func<PurchaseEventArgs, PurchaseProcessingResult>[] processPurchaseMethods = null;

        internal bool isInitialized => storeController != null && extensionProvider != null;
        internal System.Action hideLoading { private get; set; }
        internal Dictionary<string, ProductData> productDict { private get; set; }

        ValidationStatus validationStatus = default;

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            storeController = controller;
            extensionProvider = extensions;

            System.Func<PurchaseEventArgs, PurchaseProcessingResult> a = ProcessPurchaseWithReceiptValidation;
            onInitialized?.Invoke();
            onInitialized = null;
        }

        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            hideLoading?.Invoke();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            return processPurchaseMethods[(int)validationStatus].Invoke(args);
        }

        internal StoreListener() {
            validationStatus = ValidationStatus.NoValidation;
            try {
                // get Tangle Data if exists
                var tangleAssembly = System.Reflection.Assembly.Load("Tangle");

                var googleTangle = tangleAssembly.GetType("UnityEngine.Purchasing.Security.GooglePlayTangle");
                if (googleTangle != null) {
                    var dataMethod = googleTangle.GetMethod("Data");
                    googleTangleData = (byte[])dataMethod.Invoke(null, null);
                }

                var appleTangle = tangleAssembly.GetType("UnityEngine.Purchasing.Security.AppleTangle");
                if (appleTangle != null) {
                    var dataMethod = appleTangle.GetMethod("Data");
                    appleTangleData = (byte[])dataMethod.Invoke(null, null);
                }

                // They should have both?
                if (googleTangle != null && appleTangle != null) {
                    validationStatus = ValidationStatus.ReceiptValidation;
                }
            } catch (System.Exception e) {
                Debug.Log("No Tangle found, proceed without validation");
                Debug.LogException(e);
            }

            processPurchaseMethods = new System.Func<PurchaseEventArgs, PurchaseProcessingResult>[]{
                ProcessPurchaseWithoutValidation,
                ProcessPurchaseWithReceiptValidation,
            };
        }

        private PurchaseProcessingResult ProcessPurchaseWithoutValidation(PurchaseEventArgs args) {
            Debug.Log("[IAPMANAGER] withOUT validation");
            var purchasedProductID = args.purchasedProduct.definition.id;

            Debug.Assert(productDict.ContainsKey(purchasedProductID), $"How is {purchasedProductID} not found in the catalogue??");

            try {
                var unityAssembly = System.Reflection.Assembly.Load("Assembly-CSharp");
                var singularType = unityAssembly.GetType("SingularSDK");
                if (singularType != null) {
                    var inAppPurchaseMethodInfo = singularType.GetMethod("InAppPurchase", new System.Type[] { typeof(UnityEngine.Purchasing.Product), typeof(Dictionary<string, object>), typeof(bool) });
                    inAppPurchaseMethodInfo.Invoke(null, new object[] { args.purchasedProduct, null, false });
                } else {
                    throw new System.Exception("SingularSDK Class not found");
                }
            } catch (System.Exception e) {
                Debug.LogError("Failed to send purchase data to Singular");
                Debug.LogException(e);
            }

            // Debug.Assert(productDict[purchasedProductID].onPurchase != null, $"No reward will be given for {purchasedProductID} if callback is null!");
            productDict[purchasedProductID].CompletePurchase();

            hideLoading?.Invoke();
            return PurchaseProcessingResult.Complete;
        }

        private PurchaseProcessingResult ProcessPurchaseWithReceiptValidation(PurchaseEventArgs args) {
            Debug.Log("[IAPMANAGER] with validation");
            // local receipt validation using Tangles
            bool validPurchase = true; // Presume valid for platforms with no R.V.
                                       // Unity IAP's validation logic is only included on these platforms.
                                       // Prepare the validator with the secrets we prepared in the Editor
                                       // obfuscation window.

            var validator = new CrossPlatformValidator(googleTangleData, appleTangleData, Application.identifier);


            try {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(args.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");

            /****************** Uncomment below if you need to inspect contents of receipt ***************
                            foreach (IPurchaseReceipt productReceipt in result) {
                                Debug.Log(productReceipt.productID);
                                Debug.Log(productReceipt.purchaseDate);
                                Debug.Log(productReceipt.transactionID);

            #if UNITY_ANDROID
                                GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                                if (null != google) {
                                    Debug.Log(google.purchaseState);
                                    Debug.Log(google.purchaseToken);
                                }
            #elif UNITY_IOS
                                AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                                if (null != apple) {
                                    Debug.Log(apple.originalTransactionIdentifier);
                                    Debug.Log(apple.subscriptionExpirationDate);
                                    Debug.Log(apple.cancellationDate);
                                    Debug.Log(apple.quantity);
                                }
            #endif
                            }
            ***************************************************************************************************/

            } catch (IAPSecurityException e) {
                Debug.LogException(e);
                validPurchase = false;
            }


            if (validPurchase) {
                // Unlock the appropriate content here.
                productDict[args.purchasedProduct.definition.id].CompletePurchase();

                // SingularSDK.InAppPurchase(purchaseEvent.purchasedProduct, null);
                try {
                    var unityAssembly = System.Reflection.Assembly.Load("Assembly-CSharp");
                    var singularType = unityAssembly.GetType("SingularSDK");
                    if (singularType != null) {
                        var inAppPurchaseMethodInfo = singularType.GetMethod("InAppPurchase", new System.Type[] { typeof(UnityEngine.Purchasing.Product), typeof(Dictionary<string, object>), typeof(bool) });
                        inAppPurchaseMethodInfo.Invoke(null, new object[] { args.purchasedProduct, null, false });
                    } else {
                        throw new System.Exception("SingularSDK Class not found");
                    }
                } catch (System.Exception e) {
                    Debug.LogError("Failed to send purchase data to Singular");
                    Debug.LogException(e);
                }
            }

            hideLoading?.Invoke();
            return PurchaseProcessingResult.Complete;
        }
    }
}
#endif