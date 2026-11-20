using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace Gameplay.Goal
{
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField] FieldSideData _scoringSideData;
        [SerializeField] FieldSideData _scoredSideData;
        [Tooltip("Extra distance the ball must travel into the human team's net, in world units.")]
        [SerializeField, Min(0f)] float _playerNetInset;
        [SerializeField, Range(0.5f, 1f)] float _playerNetWidthScale = 1f;

        BoxCollider2D _scoringBox;
        Vector2 _originalOffset;
        Vector2 _originalSize;

        internal void ConfigurePlayerNet(bool playerDefendsLeft)
        {
            if (_playerNetInset <= 0f || _scoredSideData == null || _scoredSideData.SideType != FieldSideType.Left)
                return;

            if (_scoringBox == null)
            {
                if (!TryGetComponent(out _scoringBox)) return;
                _originalOffset = _scoringBox.offset;
                _originalSize = _scoringBox.size;
            }

            Vector2 size = _originalSize;
            Vector2 offset = _originalOffset;
            if (playerDefendsLeft)
            {
                float insetX = transform.InverseTransformVector(Vector3.left * _playerNetInset).x;
                size.x *= _playerNetWidthScale;
                // Measure depth at the field-facing edge, accounting for the narrower box.
                offset.x += insetX - Mathf.Sign(insetX) * (_originalSize.x - size.x) * 0.5f;
            }

            _scoringBox.size = size;
            _scoringBox.offset = offset;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.GetComponent<BallScript>()) return;
        
            EventBus<GoalEvent>.Raise(new GoalEvent(_scoringSideData, _scoredSideData));
        }
    }
}
