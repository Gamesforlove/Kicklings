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
        
        public int LeftScore {get; private set;} 
        public int RightScore {get; private set;}

        public void ResetScore()
        {
            LeftScore = 0;
            RightScore = 0;
        
            _leftScoreText.text = LeftScore.ToString();
            _rightScoreText.text = RightScore.ToString();
        }
        
        public void ChangeScore(FieldSideType scoringSide)
        {
            switch (scoringSide)
            {
                case FieldSideType.Right:
                    RightScore++;
                    _rightScoreText.text = RightScore.ToString();
                    break;
                case FieldSideType.Left:
                    LeftScore++;
                    _leftScoreText.text = LeftScore.ToString();
                    break;
            }
        }

        public int GetScoreFromSide(FieldSideType side) => side ==  FieldSideType.Left ? LeftScore : RightScore;

        void Start()
        {
            _leftCountryImage.sprite = _countriesImages.GetCountrySprite(MatchFlow.MatchSettings.LeftCountryImageIndex);
            _rightCountryImage.sprite = _countriesImages.GetCountrySprite(MatchFlow.MatchSettings.RightCountryImageIndex);
        }

        
    }
}
