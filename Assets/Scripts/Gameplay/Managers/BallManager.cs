using CommonDataTypes;
using Gameplay.Spawners;
using UnityEngine;

namespace Gameplay.Managers
{
    public class BallManager : MonoBehaviour
    {
        [SerializeField] BallSpawner _ballSpawner;

        public static BallManager Instance { get; private set; }
        void Awake() => Instance = this;

        public BallScript Ball { get; private set; }
    
        public void SpawnBall() => Ball = _ballSpawner?.SpawnBall();
    
        public void ResetBall()
        {
            _ballSpawner?.ResetBall();
            Ball?.Reset();
        }

        public void ResetBall(FieldSideType sideType)
        {
            _ballSpawner?.ResetBallOnSide(sideType);
            Ball?.Reset();
        }
    }
}
