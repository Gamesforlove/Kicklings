using UnityEngine;

public class CampaignLevel : MonoBehaviour
{
    [SerializeField] private GameObject _opponent1, _opponent2;
    [SerializeField] private CampaignMapController _campaignController;
    public RectTransform RectTransform;
    public RectTransform CharacterPoint;
    private void Awake()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();
    }
    public void StartMatch()
    {
        _campaignController.StartMatch(1, _opponent1, _opponent2);
    }
}
