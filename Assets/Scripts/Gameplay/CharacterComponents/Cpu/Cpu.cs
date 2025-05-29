using System;
using System.Collections;
using Gameplay.Managers;
using UnityEngine;

namespace Gameplay.CharacterComponents.Cpu
{
    [Serializable]
    public class CpuConfiguration
    {
        public EntityData EntityData;
        public CpuDifficultyPreset.DifficultySettings DifficultySettings;

        public CpuConfiguration(EntityData entityData, CpuDifficultyPreset.DifficultySettings difficultySettings)
        {
            EntityData = entityData;
            DifficultySettings = difficultySettings;
        }
    }

    public class Cpu : Entity
    {
        CpuDifficultyPreset.DifficultySettings _difficultySettings;

        Transform _proximity1;
        Transform _proximity2;
        
        BallManager _ballManager;
        BallScript _ball;

        CountdownTimer _actionTimer;

        public void SetUp(CpuConfiguration config)
        {
            base.SetUp(config.EntityData);
            _difficultySettings = config.DifficultySettings;
            CacheProximitySensors();
            PlayerInput.enabled = false;
            PlayerIndicator.gameObject.SetActive(false);
            
            _actionTimer = new CountdownTimer(_difficultySettings.TimeBetweenKicks.RandomValue);
            _actionTimer.OnTimerStop += DoAction;
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
            StartCoroutine(RandomReflex(_difficultySettings.ReactionTime.RandomValue));
            _actionTimer.Reset(_difficultySettings.TimeBetweenKicks.RandomValue);
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
