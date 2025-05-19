using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
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
        }
        
        void ChangeVisuals(string controlScheme)
        {
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
    }
}
