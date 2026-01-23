using CommonDataTypes;

namespace EventBusSystem
{
    public readonly struct OnCountryChanged : IEvent
    {
        public readonly TeamsData.TeamData TeamData;

        public OnCountryChanged(TeamsData.TeamData teamData)
        {
            TeamData = teamData;
        }
    }
}