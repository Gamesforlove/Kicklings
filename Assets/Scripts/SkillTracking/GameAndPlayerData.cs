using UnityEngine;

public class GameAndPlayerData : MonoBehaviour
{
    public int numGamesPlayed { get; set; }
    public int numGamesWon { get; set; }
    public int numGamesLost { get; set; }
    public int numTournamentMatchesPlayed { get; set; }
    public int numTournamentMatchesWon { get; set; }
    public int numTournamentMatchesLost { get; set; }
    public int numSessionsPlayed { get; set; }
    public float totalPlaytime { get; set; }

    public int numGamesPlayedToday { get; set; }
    public int numGamesWonToday { get; set; }
    public int numGamesLostToday { get; set; }
    public int numTournamentMatchesPlayedToday { get; set; }
    public int numTournamentMatchesWonToday { get; set; }
    public int numTournamentMatchesLostToday { get; set; }
    public float totalPlaytimeToday() => Time.time;

    // Persistent ELO rating. Nudges toward each game result with the eloInc. Starts at 0.35 and saved across sessions.
    private float eloT = 0.35f;
    private const float eloInc = 0.1f;
    public void UpdateElo(bool won)
    {
        float outcome = won ? 1f : 0f;
        eloT += eloInc * (outcome - eloT);
    }

    // Skill rating: 0 = weakest, 1 = strongest. Starts at 0.35 with no data.
    // Blends the persistent ELO signal with today's Bayesian win rate. Today's weight depends on how many games you played today.
    public float T
    {
        get
        {
            int playedToday = numGamesPlayedToday + numTournamentMatchesPlayedToday;
            int wonToday = numGamesWonToday    + numTournamentMatchesWonToday;
            float todayRate = (wonToday + 1.75f) / (playedToday + 5f);
            float todayWeight = playedToday / (playedToday + 5f);
            return Mathf.Lerp(eloT, todayRate, todayWeight);
        }
    }

    public static GameAndPlayerData Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        numGamesPlayedToday = 0;
        numGamesLostToday = 0;
        numGamesWonToday = 0;
        numTournamentMatchesPlayedToday = 0;
        numTournamentMatchesLostToday = 0;
        numTournamentMatchesWonToday = 0;

        // Cross-play-session data is saved and loaded to player prefs
        eloT = PlayerPrefs.GetFloat("eloT", 0.35f);
        numGamesPlayed = PlayerPrefs.GetInt("numGamesPlayed", 0);
        numGamesLost = PlayerPrefs.GetInt("numGamesLost", 0);
        numGamesWon = PlayerPrefs.GetInt("numGamesWon", 0);
        numTournamentMatchesPlayed = PlayerPrefs.GetInt("numTournamentMatchesPlayed", 0);
        numTournamentMatchesLost = PlayerPrefs.GetInt("numTournamentMatchesLost", 0);
        numTournamentMatchesWon = PlayerPrefs.GetInt("numTournamentMatchesWon", 0);
    }

    private void OnApplicationQuit()
    {
        // Save cross-play-session data to player prefs
        PlayerPrefs.SetFloat("eloT", eloT);
        PlayerPrefs.SetInt("numGamesPlayed", numGamesPlayed);
        PlayerPrefs.SetInt("numGamesLost", numGamesLost);
        PlayerPrefs.SetInt("numGamesWon", numGamesWon);
        PlayerPrefs.SetInt("numTournamentMatchesPlayed", numTournamentMatchesPlayed);
        PlayerPrefs.SetInt("numTournamentMatchesLost", numTournamentMatchesLost);
        PlayerPrefs.SetInt("numTournamentMatchesWon", numTournamentMatchesWon);
    }
}
