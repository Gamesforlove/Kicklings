using System.Collections;
using Gameplay.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.CharacterComponents.Cpu
{
    public class Cpu : Entity
    {
        const float JumpTimer = 1f;

        Transform _proximity1;
        Transform _proximity2;
        
        BallManager _ballManager;
        BallScript _ball;
        
        bool _jumpOnCd;

        public override void SetUp()
        {
            base.SetUp();
            CacheProximitySensors();
            PlayerInput.enabled = false;
            PlayerIndicator.gameObject.SetActive(false);
        }

        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            StartCoroutine(JumpLoop(5f));
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
            StartCoroutine(JumpLoop(5f));
        }

        void Update()
        {
            CpuPlayer();
        }

        IEnumerator JumpLoop(float time)
        {
            yield return new WaitForSeconds(time);
            DoAction();
            StartCoroutine(JumpLoop(Random.Range(3f, 10f)));
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
            Debug.Log(_jumpOnCd);
            if (_jumpOnCd) return;
            
            _jumpOnCd = true;
            float tm = Random.Range(0.05f, 0.3f);
            StartCoroutine(RandomReflex(tm));
        }
        
        IEnumerator JumpCd()
        {
            yield return new WaitForSeconds(JumpTimer);
            _jumpOnCd = false;
        }
        
        IEnumerator RandomReflex(float time)
        {
            yield return new WaitForSeconds(time);
            PlayerActions.OnActionPerformed();
            StartCoroutine(JumpCd());
            yield return new WaitForSeconds(0.3f);
            PlayerActions.OnActionCancelled();
        }
    }
}
