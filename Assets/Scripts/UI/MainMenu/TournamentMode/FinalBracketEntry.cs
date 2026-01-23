namespace UI.MainMenu.TournamentMode
{
    public class FinalBracketEntry : BracketEntry
    {
        public override void Initialize(Bracket bracketData)
        {
            for (int i = 0; i < bracketData.Participants.Length; i++)
            {
                TeamEntries[i].ChangeVisualElements(bracketData.Participants[i].TeamData);
            }
            
            if (bracketData.IsPlayerBracket()) VisualEffect.SetActive(true);
        }
    }
}