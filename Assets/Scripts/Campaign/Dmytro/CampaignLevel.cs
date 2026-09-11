using CommonDataTypes;
using UnityEngine;
using UnityEngine.UI;

public class CampaignLevel : MonoBehaviour
{
    [SerializeField] private GameObject _opponent1, _opponent2;
    [SerializeField] private CampaignMapController _campaignController;
    [SerializeField] private Button _startMatch, levelButton;
    [SerializeField] private SceneName _preMatchCutScene = SceneName.None;
    [SerializeField] private SceneName _afterMatchCutScene = SceneName.None;
    public RectTransform RectTransform;
    public RectTransform CharacterPoint;
    private void Awake()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();
    }
    public void StartMatch()
    {
        _campaignController.StartMatch(1, _opponent1, _opponent2, _preMatchCutScene, _afterMatchCutScene);
    }
    public void EnableLevel()
    {
        levelButton.interactable = true;
    }
    public void EnableStartButton()
    {
        _startMatch.gameObject.SetActive(true);
    }
    public void DisableStartButton()
    {
        _startMatch.gameObject.SetActive(false);
    }
}
