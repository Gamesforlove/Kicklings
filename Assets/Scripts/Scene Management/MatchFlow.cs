using CommonDataTypes;
using EventBusSystem;

namespace Scene_Management
{
    public static class MatchFlow
    {
        public static GameModeData SelectedGameModeData { get; private set; }
        
        public static void CreateMatch(GameModeData gameModeData)
        {
            SelectedGameModeData = gameModeData;
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.Gameplay));
        }
    }
}