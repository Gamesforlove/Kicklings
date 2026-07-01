using CommonDataTypes;

namespace EventBusSystem
{
    public readonly struct OnCountryChanged : IEvent
    {
        public readonly TeamsData.TeamData TeamData;
        public readonly FieldSideType LastSelectedFieldSideType;

        public OnCountryChanged(TeamsData.TeamData teamData, FieldSideType lastSelectedFieldSideType)
        {
            TeamData = teamData;
            LastSelectedFieldSideType = lastSelectedFieldSideType;
        }
    }
}