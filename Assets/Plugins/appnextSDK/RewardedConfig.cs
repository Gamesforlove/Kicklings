using UnityEngine;

namespace appnext
{
	public class RewardedConfig : VideoConfig
    {
		public RewardedConfig() : base()
        {
			#if UNITY_ANDROID
			AndroidJavaObject instance = new AndroidJavaObject("com.appnext.ads.fullscreen.RewardedConfig");
			initParamsFromAndroidJavaObject(instance);
			#elif UNITY_IPHONE
			#endif
        }
    }
}
