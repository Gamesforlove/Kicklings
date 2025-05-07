using CommonDataTypes;
using EventBusSystem;
using UnityEngine.UI;

namespace Scene_Management
{
    public static class MatchFlow
    {
        public static MatchSettings MatchSettings { get; private set; } = new();
        public static void CreateMatch()
        {
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.Gameplay));
        }

        public static void DisposeMatch() => MatchSettings.Dispose();

        public static void SetNumberOfPlayers(int numberOfPlayers) =>
            MatchSettings.NumberOfPlayers = numberOfPlayers;

        public static void SetLeftSideShirtIndex(int leftSideShirtIndex) =>
            MatchSettings.LeftSideShirtIndex = leftSideShirtIndex;

        public static void SetRightSideShirtIndex(int rightSideShirtIndex) =>
            MatchSettings.RightSideShirtIndex = rightSideShirtIndex;

        public static void SetLeftSideShoesIndex(int leftSideShoesIndex) =>
            MatchSettings.LeftSideShoesIndex = leftSideShoesIndex;

        public static void SetRightSideShoesIndex(int rightSideShoesIndex) =>
            MatchSettings.RightSideShoesIndex = rightSideShoesIndex;

        public static void SetLeftCountryImage(int leftCountryIndex) =>
            MatchSettings.LeftCountryImageIndex = leftCountryIndex;

        public static void SetRightCountryImage(int rightCountryIndex) =>
            MatchSettings.RightCountryImageIndex = rightCountryIndex;
        
        public static void SetGoalsToEndMatch(int goalsToEndMatch) =>
            MatchSettings.GoalsToEndMatch = goalsToEndMatch;
    }
}