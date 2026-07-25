using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents.Player
{
    public class PlayerIndicator : MonoBehaviour
    {
        [SerializeField] PlayerInput _playerInput;
        
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] TextMeshPro _text;
        [SerializeField] Color[] _playerColors;

        void Start()
        {
            ChangeVisuals(_playerInput.currentControlScheme);
            
            if (gameObject.transform.root.transform.position.x > 0)
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            
            StartCoroutine(FadOutRoutine());
        }
        
        void ChangeVisuals(string controlScheme)
        {
            if (_playerColors == null || _playerColors.Length < 4)
                return;

            switch (controlScheme)
            {
                case "KeyboardPlayer1":
                    _text.text = "P1";
                    _text.color = _playerColors[0];
                    _spriteRenderer.color = _playerColors[0];
                    break;
                case "KeyboardPlayer2":
                    _text.text = "P2";
                    _text.color = _playerColors[1];
                    _spriteRenderer.color = _playerColors[1];
                    break;
                case "KeyboardPlayer3":
                    _text.text = "P3";
                    _text.color = _playerColors[2];
                    _spriteRenderer.color = _playerColors[2];
                    break;
                case "KeyboardPlayer4":
                    _text.text = "P4";
                    _text.color = _playerColors[3];
                    _spriteRenderer.color = _playerColors[3];
                    break;
            }
        }

        IEnumerator FadOutRoutine()
        {
            const float fadeTime = .5f;
            yield return new WaitForSeconds(1f);
            
            Sequence sequence = DOTween.Sequence();
            yield return sequence.Append(_spriteRenderer.DOFade(0, fadeTime))
                .Join(_text.DOFade(0, fadeTime))
                .WaitForCompletion();
            
            gameObject.SetActive(false);
        }
    }
}
