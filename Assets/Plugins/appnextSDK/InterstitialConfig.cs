using UnityEngine;

namespace appnext
{
	public class InterstitialConfig : AdConfig
    {
        public string creativeType = Interstitial.TYPE_MANAGED;
        public string skipText = "Skip";
        public bool autoPlay = true;

		public InterstitialConfig() : base()
        {
			#if UNITY_ANDROID
			AndroidJavaObject instance = new AndroidJavaObject ("com.appnext.ads.interstitial.InterstitialConfig");
			initParamsFromAndroidJavaObject(instance);
			#elif UNITY_IPHONE
			#endif
		}

		#if UNITY_ANDROID

		protected override void initParamsFromAndroidJavaObject(AndroidJavaObject instance)
		{
			base.initParamsFromAndroidJavaObject(instance);
			this.creativeType = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getCreativeType", "()Ljava/lang/String"), new jvalue[0] { });
			this.skipText = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getSkipText", "()Ljava/lang/String"), new jvalue[0] { });
			this.autoPlay = AndroidJNI.CallBooleanMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "isAutoPlay", "()Z"), new jvalue[0] { });
		}

		#elif UNITY_IPHONE

		protected override void initConfig()
		{
			base.initConfig ();

			this.creativeType = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_CREATIVE_TYPE);
			this.skipText = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_SKIP_TEXT);
			this.autoPlay = AppnextIOSSDKBridge.getConfigDefaultBool(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_AUTO_PLAY);
		}

		#endif

        public void setCreativeType(string creativeType)
        {
            this.creativeType = creativeType;
        }

        public string getCreativeType()
        {
			return this.creativeType;
        }

		public void setSkipText(string skip)
		{
			if (skip == null) {
				skip = "";
			}
			this.skipText = skip;
		}

		public string getSkipText()
		{
			return this.skipText;
		}

		public bool isAutoPlay()
        {
			return this.autoPlay;
        }

        public void setAutoPlay(bool autoPlay)
        {
            this.autoPlay = autoPlay;
        }
    }
}
