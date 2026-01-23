using UnityEngine;
using System.Collections;

namespace appnext
{
	public class FullscreenVideo : Video
    {
		public FullscreenVideo(string placementID) : base(placementID)
        {
        }

		public FullscreenVideo(string placementID, FullscreenConfig config) : base(placementID, config)
        {
        }

		protected override void initAd(string placementID)
		{
			#if UNITY_ANDROID
			createInstance("getFullScreenVideo", placementID);
			#elif UNITY_IPHONE
			this.adKey = AppnextIOSSDKBridge.createAd(AppnextIOSSDKBridge.AD_TYPE_FULL_SCREEN_VIDEO, placementID);
			#endif
		}
    }
}
