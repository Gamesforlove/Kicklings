using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;

namespace appnext
{
	public abstract class Ad : MonoBehaviour, IDisposable
	{
		public const string ORIENTATION_DEFAULT = "not_set";
		public const string ORIENTATION_LANDSCAPE = "landscape";
		public const string ORIENTATION_PORTRAIT = "portrait";
		public const string ORIENTATION_AUTO = "automatic";

		protected class WeakDictionary<TKey, TValue> where TValue : class
		{
			private readonly Dictionary<TKey, WeakReference> items;

			public WeakDictionary() {
				this.items = new Dictionary<TKey, WeakReference>();
			}

			public void Put(TKey key, TValue value) {
				this.items[key] = new WeakReference(value);
			}

			public bool Remove(TKey key) {
				WeakReference weakRef;
				if (!this.items.TryGetValue(key, out weakRef)) {
					return false;
				} else {
					this.items.Remove(key);
					return weakRef.IsAlive;
				}
			}

			public bool TryGetValue(TKey key, out TValue value) {
				WeakReference weakRef;
				if (!this.items.TryGetValue(key, out weakRef)) {
					value = null;
					return false;
				} else {
					value = (TValue)weakRef.Target;
					return (value != null);
				}
			}
		}

		public delegate void OnAdLoadedDelegate(Ad ad);
		public event OnAdLoadedDelegate onAdLoadedDelegate;

		public delegate void OnAdOpenedDelegate(Ad ad);
		public event OnAdOpenedDelegate onAdOpenedDelegate;

		public delegate void OnAdClickedDelegate(Ad ad);
		public event OnAdClickedDelegate onAdClickedDelegate;

		public delegate void OnAdClosedDelegate(Ad ad);
		public event OnAdClosedDelegate onAdClosedDelegate;

		public delegate void OnAdErrorDelegate(Ad ad, string error);
		public event OnAdErrorDelegate onAdErrorDelegate;

		protected GameObject adGameObject; // needs to be destroyed when the class is disposed
		// stuff for disposables
		private bool disposed = false;

		#if UNITY_ANDROID

		protected AndroidJavaObject instance;

		#elif UNITY_IPHONE

		protected string adKey;

		#endif

		private static int gameObjectCount = 0;
		protected static WeakDictionary<string, Ad> gameObjectNameToAdDictionary = new WeakDictionary<string, Ad>();

		protected Ad(string placementID)
		{
			initAd(placementID);
			createGameObject();
			registerAdGameObjectForEvents();
		}

		protected Ad(string placementID, AdConfig config) : this(placementID)
		{
			setButtonText(config.getButtonText());
			setButtonColor(config.getButtonColor());
			setCategories(config.getCategories());
			setPostback(config.getPostback());
			setOrientation(config.getOrientation());
			setMute(config.getMute());
			setBackButtonCanClose(config.isBackButtonCanClose());
		}

		// destructor
		~Ad()
		{
			Dispose();
		}

		//
		// IDisposable
		//
		public void Dispose()
		{
			if(!this.disposed) {
				#if UNITY_ANDROID
				#elif UNITY_IPHONE
				AppnextIOSSDKBridge.disposeAd(adKey);
				#endif
			}
			this.disposed = true;
		}

		protected abstract void initAd(string placementID);

		private void createGameObject()
		{
			string gameObjectName = Ad.CreateAdGameObjectName();
			adGameObject = new GameObject(gameObjectName);
			GameObject.DontDestroyOnLoad(adGameObject);
			adGameObject.AddComponent(GetType());
			Ad.gameObjectNameToAdDictionary.Put(adGameObject.name, this);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static string CreateAdGameObjectName()
		{
			string gameObjectName = "AppnextUnityAdGameObject_" + gameObjectCount.ToString();
			gameObjectCount++;
			return gameObjectName;
		}

		protected virtual void registerAdGameObjectForEvents()
		{
			setOnAdLoadedCallback(adGameObject.transform.name, "onAdLoadedCallback");
			setOnAdOpenedCallback(adGameObject.transform.name, "onAdOpenedCallback");
			setOnAdClickedCallback(adGameObject.transform.name, "onAdClickedCallback");
			setOnAdClosedCallback(adGameObject.transform.name, "onAdClosedCallback");
			setOnAdErrorCallback(adGameObject.transform.name, "onAdErrorCallback");
		}

		private void setOnAdLoadedCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnAdLoadedCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnAdLoadedCallback(adKey, gameObject, method);
			#endif
		}

		private void setOnAdOpenedCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnAdOpenedCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnAdOpenedCallback(adKey, gameObject, method);
			#endif
		}

		private void setOnAdClickedCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnAdClickedCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnAdClickedCallback(adKey, gameObject, method);
			#endif
		}

		private void setOnAdClosedCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnAdClosedCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnAdClosedCallback(adKey, gameObject, method);
			#endif
		}

		private void setOnAdErrorCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnAdErrorCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnAdErrorCallback(adKey, gameObject, method);
			#endif
		}

		private void onAdLoadedCallback(string message)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (thisAd.onAdLoadedDelegate != null) {
					thisAd.onAdLoadedDelegate(thisAd);
				}
			}
		}

		private void onAdOpenedCallback(string message)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (thisAd.onAdOpenedDelegate != null) {
					thisAd.onAdOpenedDelegate(thisAd);
				}
			}
		}

		private void onAdClickedCallback(string message)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (thisAd.onAdClickedDelegate != null) {
					thisAd.onAdClickedDelegate(thisAd);
				}
			}
		}

		private void onAdClosedCallback(string message)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (thisAd.onAdClosedDelegate != null) {
					thisAd.onAdClosedDelegate(thisAd);
				}
			}
		}

		private void onAdErrorCallback(string error)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (thisAd.onAdErrorDelegate != null) {
					thisAd.onAdErrorDelegate(thisAd, error);
				}
			}
		}

		#if UNITY_ANDROID

		protected void createInstance(string instanceFuncName, string placementID)
		{
			if (Application.platform == RuntimePlatform.Android) {
				using (var pluginClass = new AndroidJavaClass ("com.appnext.unityplugin.AdBuilder"))
					instance = pluginClass.CallStatic<AndroidJavaObject> (instanceFuncName, placementID);
			}
		}

		#endif

		public void loadAd()
		{
			#if UNITY_ANDROID
			instance.Call("loadAd");
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.loadAd(adKey);
			#endif
		}

		public void showAd()
		{
			#if UNITY_ANDROID
			instance.Call("showAd");
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.showAd(adKey);
			#endif
		}

		public bool isAdLoaded()
		{
			#if UNITY_ANDROID
			return AndroidJNI.CallBooleanMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "isAdLoaded", "()Z"), new jvalue[0] { });
			#elif UNITY_IPHONE
			return AppnextIOSSDKBridge.adIsLoaded(adKey);
			#else
			return false;
			#endif
		}

		public void setButtonText(string buttonText)
		{
			#if UNITY_ANDROID
			instance.Call("setButtonText", buttonText);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setButtonText(adKey, buttonText);
			#endif
		}

		public void setButtonColor(string buttonColor)
		{
			#if UNITY_ANDROID
			instance.Call("setButtonColor", buttonColor);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setButtonColor(adKey, buttonColor);
			#endif
		}

		public void setCategories(string categories)
		{
			#if UNITY_ANDROID
			instance.Call("setCategories", categories);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setCategories(adKey, categories);
			#endif
		}

		public void setPostback(string postback)
		{
			#if UNITY_ANDROID
			instance.Call("setPostback", postback);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setPostback(adKey, postback);
			#endif
		}

		public void setOrientation(string orientation)
		{
			#if UNITY_ANDROID
			instance.Call("setOrientation", orientation);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOrientation(adKey, orientation);
			#endif
		}

		public void setMute(bool mute)
		{
			#if UNITY_ANDROID
			instance.Call("setMute", mute);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setMute(adKey, mute);
			#endif
		}

		public void setBackButtonCanClose(bool backButtonCanClose)
		{
			#if UNITY_ANDROID
			instance.Call("setBackButtonCanClose", backButtonCanClose);
			#elif UNITY_IPHONE
			// iOS doesn't have a back button so do nothing
			#endif
		}
	}
}
