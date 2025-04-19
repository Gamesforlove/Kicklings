using UnityEngine;

namespace appnext
{
	public class FullscreenConfig : VideoConfig
    {
		public FullscreenConfig() : base()
        {
			#if UNITY_ANDROID
			AndroidJavaObject instance = new AndroidJavaObject ("com.appnext.ads.fullscreen.FullscreenConfig");
			initParamsFromAndroidJavaObject(instance);
			#elif UNITY_IPHONE
			#endif
		}
    }
}
