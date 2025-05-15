using UnityEngine;
using DG.Tweening;

public class LogoTextAnimations : MonoBehaviour
{
    private RectTransform logoRectTransform; // Referencia al RectTransform del logo
    [SerializeField] private float animationDuration = 1f; // Duración de cada animación
    [SerializeField] private float pulseScale = 1.2f; // Escala máxima del "pulse"
    [SerializeField] private float rotationAngle = 15f; // Ángulo de rotación máxima

    private void Awake()
    {
        logoRectTransform = GetComponent<RectTransform>();
        PlayLogoAnimation();
    }

    private void PlayLogoAnimation()
    {
        // Animación de Pulse (esta es la de incremento de escala)
        logoRectTransform.DOScale(pulseScale, animationDuration)
            .SetEase(Ease.InOutSine) // Suavizamos la animación con un Ease
            .SetLoops(-1, LoopType.Yoyo); // Repitimos infinitamente en "ping-pong"

        // Animación de Rotación (izquierda/derecha)
        logoRectTransform.DOLocalRotate(new Vector3(0, 0, rotationAngle), animationDuration)
            .SetEase(Ease.InOutSine) // Suavizamos la animación con un Ease
            .SetLoops(-1, LoopType.Yoyo); // Repitimos infinitamente en "ping-pong"
    }

    private void OnDestroy()
    {
        // Detenemos las animaciones al destruir el objeto
        logoRectTransform.DOKill();
    }
}
