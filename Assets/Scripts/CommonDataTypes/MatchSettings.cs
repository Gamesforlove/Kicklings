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
        public int GoalsToEndMatch { get; set; }
        
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
            int _goalsToEndMatch = 1;

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
                    GoalsToEndMatch = _goalsToEndMatch
                };
            }
        }
    }
}