using Unity.Services.Analytics;

public sealed class MatchEndedEvent : Event
{
    public MatchEndedEvent() : base("match_ended")
    {
    }

    public string Mode
    {
        set => SetParameter("mode", value);
    }

    public int Difficulty
    {
        set => SetParameter("difficulty", value);
    }

    public string EndReason
    {
        set => SetParameter("end_reason", value);
    }

    public float MatchDuration
    {
        set => SetParameter("match_duration", value);
    }

    public float PlayerSkillRating
    {
        set => SetParameter("player_skill_rating", value);
    }
}