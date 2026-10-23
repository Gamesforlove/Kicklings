using CommonDataTypes;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMapLevel : MonoBehaviour
{
    [SerializeField] private CampaignMapController _campaignController;
    [SerializeField] private Button _startMatch, _levelButton;
    public RectTransform RectTransform;
    public RectTransform CharacterPoint;
    public CampaignLevelData _data;
    private void Awake()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();
    }
    public void EnableLevel()
    {
        _levelButton.interactable = true;
    }
    public void EnableStartButton()
    {
        _startMatch.gameObject.SetActive(true);
    }
    public void DisableStartButton()
    {
        _startMatch.gameObject.SetActive(false);
    }
    public void InitializeLevelData(CampaignLevelData data)
    {
        _data = data;
    }
    public void StartMatch()
    {
        _campaignController.StartMatch(_data);
    }
}
