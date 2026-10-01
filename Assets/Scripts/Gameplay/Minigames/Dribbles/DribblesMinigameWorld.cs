using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DribblesMinigameWorld : MonoBehaviour
{
    [Header("World Prefabs")]
    [SerializeField] private Rigidbody2D playerPrefab;
    [SerializeField] private Rigidbody2D ballPrefab;
    [SerializeField] private GameObject checkpointMarkerPrefab;
    [SerializeField] private SpriteRenderer boundaryPrefab;
    [Tooltip("Disabled by default. Enables the previous sprite-ray effect for comparison.")]
    [SerializeField] private bool useSpriteCheckpointBurst;
    [SerializeField] private GameObject checkpointBurstPrefab;
    [SerializeField] private ParticleSystem checkpointParticleBurstPrefab;
    [SerializeField] private ParticleSystem finalCheckpointParticleBurstPrefab;

    [Header("World Presentation")]
    [SerializeField, Min(0.01f)] private float boundaryThickness = 0.12f;
    [SerializeField] private Color activeCheckpointColor = new Color32(255, 210, 58, 255);
    [SerializeField] private Color inactiveCheckpointColor = new Color32(218, 233, 240, 235);
    [SerializeField] private Color secondaryBurstColor = new Color32(255, 132, 32, 255);
    [SerializeField, Range(0f, 1f)] private float checkpointArrowDirectionInfluence = 0.65f;
    [SerializeField, Range(0f, 90f)] private float maximumCheckpointArrowAngle = 32f;

    [Header("Burst Animation")]
    [SerializeField, Min(0.01f)] private float checkpointBurstDuration = 0.65f;
    [SerializeField, Min(0.01f)] private float finalCheckpointBurstDuration = 1.1f;
    [SerializeField, Min(0f)] private float checkpointBurstDistance = 1.25f;
    [SerializeField, Min(0f)] private float finalCheckpointBurstDistance = 2.4f;
    [SerializeField, Min(0f)] private float checkpointBurstScale = 1.25f;
    [SerializeField, Min(0f)] private float finalCheckpointBurstScale = 1.7f;

    private readonly List<GameObject> activeEffects = new List<GameObject>();

    public sealed class CheckpointInstance
    {
        public readonly GameObject Root;
        private readonly SpriteRenderer[] renderers;
        private readonly Collider2D[] colliders;

        public CheckpointInstance(
            GameObject root,
            SpriteRenderer[] checkpointRenderers,
            Collider2D[] checkpointColliders)
        {
            Root = root;
            renderers = checkpointRenderers;
            colliders = checkpointColliders;
        }

        public void SetColor(Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = color;
        }

        public void SetCollidersEnabled(bool value)
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = value;
        }
    }

    public void CreateBoundaries(Vector2 minimum, Vector2 maximum)
    {
        if (boundaryPrefab == null)
        {
            Debug.LogError("Dribbles boundary prefab is not assigned.", this);
            return;
        }

        float width = maximum.x - minimum.x;
        float height = maximum.y - minimum.y;
        Vector2 center = (minimum + maximum) * 0.5f;
        CreateBoundary(
            "Bottom Boundary",
            new Vector2(center.x, minimum.y),
            new Vector2(width, boundaryThickness));
        CreateBoundary(
            "Top Boundary",
            new Vector2(center.x, maximum.y),
            new Vector2(width, boundaryThickness));
        CreateBoundary(
            "Left Boundary",
            new Vector2(minimum.x, center.y),
            new Vector2(boundaryThickness, height));
        CreateBoundary(
            "Right Boundary",
            new Vector2(maximum.x, center.y),
            new Vector2(boundaryThickness, height));
    }

    public Rigidbody2D SpawnPlayer(Vector2 position)
    {
        return SpawnBody(playerPrefab, position, "Dribbles player prefab is not assigned.");
    }

    public Rigidbody2D SpawnBall(Vector2 position)
    {
        return SpawnBody(ballPrefab, position, "Dribbles ball prefab is not assigned.");
    }

    public CheckpointInstance SpawnCheckpoint(
        Transform checkpoint,
        Transform nextCheckpoint,
        float halfWidth)
    {
        if (checkpointMarkerPrefab == null)
        {
            Debug.LogError("Dribbles checkpoint marker prefab is not assigned.", this);
            return null;
        }

        GameObject marker = Instantiate(checkpointMarkerPrefab, checkpoint);
        marker.name = checkpointMarkerPrefab.name;
        marker.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        marker.transform.localScale = Vector3.one;

        Transform leftPost = marker.transform.Find("Left Post");
        Transform rightPost = marker.transform.Find("Right Post");
        if (leftPost != null)
            leftPost.localPosition = Vector3.left * halfWidth;
        if (rightPost != null)
            rightPost.localPosition = Vector3.right * halfWidth;

        Transform arrowRoot = marker.transform.Find("Direction Arrow");
        if (arrowRoot != null && nextCheckpoint != null)
        {
            Vector2 localCheckpointDirection = marker.transform.InverseTransformDirection(
                nextCheckpoint.position - marker.transform.position);
            if (localCheckpointDirection.sqrMagnitude > Mathf.Epsilon)
            {
                Vector2 arrowDirection = Vector2.Lerp(
                    Vector2.up,
                    localCheckpointDirection.normalized,
                    checkpointArrowDirectionInfluence).normalized;
                float arrowAngle = Mathf.Clamp(
                    Vector2.SignedAngle(Vector2.up, arrowDirection),
                    -maximumCheckpointArrowAngle,
                    maximumCheckpointArrowAngle);
                arrowRoot.localRotation = Quaternion.Euler(0f, 0f, arrowAngle);
            }
        }

        return new CheckpointInstance(
            marker,
            marker.GetComponentsInChildren<SpriteRenderer>(true),
            marker.GetComponentsInChildren<Collider2D>(true));
    }

    public void SetCheckpointActive(CheckpointInstance checkpoint, bool isActive)
    {
        if (checkpoint == null)
            return;

        checkpoint.Root.SetActive(true);
        checkpoint.SetColor(isActive ? activeCheckpointColor : inactiveCheckpointColor);
        checkpoint.SetCollidersEnabled(isActive);
    }

    public void PulseCheckpoint(CheckpointInstance checkpoint, float pulse)
    {
        if (checkpoint == null)
            return;

        checkpoint.SetColor(Color.Lerp(activeCheckpointColor, Color.white, pulse));
    }

    public void PlayCheckpointBurst(Vector2 position, bool isFinalCheckpoint)
    {
        if (!useSpriteCheckpointBurst)
        {
            PlayParticleCheckpointBurst(position, isFinalCheckpoint);
            return;
        }

        if (checkpointBurstPrefab == null)
            return;

        GameObject effect = Instantiate(checkpointBurstPrefab, position, Quaternion.identity, transform);
        effect.name = isFinalCheckpoint ? "Final Checkpoint Burst" : "Checkpoint Burst";
        activeEffects.Add(effect);
        StartCoroutine(AnimateCheckpointBurst(effect, isFinalCheckpoint));
    }

    public void ClearCheckpointEffects()
    {
        StopAllCoroutines();
        for (int i = 0; i < activeEffects.Count; i++)
        {
            if (activeEffects[i] != null)
                Destroy(activeEffects[i]);
        }

        activeEffects.Clear();
    }

    private Rigidbody2D SpawnBody(
        Rigidbody2D prefab,
        Vector2 position,
        string missingPrefabMessage)
    {
        if (prefab == null)
        {
            Debug.LogError(missingPrefabMessage, this);
            return null;
        }

        Rigidbody2D body = Instantiate(prefab, position, Quaternion.identity, transform);
        body.name = prefab.name;
        return body;
    }

    private void PlayParticleCheckpointBurst(Vector2 position, bool isFinalCheckpoint)
    {
        ParticleSystem prefab = isFinalCheckpoint
            ? finalCheckpointParticleBurstPrefab
            : checkpointParticleBurstPrefab;
        if (prefab == null)
            return;

        ParticleSystem effect = Instantiate(prefab, position, Quaternion.identity, transform);
        effect.name = isFinalCheckpoint
            ? "Final Checkpoint Particle Burst"
            : "Checkpoint Particle Burst";
        activeEffects.Add(effect.gameObject);
        effect.Play(true);
    }

    private void CreateBoundary(string objectName, Vector2 position, Vector2 size)
    {
        SpriteRenderer boundary = Instantiate(boundaryPrefab, position, Quaternion.identity, transform);
        boundary.name = objectName;
        Vector2 spriteSize = Vector2.one;
        if (boundary.sprite != null)
            spriteSize = boundary.sprite.bounds.size;
        boundary.transform.localScale = new Vector3(
            size.x / Mathf.Max(spriteSize.x, Mathf.Epsilon),
            size.y / Mathf.Max(spriteSize.y, Mathf.Epsilon),
            1f);
    }

    private IEnumerator AnimateCheckpointBurst(GameObject effect, bool isFinalCheckpoint)
    {
        SpriteRenderer[] particles = effect.GetComponentsInChildren<SpriteRenderer>();
        Vector3[] initialScales = new Vector3[particles.Length];
        Vector3[] directions = new Vector3[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            initialScales[i] = particles[i].transform.localScale;
            directions[i] = particles[i].transform.up;
            particles[i].color = i % 2 == 0
                ? activeCheckpointColor
                : isFinalCheckpoint ? Color.white : secondaryBurstColor;
        }

        float duration = isFinalCheckpoint
            ? finalCheckpointBurstDuration
            : checkpointBurstDuration;
        float distance = isFinalCheckpoint
            ? finalCheckpointBurstDistance
            : checkpointBurstDistance;
        float finalScale = isFinalCheckpoint
            ? finalCheckpointBurstScale
            : checkpointBurstScale;
        float elapsed = 0f;
        while (elapsed < duration && effect != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < particles.Length; i++)
            {
                Transform particle = particles[i].transform;
                particle.localPosition = directions[i] * Mathf.Lerp(0.1f, distance, progress);
                particle.localScale = initialScales[i] * Mathf.Lerp(1f, finalScale, progress);
                Color color = particles[i].color;
                color.a = 1f - progress;
                particles[i].color = color;
            }

            yield return null;
        }

        activeEffects.Remove(effect);
        if (effect != null)
            Destroy(effect);
    }
}
