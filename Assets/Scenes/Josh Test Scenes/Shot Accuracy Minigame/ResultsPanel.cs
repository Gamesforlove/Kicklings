using TMPro;
using UnityEngine;

namespace Gameplay.ShotAccuracy
{
    public class ResultsPanel : MonoBehaviour
    {
        [SerializeField] GameObject _panel; // the root to show/hide (can be this.gameObject)
        [SerializeField] TextMeshProUGUI _headlineText;
        [SerializeField] TextMeshProUGUI _scoreText;

        [Header("Colors")]
        [SerializeField] Color _wonColor = new Color(0.25f, 0.75f, 0.35f);
        [SerializeField] Color _lostColor = new Color(0.85f, 0.2f, 0.2f);

        void Awake()
        {
            Hide();
        }

        public void Show(bool won, int score, int target)
        {
            if (_headlineText != null)
            {
                _headlineText.text = won ? "CHALLENGE COMPLETE!" : "TIME'S UP!";
                _headlineText.color = won ? _wonColor : _lostColor;
            }

            if (_scoreText != null)
                _scoreText.text = $"{score} / {target}";

            if (_panel != null) _panel.SetActive(true);
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
        }
    }
}