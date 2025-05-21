using System.Collections;
using Gameplay.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.CharacterComponents.Cpu
{
    public class Cpu : MonoBehaviour
    {
        [SerializeField] float _jumpTimer = 1f;
        
        [SerializeField] GameObject _proxymity1, _proxymity2, _proxymity3;
    
        PlayerActions _playerActions;
        BallManager _ballManager;
        BallScript _ball;
        
        bool _jumpOnCd;

        void Awake()
        {
            _playerActions = GetComponent<PlayerActions>();
            
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

            if ((_proxymity1.transform.position - _ball.transform.position).magnitude < 0.5f + 2 * speedMux)
            {
                Debug.Log("Near");
                ballisnear = true;
            }
            if ((_proxymity2.transform.position - _ball.transform.position).magnitude < 0.5f + 1 * speedMux)
            {
                Debug.Log("Near");
                ballisnear = true;
            }
            if ((_proxymity3.transform.position - _ball.transform.position).magnitude < 1f + 2 * speedMux)
            {
                Debug.Log("Near");
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
            yield return new WaitForSeconds(_jumpTimer);
            _jumpOnCd = false;
        }
        
        IEnumerator RandomReflex(float time)
        {
            yield return new WaitForSeconds(time);
            _playerActions.OnActionPerformed();
            StartCoroutine(JumpCd());
            yield return new WaitForSeconds(0.3f);
            _playerActions.OnActionCancelled();
        }
    }
}
