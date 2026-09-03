using Scene_Management;
using UnityEngine;

public class ContinueCampaignButton : MonoBehaviour
{
    public void NextLevel()
    {
        MatchFlow.ContinueCampaign();
    }
}
