using UnityEngine;
using UnityEngine.Events;

public class BallLauncher : MonoBehaviour
{
    
    public GameObject _ballPrefab;
    public Rigidbody2D _ball;

    public Transform launchPoint;  
    public Transform target;        
    
    public float launchSpeed;
    public float minLaunchSpeed;
    public float maxLaunchSpeed;
    public float arcHeight;
    public float minArc;
    public float maxArc;
    public float spreadDegrees = 10f;
    
    public bool autoLaunch = true;
    public float minDelay = 3f;
    public float maxDelay = 4f;

    [Header("Events")]
  
    public Vector3UnityEvent onLaunch;

    public GameObject go;

    public RigidbodyUnityEvent onBeforeLaunch;

    private float nextLaunchTime;

    private void Start()
    {
        if (launchPoint == null) launchPoint = transform;
        ScheduleNextLaunch();
        go = Instantiate(_ballPrefab);
        go.SetActive(false);
        _ball = go.GetComponent<Rigidbody2D>();
        
    }

    private void Update()
    {
        if (autoLaunch && Time.time >= nextLaunchTime)
        {
            Debug.Log("Launching " + target.name);
            Fire();
            ScheduleNextLaunch();
        }
    }

    private void ScheduleNextLaunch()
    {
        nextLaunchTime = Time.time + Random.Range(minDelay, maxDelay);
    }


    public void Fire()
    {
        go.SetActive(true);
        if (_ball == null || target == null)
        {
            return;
        }

        arcHeight = Random.Range(minArc, maxArc);
        launchSpeed = Random.Range(minLaunchSpeed, maxLaunchSpeed);

        _ball.position = launchPoint.position;
        _ball.linearVelocity = Vector3.zero;

        Vector3 velocity = CalculateVelocity(launchPoint.position, target.position, launchSpeed, arcHeight);
        velocity = ApplySpread(velocity, spreadDegrees);

        _ball.linearVelocity = velocity;

        onLaunch?.Invoke(velocity);
    }

    public void DespawnBall()
    {
        go.SetActive(false);
    }

    private Vector3 CalculateVelocity(Vector3 origin, Vector3 destination, float speed, float height)
    {
        Vector3 toTarget = destination - origin;
        Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);

        Vector3 velocity = toTarget.normalized * speed;

        if (height > 0f && flatDir.magnitude > 0.01f)
        {
            velocity.y += Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * height);
        }

        return velocity;
    }

    private Vector3 ApplySpread(Vector3 velocity, float degrees)
    {
        if (degrees <= 0f) return velocity;
        Quaternion spread = Quaternion.Euler(
            Random.Range(-degrees, degrees),
            Random.Range(-degrees, degrees),
            0f);
        return spread * velocity;
    }
}

[System.Serializable] public class Vector3UnityEvent : UnityEvent<Vector3> { }
[System.Serializable] public class RigidbodyUnityEvent : UnityEvent<Rigidbody> { }