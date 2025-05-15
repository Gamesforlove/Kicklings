using CommonDataTypes;

namespace Scene_Management
{
    public class Match
    {
        public MatchSettings Settings { get; private set; }
        public bool IsPlayerWinner { get; set; }
        
        public Match(MatchSettings settings)
        {
            Settings = settings;
        }

        public void Dispose()
        {
            Settings.Dispose();
            IsPlayerWinner = false;
        }
    }
}