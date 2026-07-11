using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CountryFacts", menuName = "Scriptable Objects/CountryFacts")]
[Serializable]
public class CountryFacts : ScriptableObject
{
    [SerializeField] private List<Country> Countries = new List<Country>();

    [Serializable]
    private class Country
    {
        public string countryName;
        public List<string> facts;
    }

    public string GetRandomCountryFactByName(string _countryName) 
    {
        string fact = "No facts for this country(";

        Country Country = Countries.FirstOrDefault(x => x.countryName == _countryName);
        if (Country == null) 
            return fact;
        else
            return Country.facts[UnityEngine.Random.Range(0, Country.facts.Count)];
    }
}