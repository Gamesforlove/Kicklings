using UnityEngine;
using System.Collections;

namespace appnext
{
	public class RewardedVideo : Video
    {
		public RewardedVideo(string placementID) : base(placementID)
        {
        }

		public RewardedVideo(string placementID, RewardedConfig config) : base(placementID, config)
        {
        }

		protected override void initAd(string placementID)
		{
			#if UNITY_ANDROID
			createInstance("getRewardedVideo", placementID);
			#elif UNITY_IPHONE
			this.adKey = AppnextIOSSDKBridge.createAd(AppnextIOSSDKBridge.AD_TYPE_REWARDED_VIDEO, placementID);
			#endif
		}

		public void setRewardedServerSidePostback(string rewardsTransactionId, string rewardsUserId,
                                                  string rewardsRewardTypeCurrency, string rewardsAmountRewarded,
                                                  string rewardsCustomParameter)
        {
			#if UNITY_ANDROID
			instance.Call("setRewardedServerSidePostback", rewardsTransactionId, rewardsUserId, rewardsRewardTypeCurrency, rewardsAmountRewarded, rewardsCustomParameter);
			#elif UNITY_IPHONE
			AppnextIOSSDKBridge.setRewardsTransactionId(adKey, rewardsTransactionId);
			AppnextIOSSDKBridge.setRewardsUserId(adKey, rewardsUserId);
			AppnextIOSSDKBridge.setRewardsRewardTypeCurrency(adKey, rewardsRewardTypeCurrency);
			AppnextIOSSDKBridge.setRewardsAmountRewarded(adKey, rewardsAmountRewarded);
			AppnextIOSSDKBridge.setRewardsCustomParameter(adKey, rewardsCustomParameter);
			#endif
        }
    }
}
