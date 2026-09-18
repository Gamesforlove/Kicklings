using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.ShotAccuracy
{

    public class ShotAccuracyManager : MonoBehaviour
    {
        [SerializeField] SweepingLine _horizontalSweep; 
        [SerializeField] SweepingLine _verticalSweep;   
        [SerializeField] ShotTarget _target;
        [SerializeField] ShotAccuracyUi _ui;

        [Header("Round")]
        [SerializeField] int _totalShots = 10;
        [SerializeField] float _roundDuration = 60f;
        [SerializeField] int _scoreTarget = 600;

        [Header("Shot")]
        [SerializeField] int _pointsPerPerfectShot = 100;
        [SerializeField] float _resolveDelay = 1f;
        [SerializeField] bool _startOnAwake = true;

        enum Phase { Idle, AimingX, AimingY, Resolving, Finished }
        Phase _phase = Phase.Idle;

        int _score;
        int _shotsUsed;
        float _timeRemaining;
        float _shotX, _shotY;

        public bool RoundDone => _phase == Phase.Finished;
        public int Score => _score;

        void Start()
        {
            if (_startOnAwake) StartRound();
        }

        public void StartRound()
        {
            _score = 0;
            _shotsUsed = 0;
            _timeRemaining = _roundDuration;

            _ui?.InitializeRound(_scoreTarget, _roundDuration, _totalShots);
            _ui?.UpdateScore(_score, _scoreTarget);

            BeginShot();
        }

        void BeginShot()
        {
            _ui?.ClearFeedback();
            _ui?.UpdateShots(_shotsUsed, _totalShots);

            _target?.Randomize();
            _target?.SetVisible(true);

            _verticalSweep?.Hide();
            _horizontalSweep?.BeginSweep();

            _phase = Phase.AimingX;
        }

        void Update()
        {
            if (_phase == Phase.Idle || _phase == Phase.Finished) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopCurrentLine();
            }

            _timeRemaining -= Time.deltaTime;
            _ui?.UpdateTimer(Mathf.Max(_timeRemaining, 0f));

            if (_timeRemaining <= 0f) EndRound();
        }
        
        public void OnStopInput(InputAction.CallbackContext context)
        {
            if (context.performed) StopCurrentLine();
        }
        


        public void StopCurrentLine()
        {
            switch (_phase)
            {
                case Phase.AimingX:
                    _shotX = _horizontalSweep != null ? _horizontalSweep.StopSweep() : 0f;
                    _verticalSweep?.BeginSweep();
                    _phase = Phase.AimingY;
                    break;

                case Phase.AimingY:
                    _shotY = _verticalSweep != null ? _verticalSweep.StopSweep() : 0f;
                    _phase = Phase.Resolving;
                    StartCoroutine(ResolveShot());
                    break;
            }
        }

        IEnumerator ResolveShot()
        {
            Vector2 placement = new Vector2(_shotX, _shotY);

            float accuracy = _target != null ? _target.Evaluate(placement) : 0f;
            int points = Mathf.RoundToInt(accuracy * _pointsPerPerfectShot);

            _score += points;
            _shotsUsed++;

            _ui?.UpdateScore(_score, _scoreTarget);
            _ui?.ShowShotFeedback(accuracy, points);

            yield return new WaitForSeconds(_resolveDelay);

            if (_shotsUsed >= _totalShots || _timeRemaining <= 0f)
            {
                EndRound();
                yield break;
            }

            BeginShot();
        }

        void EndRound()
        {
            if (_phase == Phase.Finished) return;
            _phase = Phase.Finished;

            _horizontalSweep?.Hide();
            _verticalSweep?.Hide();
            _target?.SetVisible(false);

            //_ui?.ShowResult(_score >= _scoreTarget, _score, _scoreTarget);
        }
    }
}