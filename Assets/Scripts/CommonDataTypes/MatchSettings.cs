using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CommonDataTypes
{
    public class MatchSettings
    {
        public int MaxNumberOfEntities { get; private set; } = 4;
        public int NumberOfPlayers { get; set; }
        public int LeftSideShirtIndex { get; set; }
        public int RightSideShirtIndex { get; set; }
        public int LeftSideShoesIndex { get; set; }
        public int RightSideShoesIndex { get; set; }
        public int LeftCountryImageIndex { get; set; }
        public int RightCountryImageIndex { get; set; }
        public float LeftSkinToneValue { get; set; }
        public float RightSkinToneValue { get; set; }
        public int GoalsToEndMatch { get; set; } = 5;
        public bool IsTournamentMatch {get; private set;}

        //Campaign
        public bool IsCampaignMatch {get; private set;}
        public GameObject[] SpecificPlayers { get; private set;} = new GameObject[4];
        
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
            if (SpecificPlayers != null)
            {
                System.Array.Clear(SpecificPlayers, 0, SpecificPlayers.Length);
            }
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
            GameObject[] _specificPlayers = new GameObject[4];

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
            public Builder WithIsCampaignMatch(bool isTournamentMatch)
            {
                _isCampaignMatch = isTournamentMatch;
                return this;
            }
            public Builder WithSpecificPlayers(GameObject player1, GameObject player2, GameObject opponent1, GameObject opponent2)
            {
                _specificPlayers[0] = player1;
                _specificPlayers[1] = player2;
                _specificPlayers[2] = opponent1;
                _specificPlayers[3] = opponent2;
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
                    SpecificPlayers = _specificPlayers
                };
            }
        }
    }
}