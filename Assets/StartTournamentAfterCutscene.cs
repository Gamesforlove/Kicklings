using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using System.Collections;
using UI.MainMenu.TournamentMode;
using UnityEngine;

public class StartTournamentAfterCutscene : MonoBehaviour
{
    public static StartTournamentAfterCutscene Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerCutsceneThenTournament(string country, TournamentLayoutComponent _tournamentLayoutComponent, TournamentConfiguration _tournamentConfiguration, Tournament Tournament, int leftCountryImageIndex, int rightCountryImageIndex)
    {
        StartCoroutine(playCutsceneThenTournamentRoutine(country, _tournamentLayoutComponent, _tournamentConfiguration, Tournament, leftCountryImageIndex, rightCountryImageIndex));
    }

    IEnumerator playCutsceneThenTournamentRoutine(string country, TournamentLayoutComponent _tournamentLayoutComponent, TournamentConfiguration _tournamentConfiguration, Tournament Tournament, int leftCountryImageIndex, int rightCountryImageIndex)
    {
        // play cutscene
        TournamentCutsceneContext.Country = country;
        TournamentCutsceneContext.NumPoints = goalsToEndMatch;
        TournamentCutsceneContext.NumTeams = _tournamentLayoutComponent.GetLayoutModeTeams();
        EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.TournamentIntroCutscene));
        yield return null;
        yield return new WaitUntil(() => TournamentCutsceneState.Instance != null && TournamentCutsceneState.Instance.CutsceneStarted);
        yield return new WaitUntil(() => TournamentCutsceneState.Instance == null || TournamentCutsceneState.Instance.CutsceneFinished);

        // play match
        MatchSettings matchSettings = new MatchSettings.Builder()
            .WithLeftShirtIndex(_tournamentConfiguration.PlayerShirtIndex)
            .WithLeftShoesIndex(_tournamentConfiguration.PlayerShoesIndex)
            .WithLeftCountryImageIndex(leftCountryImageIndex)
            .WithRightCountryImageIndex(rightCountryImageIndex)
            .WithRightSkinToneValue(RandomSkinTone())
            .WithLeftSkinToneValue(_tournamentConfiguration.PlayerSkinTone)
            .WithIsTournamentMatch(isTournamentMatch)
            .WithGoalsToEndMatch(goalsToEndMatch)
            .Build();
        MatchFlow.CreateTournamentMatch(matchSettings, Tournament);

        Destroy(gameObject, 2f);
    }

    public static float RandomSkinTone() => Random.Range(0, 1.0f);
    public const bool isTournamentMatch = true;
    public const int goalsToEndMatch = 5;
}
