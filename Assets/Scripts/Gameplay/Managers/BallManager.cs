using CommonDataTypes;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] BallSpawner _ballSpawner;

    BallScript _ball;
    
    void Start()
    {
        _ball = _ballSpawner.SpawnBall();
    }

    public void ResetBallPositionOnSide(FieldSideType sideType)
    {
        _ballSpawner.ResetBallOnSide(sideType);
        _ball.Reset();
    }
}
