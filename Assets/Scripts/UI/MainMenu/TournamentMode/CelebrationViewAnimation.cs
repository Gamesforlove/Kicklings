using DG.Tweening;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class CelebrationViewAnimation : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Transform _winTextTransform;
        [SerializeField] Transform _raysTransform;

        [Header("Characters")]
        [SerializeField] Transform _character1Transform;
        [SerializeField] Transform _character2Transform;

        void Start()
        {
            AnimateVictoryScreen();
        }

        void AnimateVictoryScreen()
        {
            _winTextTransform.DOPunchScale(Vector3.one * 0.1f, 1f, 5, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
            
            _raysTransform.DORotate(new Vector3(0, 0, 360f), 12f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);

            _raysTransform.DOScale(1.1f, 1.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        
            AnimateCharacterJump(_character1Transform);
            AnimateCharacterJump(_character2Transform);
        }

        void AnimateCharacterJump(Transform character)
        {
            float jumpHeight = 1f;
            character.DOJump(character.position, jumpHeight, 1, 0.8f)
                .SetEase(Ease.OutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);

            character.DORotate(new Vector3(0, 0, 15f), 0.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }
    }
}
