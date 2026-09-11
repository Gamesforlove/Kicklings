using TMPro;
using UnityEngine;

namespace UI.Gameplay
{

    public class ChallengeScoreBoard : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] string _format = "{0} / {1}"; // e.g. "3 / 10"

        public void ResetScore(int target)
        {
            ChangeScore(0, target);
        }

        public void ChangeScore(int score, int target)
        {
            _scoreText.text = string.Format(_format, score, target);
        }
    }
}