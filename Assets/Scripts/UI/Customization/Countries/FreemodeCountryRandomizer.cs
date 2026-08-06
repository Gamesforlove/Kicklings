using CommonDataTypes;
using System.Collections;
using UI.Customization.Countries;
using UnityEngine;
using EventBusSystem;

public class FreemodeCountryRandomizer : MonoBehaviour
{
    [SerializeField] private CountryCustomizationController _leftCountry;
    [SerializeField] private CountryCustomizationController _RightCountry;
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
        int randomIndex1 = Random.Range(0, _teamsData.Teams.Count);
        int randomIndex2 = Random.Range(0, _teamsData.Teams.Count);
        if (randomIndex2 == randomIndex1)
            randomIndex2++;

        yield return new WaitForSeconds(.05f);
        _leftCountry.Select();
        EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamsData.Teams[randomIndex1], FieldSideType.Left));
        yield return new WaitForSeconds(.02f);
        _RightCountry.Select();
        EventBus<OnCountryChanged>.Raise(new OnCountryChanged(_teamsData.Teams[randomIndex2], FieldSideType.Right));
    }
}
