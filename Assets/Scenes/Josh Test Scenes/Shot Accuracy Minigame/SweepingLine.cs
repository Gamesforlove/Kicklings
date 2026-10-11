using UnityEngine;

namespace Gameplay.ShotAccuracy
{
    /// <summary>
    /// A single line that sweeps across the goal-net space until stopped.
    /// Written once and used for both axes:
    ///   TravelAxis.Horizontal = a VERTICAL line sliding left/right  -> gives the shot's X
    ///   TravelAxis.Vertical   = a HORIZONTAL line sliding up/down   -> gives the shot's Y
    ///
    /// World-space version: _line is expected to be a child of _bounds (a SpriteRenderer
    /// object), and all positions are _line.localPosition relative to _bounds. _boundsSize
    /// is the net's extent in world units (e.g. matching its sprite size), since a plain
    /// Transform has no rect to read a width/height from.
    /// </summary>
    public class SweepingLine : MonoBehaviour
    {
        public enum TravelAxis { Horizontal, Vertical }

        [SerializeField] Transform _line;
        [SerializeField] Transform _bounds;       //
        [SerializeField] Vector2 _boundsSize = new Vector2(4f, 3f); // world-unit width/height of the net
        [SerializeField] TravelAxis _axis = TravelAxis.Horizontal;

        [Header("Movement")]
        [SerializeField] float _speed = 3f;      
        [SerializeField] bool _pingPong = true;   
        [SerializeField] bool _randomStartPosition = true;

        float _half;
        float _position;
        int _direction = 1;
        bool _sweeping;

        public bool IsSweeping => _sweeping;

        public float LocalOffset => _position;

        public float NormalizedOffset => _half <= 0f ? 0f : _position / _half;

        public void BeginSweep()
        {
            CacheExtent();

            _position = _randomStartPosition
                ? Random.Range(-_half, _half)
                : -_half;

            _direction = 1;
            _sweeping = true;

            if (_line != null) _line.gameObject.SetActive(true);
            Apply();
        }

        /// <summary>Stops the line and returns where it landed, in _bounds local space.</summary>
        public float StopSweep()
        {
            _sweeping = false;
            return _position;
        }

        public void Hide()
        {
            _sweeping = false;
            if (_line != null) _line.gameObject.SetActive(false);
        }

        void Update()
        {
            if (!_sweeping || _line == null) return;

            _position += _speed * _direction * Time.deltaTime;

            if (_position > _half)
            {
                if (_pingPong) { _position = _half; _direction = -1; }
                else _position = -_half;
            }
            else if (_position < -_half)
            {
                if (_pingPong) { _position = -_half; _direction = 1; }
                else _position = _half;
            }

            Apply();
        }

        void CacheExtent()
        {
            _half = (_axis == TravelAxis.Horizontal ? _boundsSize.x : _boundsSize.y) * 0.5f;
        }

        void Apply()
        {
            if (_line == null) return;

            Vector3 p = _line.localPosition;
            if (_axis == TravelAxis.Horizontal) p.x = _position;
            else p.y = _position;
            _line.localPosition = p;
        }


        public void SetSpeed(float speed) => _speed = speed;
    }
}