using CommonDataTypes;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] BallSpawner _ballSpawner;

    BallScript _ball;
    
    public void SpawnBall() => _ball = _ballSpawner.SpawnBall();
    
    public void ResetBall()
    {
        _ballSpawner.ResetBall();
        _ball.Reset();
    }

    public void ResetBall(FieldSideType sideType)
    {
        _ballSpawner.ResetBallOnSide(sideType);
        _ball.Reset();
    }
}
