using CommonDataTypes;

namespace Scene_Management
{
    public class Match
    {
        public MatchSettings Settings { get; }
        public bool IsPlayerWinner { get; set; }
        public bool IsPlayAgain { get; set; }
        
        public Match(MatchSettings settings)
        {
            Settings = settings;
        }

        public void Dispose()
        {
            Settings.Dispose();
            IsPlayerWinner = false;
            IsPlayAgain = false;
        }
    }
}