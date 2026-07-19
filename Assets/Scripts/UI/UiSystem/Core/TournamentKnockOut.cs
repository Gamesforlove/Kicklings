using Gameplay.Managers;
using Scene_Management;
using UnityEngine;

public class TournamentKnockOut : MonoBehaviour
{
    public void SetPlayAgain()
    {
        MatchFlow.Match.IsPlayAgain = true;
    }
}
