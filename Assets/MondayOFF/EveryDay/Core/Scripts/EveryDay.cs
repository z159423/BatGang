/*
 _ _ _                             __    _____   _____
' ) ) )             /             / ')    /  '    /  '
 / / /  __ ____  __/  __.  __  , /  /  ,-/-,   ,-/-,  
/ ' (_ (_)/ / <_(_/_ (_/|_/ (_/_(__/  (_/     (_/     
                             /                        
                            '                         

EveryDay.cs, v 1.0.0
Copyright(c) 2019 Monday OFF, http://mondayoff.me
*/

using UnityEngine;

[assembly: UnityEngine.Scripting.Preserve]
namespace MondayOFF {
    public static class EveryDay {
        internal static Identifier identifier;
        internal static Referrer referrer;
        internal static Accessor accessor;


        public static string CustomUserID {
            get {
                if (identifier == null) {
                    initialize();
                }
                return identifier.CustomUser;
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void AfterSceneLoad() {
            initialize();
        }


        static void initialize() {
            if (accessor != null) {
                Debug.Log("[EVERYDAY] Already initialized");
                return;
            }

            Debug.Log("[EVERYDAY] INIT");
            identifier = new Identifier(OnIdentifierReady);
            referrer = new Referrer(OnReferrerReady);
            accessor = new Accessor();

            identifier.initialize();
            referrer.initialize();
        }

        static void OnIdentifierReady() {
            TryToSetAccessReady();
        }

        static void OnReferrerReady() {
            TryToSetAccessReady();
        }

        static void TryToSetAccessReady() {
            if (IsReadyToAccess()) OnAccessReady();
        }

        static bool IsReadyToAccess() {
            return (identifier != null && referrer != null)
                && (identifier.IsIdentifierReady && referrer.IsReferrerReady);
        }

        static void OnAccessReady() {
/**********************************************************************
    Access no longer required.. for now
**********************************************************************/
            return;

            accessor.Access();
        }
    }
}