using AudioSystem;
using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

public class LoadCampaign : MonoBehaviour
{
    public void LoadCampaignScene()
    {
        MusicManager.Instance.StopMusic();
        EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.TestCampaignStart));
    }
}
