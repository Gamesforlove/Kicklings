using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace appnext
{
	public class AppnextIOSSDKBridge : MonoBehaviour
	{
		public const string AD_TYPE_INTERSTITIAL = "interstitial";
		public const string AD_TYPE_FULL_SCREEN_VIDEO = "fullscreen";
		public const string AD_TYPE_REWARDED_VIDEO = "rewarded";

		// All Ads Config
		public const string AD_CONFIG_DEFAULT_BUTTON_TEXT = "buttonText";
		public const string AD_CONFIG_DEFAULT_BUTTON_COLOR = "buttonColor";
		public const string AD_CONFIG_DEFAULT_CATEGORIES = "categories";
		public const string AD_CONFIG_DEFAULT_POSTBACK = "postback";
		public const string AD_CONFIG_DEFAULT_ORIENTATION = "orientation";
		public const string AD_CONFIG_DEFAULT_MUTE = "mute";

		// Interstitial Ads Config
		public const string AD_CONFIG_DEFAULT_CREATIVE_TYPE = "creativeType";
		public const string AD_CONFIG_DEFAULT_SKIP_TEXT = "skipText";
		public const string AD_CONFIG_DEFAULT_AUTO_PLAY = "autoPlay";

		// Video Ads Config
		public const string AD_CONFIG_DEFAULT_PROGRESS_TYPE = "progressType";
		public const string AD_CONFIG_DEFAULT_PROGRESS_COLOR = "progressColor";
		public const string AD_CONFIG_DEFAULT_VIDEO_LENGTH = "videoLength";
		public const string AD_CONFIG_DEFAULT_DELAY = "delay";
		public const string AD_CONFIG_DEFAULT_SHOW_CLOSE = "showClose";

		#if UNITY_IPHONE

		// Global SDK Methods

		[DllImport ("__Internal")]
		private static extern string appnextGetApiVersion();

		// Config default getters

		[DllImport ("__Internal")]
		private static extern string appnextGetConfigDefaultString(string configParamName);

		[DllImport ("__Internal")]
		private static extern bool appnextGetConfigDefaultBool(string configParamName);

		[DllImport ("__Internal")]
		private static extern double appnextGetConfigDefaultDouble(string configParamName);

		// Ad Managment Methods

		[DllImport ("__Internal")]
		private static extern string appnextCreateAd(string adTypeName, string placementID);

		[DllImport ("__Internal")]
		private static extern bool appnextDisposeAd(string adKey);

		[DllImport ("__Internal")]
		private static extern void appnextLoadAd(string adKey);

		[DllImport ("__Internal")]
		private static extern void appnextShowAd(string adKey);

		[DllImport ("__Internal")]
		private static extern bool appnextAdIsLoaded(string adKey);

		// Setter Methods - All Ads

		[DllImport ("__Internal")]
		private static extern void appnextSetButtonText(string adKey, string buttonText);

		[DllImport ("__Internal")]
		private static extern void appnextSetButtonColor(string adKey, string buttonColor);

		[DllImport ("__Internal")]
		private static extern void appnextSetCategories(string adKey, string categories);

		[DllImport ("__Internal")]
		private static extern void appnextSetPostback(string adKey, string postback);

		[DllImport ("__Internal")]
		private static extern void appnextSetOrientation(string adKey, string orientation);

		[DllImport ("__Internal")]
		private static extern void appnextSetMute(string adKey, bool mute);

		[DllImport ("__Internal")]
		private static extern void appnextSetSkipText(string adKey, string skipText);

		[DllImport ("__Internal")]
		private static extern void appnextSetCreativeType(string adKey, string creativeType);

		[DllImport ("__Internal")]
		private static extern void appnextSetAutoPlay(string adKey, bool autoPlay);

		// Setter Methods - Video Ads

		[DllImport ("__Internal")]
		private static extern void appnextSetProgressType(string adKey, string progressType);

		[DllImport ("__Internal")]
		private static extern void appnextSetProgressColor(string adKey, string progressColor);

		[DllImport ("__Internal")]
		private static extern void appnextSetVideoLength(string adKey, string videoLength);

		[DllImport ("__Internal")]
		private static extern void appnextSetShowClose(string adKey, bool showClose);

		[DllImport ("__Internal")]
		private static extern void appnextSetCloseDelay(string adKey, double delay);

		// Setter Methods - Rewarded Video Ads

		[DllImport ("__Internal")]
		private static extern void appnextSetRewardsTransactionId(string adKey, string rewardsTransactionId);

		[DllImport ("__Internal")]
		private static extern void appnextSetRewardsUserId(string adKey, string rewardsUserId);

		[DllImport ("__Internal")]
		private static extern void appnextSetRewardsRewardTypeCurrency(string adKey, string rewardsRewardTypeCurrency);

		[DllImport ("__Internal")]
		private static extern void appnextSetRewardsAmountRewarded(string adKey, string rewardsAmountRewarded);

		[DllImport ("__Internal")]
		private static extern void appnextSetRewardsCustomParameter(string adKey, string rewardsCustomParameter);

		// Callback Methods - All Ads Callbacks

		[DllImport ("__Internal")]
		private static extern void appnextSetOnAdLoadedCallback(string adKey, string gameObject, string method);

		[DllImport ("__Internal")]
		private static extern void appnextSetOnAdOpenedCallback(string adKey, string gameObject, string method);

		[DllImport ("__Internal")]
		private static extern void appnextSetOnAdClickedCallback(string adKey, string gameObject, string method);

		[DllImport ("__Internal")]
		private static extern void appnextSetOnAdClosedCallback(string adKey, string gameObject, string method);

		[DllImport ("__Internal")]
		private static extern void appnextSetOnAdErrorCallback(string adKey, string gameObject, string method);

		// Callback Methods - Video Ads Callback

		[DllImport ("__Internal")]
		private static extern void appnextSetOnVideoEndedCallback(string adKey, string gameObject, string method);

		// Public Bridge Methods - Config default getters

		public static string getConfigDefaultString(string configParamName) {
			return appnextGetConfigDefaultString(configParamName);
		}

		public static bool getConfigDefaultBool(string configParamName) {
			return appnextGetConfigDefaultBool(configParamName);
		}

		public static double getConfigDefaultDouble(string configParamName) {
			return appnextGetConfigDefaultDouble(configParamName);
		}

		// Public Bridge Methods - All Ads

		public static string createAd(string adTypeName, string placementID) {
			return appnextCreateAd(adTypeName, placementID);
		}

		public static void disposeAd(string adKey) {
			appnextDisposeAd(adKey);
		}

		public static void loadAd(string adKey) {
			appnextLoadAd(adKey);
		}

		public static void showAd(string adKey) {
			appnextShowAd(adKey);
		}

		public static bool adIsLoaded(string adKey) {
			return appnextAdIsLoaded(adKey);
		}

		public static void setButtonText(string adKey, string buttonText) {
			appnextSetButtonText(adKey, buttonText);
		}

		public static void setButtonColor(string adKey, string buttonColor) {
			appnextSetButtonColor(adKey, buttonColor);
		}

		public static void setCategories(string adKey, string categories) {
			appnextSetCategories(adKey, categories);
		}

		public static void setPostback(string adKey, string postback) {
			appnextSetPostback(adKey, postback);
		}

		public static void setOrientation(string adKey, string orientation) {
			appnextSetOrientation(adKey, orientation);
		}

		public static void setMute(string adKey, bool mute) {
			appnextSetMute(adKey, mute);
		}

		// Public Bridge Methods - Interstitial Ads

		public static void setSkipText(string adKey, string skipText) {
			appnextSetSkipText(adKey, skipText);
		}

		public static void setCreativeType(string adKey, string creativeType) {
			appnextSetCreativeType(adKey, creativeType);
		}

		public static void setAutoPlay(string adKey, bool autoPlay) {
			appnextSetAutoPlay(adKey, autoPlay);
		}

		// Public Bridge Methods - Video Ads

		public static void setProgressType(string adKey, string progressType) {
			appnextSetProgressType(adKey, progressType);
		}

		public static void setProgressColor(string adKey, string progressColor) {
			appnextSetProgressColor(adKey, progressColor);
		}

		public static void setVideoLength(string adKey, string videoLength) {
			appnextSetVideoLength(adKey, videoLength);
		}

		public static void setShowClose(string adKey, bool showClose) {
			appnextSetShowClose(adKey, showClose);
		}

		public static void setCloseDelay(string adKey, double delay) {
			appnextSetCloseDelay(adKey, delay);
		}

		// Public Bridge Methods - Rewarded Video Ads

		public static void setRewardsTransactionId(string adKey, string rewardsTransactionId) {
			appnextSetRewardsTransactionId(adKey, rewardsTransactionId);
		}

		public static void setRewardsUserId(string adKey, string rewardsUserId) {
			appnextSetRewardsUserId(adKey, rewardsUserId);
		}

		public static void setRewardsRewardTypeCurrency(string adKey, string rewardsRewardTypeCurrency) {
			appnextSetRewardsRewardTypeCurrency(adKey, rewardsRewardTypeCurrency);
		}

		public static void setRewardsAmountRewarded(string adKey, string rewardsAmountRewarded) {
			appnextSetRewardsAmountRewarded(adKey, rewardsAmountRewarded);
		}

		public static void setRewardsCustomParameter(string adKey, string rewardsCustomParameter) {
			appnextSetRewardsCustomParameter(adKey, rewardsCustomParameter);
		}

		// Public Bridge Methods - Ads Callbacks

		public static void setOnAdLoadedCallback(string adKey, string gameObject, string method) {
			appnextSetOnAdLoadedCallback(adKey, gameObject, method);
		}

		public static void setOnAdOpenedCallback(string adKey, string gameObject, string method) {
			appnextSetOnAdOpenedCallback(adKey, gameObject, method);
		}

		public static void setOnAdClickedCallback(string adKey, string gameObject, string method) {
			appnextSetOnAdClickedCallback(adKey, gameObject, method);
		}

		public static void setOnAdClosedCallback(string adKey, string gameObject, string method) {
			appnextSetOnAdClosedCallback(adKey, gameObject, method);
		}

		public static void setOnAdErrorCallback(string adKey, string gameObject, string method) {
			appnextSetOnAdErrorCallback(adKey, gameObject, method);
		}

		// Public Bridge Methods - Video Ads Callback

		public static void setOnVideoEndedCallback(string adKey, string gameObject, string method) {
			appnextSetOnVideoEndedCallback(adKey, gameObject, method);
		}

		#endif
	}
}
