using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    /// <summary>
    /// Self-contained win/lose panel for challenge minigames. Plain SetActive show/hide,
    /// same approach UiManager already uses for the tournament/campaign end screens —
    /// no UIView, CanvasGroup, or UIViewsManager involved. Starts hidden.
    /// </summary>
    public class ChallengeResultPanel : MonoBehaviour
    {
        [SerializeField] GameObject _panel; // root to show/hide (can be this.gameObject)
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