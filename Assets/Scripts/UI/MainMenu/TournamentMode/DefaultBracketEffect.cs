namespace UI.MainMenu.TournamentMode
{
    public class DefaultBracketEffect : BracketVisualEffect
    {
        public override void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}