using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class CampaignStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject _clearSaveDataText;
    [SerializeField] private Button _ContinueCampaignButton;
    [SerializeField] private CampaignStructure _campaign;
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
    public void ContinueCampaign()
    {
        if (!LoadCampaign())
        {
            _clearSaveDataText.SetActive(true);
        }
    }
    public void NewCampaign()
    {
        try
        {
            StorageData emptyData = new StorageData();
            SaveLoadGame.Save(emptyData);
            LoadCampaign();
        }
        catch (System.Exception e)
        {
            _clearSaveDataText.SetActive(true);

            #if UNITY_EDITOR
                Debug.LogError(e);
                throw;
            #endif
        }
    }
    private bool LoadCampaign()
    {
        if (SaveLoadGame.Load())
        {
            if (_campaign.CurrentStage == null)// need to update endofcampaign check
            {
                #if UNITY_EDITOR
                    Debug.LogError("Current stage index is out of range");
                #endif
                return false;
            }
            CampaignTracker.Instance.StartCampaign();
            return true;
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Could not load save file");
            #endif
        }
        return false;
    }
    public void ClearSaveData()
    {
        StorageData emptyData = new StorageData();
        SaveLoadGame.Save(emptyData);
    }
}
