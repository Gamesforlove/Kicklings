using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonDataTypes
{
    [CreateAssetMenu(fileName = "TeamsData", menuName = "Scriptable Objects/TeamsData")]
    public class TeamsData : ScriptableObject
    {
        [Serializable]
        public class TeamData
        {
            public int Id;
            public string Name;
            public string FullName;
            public Sprite Icon;
            public Sprite ShirtSprite;
            public Color CountryColor;
        }
        
        public List<TeamData> Teams;

        public TeamData GetTeamById(int id) => Teams.Find(x => x.Id == id);

        public TeamData GetTeamByName(string name) => Teams.Find(x => x.Name == name);

        void OnValidate()
        {
            if (Teams == null) return;

            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i] == null)
                    continue;
                if (Teams[i].Icon != null)
                    Teams[i].Name = Teams[i].Icon.name;
                Teams[i].Id = i;
            }
        }
    }
}