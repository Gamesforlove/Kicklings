using CommonDataTypes;
using UnityEngine;

namespace Gameplay.Spawners
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] Transform _leftSidePosition, _rightSidePosition, _centerPosition;
        [SerializeField] GameObject _ballPrefab;
    
        GameObject _ball;

        public BallScript SpawnBall()
        {
            GameObject go = Instantiate(_ballPrefab);
            go.transform.position = _centerPosition.position;
            _ball = go;
            return go.GetComponent<BallScript>();
        }

        public void ResetBall()
        {
            _ball.transform.position = _centerPosition.position;
            _ball.transform.rotation = Quaternion.identity;
        }

        public void ResetBallOnSide(FieldSideType sideType)
        {
            if (sideType == FieldSideType.Left)
            {
                _ball.transform.position = _leftSidePosition.position;
                _ball.transform.rotation = Quaternion.identity;
            }
            else if (sideType == FieldSideType.Right)
            {
                _ball.transform.position = _rightSidePosition.position;
                _ball.transform.rotation = Quaternion.identity;
            }
        }
    }
}
