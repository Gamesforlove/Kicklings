using UnityEngine;

namespace Gameplay.ShotAccuracy
{
    /// <summary>
    /// The highlighted portion of the net the player is aiming at.
    /// Scoring is graded by distance from center, not binary hit/miss.
    ///
    /// World-space version: _target is expected to be a child of _bounds, positioned via
    /// localPosition. _boundsSize is the net's extent in world units.
    /// </summary>
    public class ShotTarget : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Transform _bounds;
        [SerializeField] private Vector2 _boundsSize = new Vector2(4f, 3f);

        [Header("Placement")]
        [SerializeField] bool _randomizePosition = true;
        [SerializeField, Range(0f, 0.9f)] float _edgeMargin = 0.2f; 
        [Header("Scoring")]
        [Tooltip("Within this distance (world units) of center, the shot scores full points.")]
        [SerializeField] float _perfectRadius = 0.3f;
        [Tooltip("Beyond this distance, the shot scores nothing. Between the two, points fall off linearly.")]
        [SerializeField] float _scoringRadius = 1.5f;


        public Vector2 Center => _target != null ? (Vector2)_target.localPosition : Vector2.zero;

        public void Randomize()
        {
            if (!_randomizePosition || _target == null) return;

            float halfW = _boundsSize.x * 0.5f * (1f - _edgeMargin);
            float halfH = _boundsSize.y * 0.5f * (1f - _edgeMargin);

            Vector3 p = _target.localPosition;
            p.x = Random.Range(-halfW, halfW);
            p.y = Random.Range(-halfH, halfH);
            _target.localPosition = p;
        }

        public void SetVisible(bool visible)
        {
            if (_target != null) _target.gameObject.SetActive(visible);
        }


        public float Evaluate(Vector2 shot)
        {
            float distance = Vector2.Distance(shot, Center);

            if (distance <= _perfectRadius) return 1f;
            if (distance >= _scoringRadius) return 0f;

            return 1f - Mathf.InverseLerp(_perfectRadius, _scoringRadius, distance);
        }

        public float DistanceFromCenter(Vector2 shot) => Vector2.Distance(shot, Center);


        public void SetRadii(float perfectRadius, float scoringRadius)
        {
            _perfectRadius = perfectRadius;
            _scoringRadius = scoringRadius;
        }
    }
}