using System;
using System.Collections;
using Gameplay.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.CharacterComponents.Cpu
{
    public class Cpu : Entity
    {
        Transform _proximity1;
        Transform _proximity2;
        
        BallManager _ballManager;
        BallScript _ball;

        CountdownTimer _actionTimer;

        void Awake()
        {
            _actionTimer = new CountdownTimer(Random.Range(3f, 10f));
            _actionTimer.OnTimerStop += DoAction;
        }

        public override void SetUp(EntityData data)
        {
            base.SetUp(data);
            CacheProximitySensors();
            PlayerInput.enabled = false;
            PlayerIndicator.gameObject.SetActive(false);
        }

        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            _actionTimer.Reset();
        }

        void CacheProximitySensors()
        {
            Transform intermediateChild = gameObject.transform.Find("Sensors");
            _proximity1 = intermediateChild.Find("Proximity1").transform;
            _proximity2 = intermediateChild.Find("Proximity2").transform;
        }

        void Start()
        {
            _ballManager = FindFirstObjectByType<BallManager>();
            _ball = _ballManager.Ball;
            _actionTimer.Start();
        }

        void Update()
        {
            CpuPlayer();
            _actionTimer.Tick(Time.deltaTime);
        }
    
        void CpuPlayer()
        {
            bool ballisnear = false;
            
            float speedMux = _ball.Rigidbody.linearVelocity.magnitude / 8;

            if ((_proximity1.position - _ball.transform.position).magnitude < 0.5f + 2 * speedMux)
            {
                ballisnear = true;
            }
            if ((_proximity2.position - _ball.transform.position).magnitude < 0.5f + 1 * speedMux)
            {
                ballisnear = true;
            }

            if (ballisnear)
            {
                DoAction();
            }
        }
        
        void DoAction()
        {
            float tm = Random.Range(0.1f, 0.3f);
            StartCoroutine(RandomReflex(tm));
            _actionTimer.Reset(Random.Range(3f, 10f));
            _actionTimer.Start();
        }
        
        IEnumerator RandomReflex(float time)
        {
            yield return new WaitForSeconds(time);
            PlayerActions.OnActionPerformed();
            yield return new WaitForSeconds(0.3f);
            PlayerActions.OnActionCancelled();
        }
    }
}
