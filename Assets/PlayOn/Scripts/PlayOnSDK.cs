using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
#if UNITY_IOS
using AOT;
#endif
public class PlayOnSDK
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public const string BundleClassName = "android.os.Bundle";
    public const string DateClassName = "java.util.Date";
    public const string HashClassName = "java.util.HashMap";

    private static AndroidJavaObject _androidBridge; 

    private static AndroidJavaObject getBridge ()
	{
		if (_androidBridge == null)
			using (var pluginClass = new AndroidJavaClass( AndroidBridge ))
				_androidBridge = pluginClass.CallStatic<AndroidJavaObject> ("getInstance");
		return _androidBridge;
	}

	private readonly static string AndroidBridge = "com.playon.bridge.PlayOnManager";
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void _playOnInitialize(string apiKey, string iosStoreId);
    [DllImport("__Internal")]
    public static extern void _playOnSetEngineInfo(string engineName, string engineVersion);
    [DllImport("__Internal")]
    public static extern bool _playOnIsInitialized();
    [DllImport("__Internal")]
    public static extern void _playOnSetGdprConsent(bool consent);
    [DllImport("__Internal")]
    public static extern void _playOnSetIABUSPrivacyString(string consentString);
    [DllImport("__Internal")]
    public static extern void _playOnClearIABUSPrivacyString();
    [DllImport("__Internal")]
    public static extern bool _playOnGetIsChildDirected();
    [DllImport("__Internal")]
    public static extern float _playOnGetDeviceVolumeLevel();
    [DllImport("__Internal")]
    public static extern void _playOnSetIsChildDirected(bool flag);
    [DllImport("__Internal")]
    public static extern void _playOnSetLogLevel(int level);
    [DllImport("__Internal")]
    public static extern void _playOnRequestTrackingAuthorization();
    [DllImport("__Internal")]
    public static extern void _playOnAddCustomAttribute(string key, string value);
    [DllImport("__Internal")]
    public static extern void _playOnClearCustomAttributes();
    [DllImport("__Internal")]
    public static extern void _playOnRemoveCustomAttribute(string key);
    [DllImport("__Internal")]
    public static extern IntPtr _playOnSetOnInitializationListener(IntPtr callbackRef, PlayOnListener.PlayOnNoArgsDelegateNative onInitialization);
    [DllImport("__Internal")]
    public static extern List<SortedDictionary<String, String>> _playOnGetCustomAttributes();
    [DllImport("__Internal")]
    public static extern List<SortedDictionary<String, String>> _playOnGetCustomAttributes(string key);
#endif

    public static string SDK_VERSION = "2.0.2";

    public enum LogLevel
    {
        None,
        Info,
        Debug
    }
    
    public enum Position
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Centered,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum AdUnitType
    {
        AudioBannerAd,
        AudioRewardedBannerAd,
        AudioLogoAd,
        AudioRewardedLogoAd
    }

    public enum AdUnitRewardType
    {
        InLevel,
        EndLevel
    }

    public enum AdUnitActionButtonType{
        Mute,
        Close,
        None
    }

    public delegate void PlayOnNoArgsDelegate();
    public delegate void PlayOnStateDelegate(bool flag);
    public delegate void PlayOnImpressionDelegate(AdUnit.ImpressionData data);
    public delegate void PlayOnFloatDelegate(float amount);


    public class PlayOnListener
#if UNITY_ANDROID && !UNITY_EDITOR
    : AndroidJavaProxy
#endif
    {
        public PlayOnSDK.PlayOnNoArgsDelegate OnInitializationFinished = () => { };

#if UNITY_ANDROID && !UNITY_EDITOR
        public PlayOnListener() : base ("com.playon.bridge.common.SdkInitializationListener") { }

        void onInitializationFinished () {
            UnityMainThreadDispatcher.Instance().Enqueue( () => OnInitializationFinished() ) ;
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
        public IntPtr playOnNativeListenerRef;

        public delegate void PlayOnNoArgsDelegateNative (IntPtr client);

        private static PlayOnListener IntPtrToClient(IntPtr cl){
            GCHandle handle = (GCHandle)cl;
            return handle.Target as PlayOnListener;
        }

        [MonoPInvokeCallback(typeof(PlayOnNoArgsDelegateNative ))]
        public static void OnInitializationNative(IntPtr client){
            PlayOnListener listener = IntPtrToClient(client);
            listener.OnInitializationFinished();
        }
#endif
    }

    private static PlayOnListener playOnListener = new PlayOnListener();

    public static PlayOnSDK.PlayOnNoArgsDelegate OnInitializationFinished
    {
        get {
            return playOnListener.OnInitializationFinished;
        }
        set {
            playOnListener.OnInitializationFinished = value;
        }
    }

    public static void Initialize(string apiKey, string iosStoreId = "")
    {
        UnityMainThreadDispatcher.Instance();
        string unityVersion = Application.unityVersion;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {   
            getBridge ().Call("setEngineInformation", "unity_" + unityVersion, SDK_VERSION);
            getBridge ().Call("setOnInitializationListener", playOnListener);
            getBridge ().Call("initialize", activity, apiKey);
        }));
#elif UNITY_IOS && !UNITY_EDITOR
        playOnListener.playOnNativeListenerRef = _playOnSetOnInitializationListener((IntPtr)GCHandle.Alloc(playOnListener), PlayOnListener.OnInitializationNative);
        _playOnSetEngineInfo("unity", SDK_VERSION);
        _playOnInitialize(apiKey, iosStoreId);
#else
	Debug.Log("PlayOnSDK. This Platform is not supported. Dummy Initialization");
#endif
    }

    public static bool IsInitialized()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return getBridge ().Call<bool>("isInitialized");
#elif UNITY_IOS && !UNITY_EDITOR
        return _playOnIsInitialized();
#else
	Debug.Log("PlayOnSDK. Dummy Initialization. Default value true");
        return true;
#endif

    }

    [Obsolete("Function is out of date. Will be removed soon")]
    public static void SetTestEnabled(bool flag)
    {
    }

    public static void SetIsChildDirected(bool flag){
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("setIsChildDirected", flag);
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnSetIsChildDirected(flag);
#endif  
    }

     public static bool GetIsChildDirected(){
#if UNITY_ANDROID && !UNITY_EDITOR
        return getBridge ().Call<bool>("getIsChildDirected");
#elif UNITY_IOS && !UNITY_EDITOR
        return _playOnGetIsChildDirected();
#else
        Debug.LogWarning("PlayOnSDK. Editor mode is not supported. Returned value always false");
        return false;
#endif
    }
    
    /// <summary>
    /// Returns current device volume in Percentages from 0 to 100
    /// </summary>
    public static float GetDeviceVolumeLevel(){
#if UNITY_ANDROID && !UNITY_EDITOR
        return getBridge ().Call<float>("getDeviceVolumeLevel");
#elif UNITY_IOS && !UNITY_EDITOR
        return _playOnGetDeviceVolumeLevel();
#else
        Debug.LogWarning("PlayOnSDK. Editor mode is not supported. Returned value always 100");
        return 100.0f;
#endif
    }

    public static void SetGdprConsent(bool consent)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("setGDPRConsent", consent);
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnSetGdprConsent(consent);
#endif
    }

    public static void SetIABUSPrivacyString(string consentString)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("setIABUSPrivacyString", consentString);
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnSetIABUSPrivacyString(consentString);
#endif
    }

    public static void ClearIABUSPrivacyString()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("clearIABUSPrivacyString");
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnClearIABUSPrivacyString();
#endif
    }

    public static void AddCustomAttribute(string key, string value){
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("addCustomAttribute", key, value);
#endif

#if UNITY_IOS && !UNITY_EDITOR
        _playOnAddCustomAttribute(key, value);
#endif
    }

    public static void ClearCustomAttributes(){
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("clearCustomAttributes");
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnClearCustomAttributes();
#endif
    }

    public static void RemoveCustomAttribute(string key){
#if UNITY_ANDROID && !UNITY_EDITOR
        getBridge ().Call("removeCustomAttribute", key);
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnRemoveCustomAttribute(key);
#endif
    }

    public static List<SortedDictionary<String, String>> GetCustomAttributes(){
#if UNITY_ANDROID && !UNITY_EDITOR
        return getBridge ().Call<List<SortedDictionary<String, String>>>("getCustomAttributes");
#elif UNITY_IOS && !UNITY_EDITOR
        return _playOnGetCustomAttributes();
#else
        return new List<SortedDictionary<String, String>>();
#endif
    }

    public static List<SortedDictionary<String, String>> GetCustomAttributes(string key){
#if UNITY_ANDROID && !UNITY_EDITOR
        return getBridge ().Call<List<SortedDictionary<String, String>>>("getCustomAttributes", key);
#elif UNITY_IOS && !UNITY_EDITOR
        return _playOnGetCustomAttributes(key);
#else
        return new List<SortedDictionary<String, String>>();
#endif
    }

    public static void SetLogLevel(LogLevel level)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass typeEnum = new AndroidJavaClass ("com.playon.bridge.common.Log$LogLevel");
        AndroidJavaObject curType = typeEnum.CallStatic<AndroidJavaObject> ("valueOf", level.ToString ());
        getBridge ().Call("setLogLevel", curType);
#elif UNITY_IOS && !UNITY_EDITOR
        _playOnSetLogLevel((int)level);
#endif    
    }

	public static void requestTrackingAuthorization()
	{
#if UNITY_IOS && !UNITY_EDITOR
            _playOnRequestTrackingAuthorization();
#else
            Debug.Log("PlayOnSDK: Requesting tracking authorization is only supported for iOS platform.");
#endif
        }

    public delegate void OnApplicationPause(bool isPaused);
    public static OnApplicationPause onApplicationPause = (isPaused) => { Debug.Log("PlayOnPaused " + isPaused); };

}