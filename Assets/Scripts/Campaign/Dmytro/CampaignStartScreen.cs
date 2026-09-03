using SaveSystem;
using UnityEngine;

public class CampaignStartScreen : MonoBehaviour
{
    public void StartOrContinueCampaign()
    {
        if (CampaignTracker.Instance)
        {
            CampaignTracker.Instance.StartCampaign();
        }
        else
        {
            #if UNITY_EDITOR
                        Debug.LogError("CampaignTracker.Instance is null");
            #endif
        }
    }
    public void ClearSaveData()
    {
        StorageData emptyData = new StorageData();
        SaveLoadGame.Save(emptyData);
    }
}
