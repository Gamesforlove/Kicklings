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
        
        BallManager _ballManager;
        BallScript _ball;

        CountdownTimer _actionTimer;
        
        BallProximityChecker _ballProximityChecker;

        public void SetUp(CpuConfiguration config)
        {
            base.SetUp(config.EntityData);
            _difficultySettings = config.DifficultySettings;
            
            _ballProximityChecker.SetUp(_difficultySettings.ProximityPoints);
            
            _actionTimer = new CountdownTimer(_difficultySettings.TimeBetweenKicks.RandomValue);
            _actionTimer.OnTimerStop += DoAction;
            _actionTimer.Start();
        }


        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            _actionTimer.Reset();
        }

        void Awake()
        {
            _ballProximityChecker = GetComponent<BallProximityChecker>();
            _ballManager = BallManager.Instance;
        }

        void Update()
        {
            CpuPlayer();
            _actionTimer.Tick(Time.deltaTime);
        }
    
        void CpuPlayer()
        {
            if (_ballProximityChecker.IsBallWithinRange(_ballManager.Ball.Rigidbody))
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
