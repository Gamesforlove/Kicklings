using UnityEngine;

public class CampaignLevel : MonoBehaviour
{
    [SerializeField] private GameObject _opponent1, _opponent2;
    [SerializeField] private CampaignMapController _campaignController;

    public void StartMatch()
    {
        _campaignController.StartMatch(1, _opponent1, _opponent2);
    }
}
