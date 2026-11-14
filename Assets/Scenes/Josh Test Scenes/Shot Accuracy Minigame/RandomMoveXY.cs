using UnityEngine;

public class RandomMoveXY : MonoBehaviour
{
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -5f;
    public float maxY = 5f;
    public float moveInterval = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        GetNewTargetPosition();
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, targetPosition, timer / moveInterval);
        if (timer >= moveInterval)
        {
            GetNewTargetPosition();
        }
    }

    void GetNewTargetPosition()
    {
        startPosition = transform.position;
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        targetPosition = new Vector3(randomX, randomY, transform.position.z);
        timer = 0f;
    }
}