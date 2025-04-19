using CommonDataTypes;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] Transform _matchStartPosition, _leftSidePosition, _rightSidePosition;
    [SerializeField] GameObject _ballPrefab;
    
    GameObject _ball;

    public BallScript SpawnBall()
    {
        GameObject go = Instantiate(_ballPrefab);
        go.transform.position = _matchStartPosition.position;
        _ball = go;
        return go.GetComponent<BallScript>();
    }

    public void ResetBallOnSide(FieldSideType sideType)
    {
        if (sideType == FieldSideType.Left)
            _ball.transform.position = _leftSidePosition.position;
        else if (sideType == FieldSideType.Right)
            _ball.transform.position = _rightSidePosition.position;
    }
}
