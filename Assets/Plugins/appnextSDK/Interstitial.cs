using UnityEngine;
using System.Collections;
using System;

namespace appnext
{
	public class Interstitial : Ad
    {
        public const string TYPE_MANAGED = "managed";
        public const string TYPE_VIDEO = "video";
        public const string TYPE_STATIC = "static";

		public Interstitial(string placementID) : base(placementID)
		{
		}

		public Interstitial(string placementID, InterstitialConfig config) : base(placementID, config)
        {
            setCreativeType(config.getCreativeType());
            setAutoPlay(config.isAutoPlay());
            setSkipText(config.getSkipText());
        }

		protected override void initAd(string placementID)
		{
			#if UNITY_ANDROID
			createInstance("getInterstitial", placementID);
			#elif UNITY_IPHONE
			this.adKey = AppnextIOSSDKBridge.createAd(AppnextIOSSDKBridge.AD_TYPE_INTERSTITIAL, placementID);
			#endif
		}

		public void setSkipText(string skipText)
        {
			#if UNITY_ANDROID
			instance.Call("setSkipText", skipText);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setSkipText(adKey, skipText);
			#endif
        }

        public void setCreativeType(string creativeType)
        {
			#if UNITY_ANDROID
			instance.Call("setCreativeType", creativeType);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setCreativeType(adKey, creativeType);
			#endif
        }

        public void setAutoPlay(bool autoPlay)
        {
			#if UNITY_ANDROID
			instance.Call("setAutoPlay", autoPlay);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setAutoPlay(adKey, autoPlay);
			#endif
        }
    }
}
