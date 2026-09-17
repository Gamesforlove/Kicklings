using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CommonDataTypes
{
    [System.Serializable]
    public class MatchSettings
    {
        [field: SerializeField] public int MaxNumberOfEntities { get; private set; } = 4;
        [field: SerializeField] public int NumberOfPlayers { get; set; }
        [field: SerializeField] public int LeftSideShirtIndex { get; set; }
        [field: SerializeField] public int RightSideShirtIndex { get; set; }
        [field: SerializeField] public int LeftSideShoesIndex { get; set; }
        [field: SerializeField] public int RightSideShoesIndex { get; set; }
        [field: SerializeField] public int LeftCountryImageIndex { get; set; }
        [field: SerializeField] public int RightCountryImageIndex { get; set; }
        [field: SerializeField] public int GoalsToEndMatch { get; set; } = 5;
        [field: SerializeField] public bool IsTournamentMatch {get; private set;}
        [field: SerializeField] public bool SplitControls { get; private set; }
        [field: SerializeField] public float LeftSkinToneValue { get; private set; }
        [field: SerializeField] public float RightSkinToneValue { get; private set; }
        
        //Campaign
        [field: SerializeField] public bool IsCampaignMatch {get; private set;}
        [field: SerializeField] public CampaignLevelData LevelData {get; private set;}

        public MatchSettings() { }

        public void Dispose()
        {
            NumberOfPlayers = 0;
            LeftSideShirtIndex = 0;
            RightSideShirtIndex = 0;
            LeftSideShoesIndex = 0;
            RightSideShoesIndex = 0;
            LeftCountryImageIndex = 0;
            RightCountryImageIndex = 0;
            GoalsToEndMatch = 0;
            IsTournamentMatch = false;

            IsCampaignMatch = false;
            LevelData = null;
/*            PreMatchCutScene = SceneName.None;
            AfterMatchCutScene = SceneName.None;
            AfterMatchDefeatCutScene = SceneName.None;
            if (SpecificPlayers != null)
            {
                System.Array.Clear(SpecificPlayers, 0, SpecificPlayers.Length);
            }*/

            SplitControls = false;
        }

        public class Builder
        {
            readonly int _maxNumberOfEntities = 4;
            int _numberOfPlayers = 1;
            int _leftSideShirtIndex;
            int _rightSideShirtIndex;
            int _leftSideShoesIndex;
            int _rightSideShoesIndex;
            int _leftCountryImageIndex;
            int _rightCountryImageIndex;
            float _leftSkinToneValue;
            float _rightSkinToneValue;
            int _goalsToEndMatch = 5;
            bool _isTournamentMatch = false;
            bool _isCampaignMatch = false;
            CampaignLevelData _levelData;
            bool _splitControls = false;

            public Builder WithNumberOfPlayers(int numberOfPlayers)
            {
                _numberOfPlayers = numberOfPlayers;
                return this;
            }
            
            public Builder WithLeftShirtIndex(int index)
            {
                _leftSideShirtIndex = index;
                return this;
            }
            
            public Builder WithLeftShoesIndex(int index)
            {
                _leftSideShoesIndex = index;
                return this;
            }
            
            public Builder WithRightShirtIndex(int index)
            {
                _rightSideShirtIndex = index;
                return this;
            }
            
            public Builder WithRightShoesIndex(int index)
            {
                _rightSideShoesIndex = index;
                return this;
            }

            public Builder WithLeftCountryImageIndex(int index)
            {
                _leftCountryImageIndex = index;
                return this;
            }

            public Builder WithRightCountryImageIndex(int index)
            {
                _rightCountryImageIndex = index;
                return this;
            }
            public Builder WithLeftSkinToneValue(float value)
            {
                _leftSkinToneValue = value;
                return this;
            }
            public Builder WithRightSkinToneValue(float value)
            {
                _rightSkinToneValue = value;
                return this;
            }

            public Builder WithGoalsToEndMatch(int goalsToEndMatch)
            {
                _goalsToEndMatch = goalsToEndMatch;
                return this;
            }

            public Builder WithIsTournamentMatch(bool isTournamentMatch)
            {
                _isTournamentMatch = isTournamentMatch;
                return this;
            }
            
            public Builder WithIsCampaignMatch(bool isCampaignMatch)
            {
                _isCampaignMatch = isCampaignMatch;
                return this;
            }
            public Builder WithLevelData(CampaignLevelData levelData)
            {
                _levelData = levelData;
                return this;
            }
            public Builder WithSplitControls(bool splitControls)
            {
                _splitControls = splitControls;
                return this;
            }

            public MatchSettings Build()
            {
                return new MatchSettings
                {
                    MaxNumberOfEntities = _maxNumberOfEntities,
                    NumberOfPlayers = _numberOfPlayers,
                    LeftSideShirtIndex = _leftSideShirtIndex,
                    RightSideShirtIndex = _rightSideShirtIndex,
                    LeftSideShoesIndex = _leftSideShoesIndex,
                    RightSideShoesIndex = _rightSideShoesIndex,
                    LeftCountryImageIndex = _leftCountryImageIndex,
                    RightCountryImageIndex = _rightCountryImageIndex,
                    LeftSkinToneValue = _leftSkinToneValue,
                    RightSkinToneValue = _rightSkinToneValue,
                    GoalsToEndMatch = _goalsToEndMatch,
                    IsTournamentMatch = _isTournamentMatch,
                    IsCampaignMatch = _isCampaignMatch,
                    LevelData = _levelData,
                    SplitControls = _splitControls
                };
            }
        }
    }
}