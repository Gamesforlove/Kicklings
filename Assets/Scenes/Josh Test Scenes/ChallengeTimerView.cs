using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{

    public class ChallengeTimerView : MonoBehaviour
    {
        [Header("Fill Bar")]
        [SerializeField] Image _fillImage; 

        [Header("Urgency Color")]
        [SerializeField] bool _useUrgencyColor = true;
        [SerializeField] Color _normalColor = new Color(0.25f, 0.75f, 0.35f);
        [SerializeField] Color _urgentColor = new Color(0.85f, 0.2f, 0.2f);
        [SerializeField, Range(0f, 1f)] float _urgentThreshold = 0.25f; 

        [Header("Pulse (optional)")]
        [SerializeField] bool _pulseWhenUrgent = true;
        [SerializeField] float _pulseSpeed = 6f;
        [SerializeField] float _pulseScale = 0.06f;

        float _maxTime = 60f;
        bool _isUrgent;

        void Awake()
        {
            if (_fillImage != null)
            {
                _fillImage.type = Image.Type.Filled;
                _fillImage.fillAmount = 1f;
            }
        }

        public void SetMaxTime(float maxTime)
        {
            _maxTime = Mathf.Max(maxTime, 0.0001f);
        }

        public void SetTime(float timeRemaining)
        {
            if (_fillImage == null) return;

            float t = Mathf.Clamp01(timeRemaining / _maxTime);
            _fillImage.fillAmount = t;

            bool nowUrgent = _useUrgencyColor && t <= _urgentThreshold;
            if (_useUrgencyColor)
            {
                _fillImage.color = Color.Lerp(_urgentColor, _normalColor, Mathf.InverseLerp(0f, _urgentThreshold, t));
            }

            if (nowUrgent && !_isUrgent)
            {
                _isUrgent = true;
            }
            else if (!nowUrgent)
            {
                _isUrgent = false;
                transform.localScale = Vector3.one;
            }
        }

        void Update()
        {
            if (!_pulseWhenUrgent || !_isUrgent || _fillImage == null) return;

            float pulse = 1f + Mathf.Sin(Time.unscaledTime * _pulseSpeed) * _pulseScale;
            transform.localScale = new Vector3(pulse, pulse, 1f);
        }

        public void ResetView()
        {
            if (_fillImage != null) _fillImage.fillAmount = 1f;
            transform.localScale = Vector3.one;
            _isUrgent = false;
        }
    }
}