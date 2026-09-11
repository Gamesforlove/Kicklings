using SaveSystem;
using UnityEngine;

public class CampaignStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject _clearSaveDataText;
    public void StartOrContinueCampaign()
    {
        if (CampaignTracker.Instance)
        {
            //temp
            if (SaveLoadGame.Load())
            {
                if (SaveLoadGame.LoadedData.PlayerLevel == 4)
                {
                    _clearSaveDataText.SetActive(true);
                    return;
                }
            }
            //temp
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
