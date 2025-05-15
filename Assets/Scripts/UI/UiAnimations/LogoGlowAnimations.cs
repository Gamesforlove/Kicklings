using DG.Tweening;
using UnityEngine;

public class LogoGlowAnimations : MonoBehaviour
{
    [Header("Configuración Rotación")]
    [SerializeField] private float rotationSpeed = 1f; // Esta es la velocidad de rotación (grados por segundo)
    [SerializeField] private float rotationDuration = 2f; 

    private RectTransform glowRectTransform;

    private void Awake()
    {
        glowRectTransform = GetComponent<RectTransform>();
        
        if (glowRectTransform == null)
        {
            Debug.LogWarning("No hay RectTransform asignado para el brillo.");
            return;
        }

        StartRotationAnimation();
    }

    private void StartRotationAnimation()
    {
        // Rotamos el brillo continuamente hacia la derecha (eje Z)
        glowRectTransform.DOLocalRotate(
            new Vector3(0, 0, -360f), // Giro completo negativo (derecha)
            rotationDuration,
            RotateMode.LocalAxisAdd
        )
        .SetEase(Ease.Linear) // Suavizamos la animación con un Ease
        .SetLoops(-1, LoopType.Restart); // Bucle infinito
    }

    private void OnDestroy()
    {
        // Detenemos la animación al destruir el objeto
        glowRectTransform.DOKill();
    }
}