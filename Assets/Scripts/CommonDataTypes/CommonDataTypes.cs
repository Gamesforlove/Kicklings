using UnityEngine.UI;

namespace CommonDataTypes
{
    public enum FieldSideType
    {
        Left, Right
    }
    
    public enum SceneName
    {
        MainMenu, Gameplay
    }

    public enum CharacterCustomizationPlayerPrefsKeys
    {
        LeftShirt, RightShirt, LeftShoes, RightShoes
    }

    public class MatchSettings
    {
        public int MaxNumberOfEntities { get; private set; } = 4;
        public int NumberOfPlayers { get; set; }
        public int LeftSideShirtIndex { get; set; }
        public int RightSideShirtIndex { get; set; }
        public int LeftSideShoesIndex { get; set; }
        public int RightSideShoesIndex { get; set; }
        public Image LeftCountryImage { get; set; }
        public Image RightCountryImage { get; set; }
        public int GoalsToEndMatch { get; set; }

        public void Dispose()
        {
            NumberOfPlayers = 0;
            LeftSideShirtIndex = 0;
            RightSideShirtIndex = 0;
            LeftSideShoesIndex = 0;
            RightSideShoesIndex = 0;
            LeftCountryImage = null;
            RightCountryImage = null;
            GoalsToEndMatch = 0;
        }
    }
}