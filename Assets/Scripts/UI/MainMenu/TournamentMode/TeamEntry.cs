using System;
using CommonDataTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.TournamentMode
{
    public class TeamEntry : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] Image _flagImage;

        public void ChangeVisualElements(TeamsData.TeamData data)
        {
            _name.text = data.Name;
            _flagImage.sprite = data.Icon;
            _flagImage.enabled = true;
        }
    }
}