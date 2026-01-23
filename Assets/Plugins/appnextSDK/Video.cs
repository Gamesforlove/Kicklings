using UnityEngine;
using System.Collections;

namespace appnext
{
	public abstract class Video : Ad
	{
		public const string PROGRESS_CLOCK = "clock";
		public const string PROGRESS_BAR = "bar";
		public const string PROGRESS_NONE = "none";
		public const string PROGRESS_DEFAULT = "default";

		public const string VIDEO_LENGTH_SHORT = "15";
		public const string VIDEO_LENGTH_LONG = "30";
		public const string VIDEO_LENGTH_DEFAULT = "default";

		public delegate void OnVideoEndedDelegate(Video ad);
		public event OnVideoEndedDelegate onVideoEndedDelegate;

		protected Video(string placementID) : base(placementID)
		{
		}

		protected Video(string placementID, VideoConfig config) : base(placementID, config)
		{
			setProgressColor(config.getProgressColor());
			setProgressType(config.getProgressType());
			if (config.getCloseDelay () >= 0) {
				setShowClose(config.isShowClose(), config.getCloseDelay());
			} else {
				setShowClose(config.isShowClose());
			}
			setVideoLength(config.getVideoLength());
		}

		protected override void registerAdGameObjectForEvents()
		{
			base.registerAdGameObjectForEvents();
			setOnVideoEndedCallback(this.adGameObject.name, "onVideoEndedCallback");
		}

		private void setOnVideoEndedCallback(string gameObject, string method)
		{
			#if UNITY_ANDROID
			instance.Call("setOnVideoEndedCallback", gameObject, method);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setOnVideoEndedCallback(adKey, gameObject, method);
			#endif
		}

		private void onVideoEndedCallback(string message)
		{
			Ad thisAd;
			bool success = Ad.gameObjectNameToAdDictionary.TryGetValue(this.name, out thisAd);
			if (success) {
				if (((Video)thisAd).onVideoEndedDelegate != null) {
					((Video)thisAd).onVideoEndedDelegate(((Video)thisAd));
				}
			}
		}

		public void setProgressType(string progressType)
		{
			#if UNITY_ANDROID
			instance.Call("setProgressType", progressType);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setProgressType(adKey, progressType);
			#endif
		}

		public void setProgressColor(string progressColor)
		{
			#if UNITY_ANDROID
			instance.Call("setProgressColor", progressColor);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setProgressColor(adKey, progressColor);
			#endif
		}

		public void setVideoLength(string videoLength)
		{
			#if UNITY_ANDROID
			instance.Call("setVideoLength", videoLength);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setVideoLength(adKey, videoLength);
			#endif
		}

		public void setShowClose(bool showClose)
		{
			#if UNITY_ANDROID
			instance.Call("setShowClose", showClose);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setShowClose(adKey, showClose);
			#endif
		}

		public void setShowClose(bool showClose, long delay)
		{
			#if UNITY_ANDROID
			instance.Call("setShowClose", showClose, delay);
			#elif UNITY_IPHONE
			double tDelay = delay / 1000;
			AppnextIOSSDKBridge.setShowClose(adKey, showClose);
			AppnextIOSSDKBridge.setCloseDelay(adKey, tDelay);
			#endif
		}
	}
}
