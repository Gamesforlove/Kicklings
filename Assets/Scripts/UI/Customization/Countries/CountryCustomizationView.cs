using CommonDataTypes;
using UnityEngine;

namespace UI.Customization.Countries
{
    public abstract class CountryCustomizationView : MonoBehaviour
    {
        public abstract void ChangeViewElements(TeamsData.TeamData teamData);
    }
}