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
            public Sprite Icon;
        }
        
        public List<TeamData> Teams;

        public TeamData GetTeamById(int id) => Teams.Find(x => x.Id == id);

        public TeamData GetTeamByName(string name) => Teams.Find(x => x.Name == name);

        void OnValidate()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                Teams[i].Name = Teams[i].Icon.name;
                Teams[i].Id = i;
            }
        }
    }
}