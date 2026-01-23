using UnityEngine;

namespace appnext
{
	public abstract class AdConfig
	{
		public string buttonText = "";
		public string buttonColor = "";
		public string categories = "";
		public string postback = "";
		public string orientation = "";
		public bool backButtonCanClose = false;
		public bool mute = false;

		public AdConfig()
		{
			#if UNITY_ANDROID
			#elif UNITY_IPHONE
			initConfig();
			#endif
		}

		#if UNITY_ANDROID

		protected virtual void initParamsFromAndroidJavaObject(AndroidJavaObject instance)
		{
			this.buttonText = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getButtonText", "()Ljava/lang/String"), new jvalue[0] { });
			this.buttonColor = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getButtonColor", "()Ljava/lang/String"), new jvalue[0] { });
			this.categories = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getCategories", "()Ljava/lang/String"), new jvalue[0] { });
			this.postback = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getPostback", "()Ljava/lang/String"), new jvalue[0] { });
			this.orientation = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getOrientation", "()Ljava/lang/String"), new jvalue[0] { });
			this.backButtonCanClose = AndroidJNI.CallBooleanMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "isBackButtonCanClose", "()Z"), new jvalue[0] { });
			this.mute = AndroidJNI.CallBooleanMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getMute", "()Z"), new jvalue[0] { });
		}

		#elif UNITY_IPHONE

		protected virtual void initConfig()
		{
			this.buttonText = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_BUTTON_TEXT);
			this.buttonColor = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_BUTTON_COLOR);
			this.categories = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_CATEGORIES);
			this.postback = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_POSTBACK);
			this.orientation = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_ORIENTATION);
			this.backButtonCanClose = false;
			this.mute = AppnextIOSSDKBridge.getConfigDefaultBool(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_MUTE);
		}

		#endif

		public void setButtonText(string buttonText)
		{
			if (buttonText == null) {
				buttonText = "";
			}
			this.buttonText = buttonText;
		}

		public string getButtonText()
		{
			return this.buttonText;
		}

		public void setButtonColor(string buttonColor)
		{
			if (buttonColor == null) {
				buttonColor = "";
			}
			this.buttonColor = buttonColor;
		}

		public string getButtonColor()
		{
			return this.buttonColor;
		}

		public void setCategories(string category)
		{
			if (category == null) {
				category = "";
			}
			this.categories = category;
		}

		public string getCategories()
		{
			return this.categories;
		}

		public void setPostback(string postback)
		{
			if (postback == null) {
				postback = "";
			}
			this.postback = postback;
		}

		public string getPostback()
		{
			return this.postback;
		}

		public void setOrientation(string orientation)
		{
			this.orientation = orientation;
		}

		public string getOrientation()
		{
			return this.orientation;
		}

		public void setBackButtonCanClose(bool canClose)
		{
			this.backButtonCanClose = canClose;
		}

		public bool isBackButtonCanClose()
		{
			return this.backButtonCanClose;
		}

		public void setMute(bool mute)
		{
			this.mute = mute;
		}

		public bool getMute()
		{
			return this.mute;
		}
	}
}
