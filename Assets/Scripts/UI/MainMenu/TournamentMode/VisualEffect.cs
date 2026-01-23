using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.TournamentMode
{
    public abstract class BracketVisualEffect : MonoBehaviour
    {
        [SerializeField] protected Image Glow, Background;
        public abstract void SetActive(bool active);
    }
}