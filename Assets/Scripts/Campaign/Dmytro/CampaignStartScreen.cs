using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class CampaignStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject _clearSaveDataText;
    [SerializeField] private Button _ContinueCampaignButton;
    private void Start()
    {
        if (CampaignTracker.Instance)
        {
            bool value = SaveLoadGame.Load();
            _ContinueCampaignButton.interactable = value;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("CampaignTracker.Instance is null");
#endif
        }
    }
    public void StartOrContinueCampaign()
    {
        if (CampaignTracker.Instance)
        {
            //temp
            if (SaveLoadGame.Load())
            {
                if (SaveLoadGame.LoadedData.PlayerLevel == 5)
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
    public void NewCampaign()
    {
        if (CampaignTracker.Instance)
        {
            StorageData emptyData = new StorageData();
            SaveLoadGame.Save(emptyData);
            if (SaveLoadGame.Load())
            {
                CampaignTracker.Instance.StartCampaign();
            }
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
