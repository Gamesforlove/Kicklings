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
            int _goalsToEndMatch = 5;
            bool _isTournamentMatch = false;

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
                    GoalsToEndMatch = _goalsToEndMatch,
                    IsTournamentMatch = _isTournamentMatch
                };
            }
        }
    }
}