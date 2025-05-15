using System;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentLayoutComponent : MonoBehaviour
    {
       public TournamentLayoutMode LayoutMode;

       public int GetLayoutModeTeams() => LayoutMode switch
       {
           TournamentLayoutMode.Four => 4,
           TournamentLayoutMode.Eight => 8,
           TournamentLayoutMode.Sixteen => 16,
       };
    }
}