using CommonDataTypes;
using EventBusSystem;
using System.Collections;
using UI.Customization.Countries;
using UnityEngine;

public class TournamentModeCountryRandomizer : MonoBehaviour
{
    [SerializeField] private CountryCustomizationController _country;
    [SerializeField] private TeamsData _teamsData;
    private Coroutine _routine;

    private void OnEnable()
    {
        _routine = StartCoroutine(RandomizeCountries());
    }
    private void OnDisable()
    {
        if (_routine != null)
            StopCoroutine(_routine);
    }
    private IEnumerator RandomizeCountries()
    {
        int randomIndex = Random.Range(0, _teamsData.Teams.Count);

        yield return new WaitForSeconds(.05f);
        _country.Select();
        EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamsData.Teams[randomIndex], FieldSideType.None));
    }
}
