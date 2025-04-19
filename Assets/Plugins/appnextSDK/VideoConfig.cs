using UnityEngine;

namespace appnext
{
	public abstract class VideoConfig : AdConfig
	{
		public string progressType = Video.PROGRESS_CLOCK;
		public string progressColor;
		public string videoLength;
		public long delay;
		public bool showClose = false;

		#if UNITY_ANDROID

		protected override void initParamsFromAndroidJavaObject(AndroidJavaObject instance)
		{
			base.initParamsFromAndroidJavaObject(instance);
			this.progressType = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getProgressType", "()Ljava/lang/String"), new jvalue[0] { });
			this.videoLength = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getVideoLength", "()Ljava/lang/String"), new jvalue[0] { });
			this.progressColor = AndroidJNI.CallStringMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getProgressColor", "()Ljava/lang/String"), new jvalue[0] { });
			this.delay = AndroidJNI.CallLongMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "getCloseDelay", "()J"), new jvalue[0] { });
			this.showClose = AndroidJNI.CallBooleanMethod (instance.GetRawObject (), AndroidJNIHelper.GetMethodID (instance.GetRawClass (), "isShowClose", "()Z"), new jvalue[0] { });
		}

		#elif UNITY_IPHONE

		protected override void initConfig()
		{
			base.initConfig ();

			this.progressType = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_PROGRESS_TYPE);
			this.progressColor = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_PROGRESS_COLOR);
			this.videoLength = AppnextIOSSDKBridge.getConfigDefaultString(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_VIDEO_LENGTH);
			this.delay = (long)(AppnextIOSSDKBridge.getConfigDefaultDouble(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_DELAY) * 1000);
			this.showClose = AppnextIOSSDKBridge.getConfigDefaultBool(AppnextIOSSDKBridge.AD_CONFIG_DEFAULT_SHOW_CLOSE);
		}

		#endif

		public void setProgressType(string progressType)
		{
			this.progressType = progressType;
		}

		public string getProgressType()
		{
			return this.progressType;
		}

		public void setProgressColor(string progressColor)
		{
			if (progressColor == null) {
				progressColor = "";
			}
			this.progressColor = progressColor;
		}

		public string getProgressColor()
		{
			return this.progressColor;
		}

		public void setVideoLength(string videoLength)
		{
			this.videoLength = videoLength;
		}

		public string getVideoLength()
		{
			return this.videoLength;
		}

		public void setShowClose(bool showClose)
		{
			this.showClose = showClose;
		}

		public void setShowClose(bool showClose, long delay)
		{
			this.showClose = showClose;
			this.delay = delay;
		}

		public bool isShowClose()
		{
			return this.showClose;
		}

		public long getCloseDelay()
		{
			return this.delay;
		}
	}
}
