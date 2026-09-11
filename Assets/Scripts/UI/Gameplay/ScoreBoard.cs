using CommonDataTypes;
using Scene_Management;
using TMPro;
using UI.Customization.Countries;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] CountriesImages _countriesImages;
        [SerializeField] TextMeshProUGUI _leftScoreText,  _rightScoreText;
        [SerializeField] Image _leftCountryImage, _rightCountryImage;

        public void ResetScore()
        {
            ChangeScore(0, 0);
        }
        
        public void ChangeScore(int leftScore, int rightScore)
        {
            _leftScoreText.text = leftScore.ToString();
            _rightScoreText.text = rightScore.ToString();
        }

        void Start()
        {
            _leftCountryImage.sprite = _countriesImages.GetCountrySprite(MatchFlow.Match.Settings.LeftCountryImageIndex);
            _rightCountryImage.sprite = _countriesImages.GetCountrySprite(MatchFlow.Match.Settings.RightCountryImageIndex);
        }
    }
}
