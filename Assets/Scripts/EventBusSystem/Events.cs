using CommonDataTypes;
using UnityEngine.UI;

namespace EventBusSystem
{
    public interface IEvent { }

    #region Gameplay

    public struct GoalEvent : IEvent
    {
        public readonly FieldSideData ScoringSideData;
        public readonly FieldSideData ScoredSideData;

        public GoalEvent(FieldSideData scoringSideData, FieldSideData scoredSideData)
        {
            ScoringSideData = scoringSideData;
            ScoredSideData = scoredSideData;
        }
    }
    
    public struct OutEvent : IEvent
    {
        public readonly FieldSideData FieldSideData;

        public OutEvent(FieldSideData data)
        {
            FieldSideData = data;
        }
    }
    
    public struct PlayerActionPerformed : IEvent {}
    
    public struct PlayerActionCanceled : IEvent {}
    
    public struct PlayerJumped : IEvent {}

    #endregion

    public readonly struct OnLoadScene : IEvent
    {
        public readonly SceneName EnumValue;
        public string Name => EnumValue.ToString();

        public OnLoadScene(SceneName enumValue)
        {
            EnumValue = enumValue;
        }
    }

    public readonly struct OnSceneLoaded : IEvent
    {
        public readonly SceneName EnumValue;
        public string SceneName => EnumValue.ToString();

        public OnSceneLoaded(SceneName enumValue)
        {
            EnumValue = enumValue;
        }
    }

    public readonly struct OnCountryChanged : IEvent
    {
        public readonly int CountryID;
        public readonly Image CountryImage;

        public OnCountryChanged(int countryID, Image countryImage)
        {
            CountryID = countryID;
            CountryImage = countryImage;
        }
    }
}
