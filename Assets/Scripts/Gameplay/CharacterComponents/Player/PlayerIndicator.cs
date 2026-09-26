using Gameplay.Managers;
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

        PlayerActions _playerActions;
        string _displayedControlScheme;

        void Awake()
        {
            _playerActions = GetComponentInParent<PlayerActions>();
        }

        public void SetUp(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _playerActions = playerInput != null ? playerInput.GetComponent<PlayerActions>() : null;
            _displayedControlScheme = null;
            RefreshControlScheme();
            RefreshVisibility();
        }

        void Start()
        {
            RefreshControlScheme();
             
            if (gameObject.transform.root.transform.position.x > 0)
                gameObject.transform.localScale = new Vector3(-1, 1, 1);

            RefreshVisibility();
        }

        void Update()
        {
            RefreshControlScheme();
            RefreshVisibility();
        }

        void RefreshControlScheme()
        {
            if (_playerInput == null || _playerInput.currentControlScheme == _displayedControlScheme)
                return;

            _displayedControlScheme = _playerInput.currentControlScheme;
            ChangeVisuals(_displayedControlScheme);
        }
         
        void ChangeVisuals(string controlScheme)
        {
            int colorIndex = -1;

            switch (controlScheme)
            {
                case "KeyboardPlayer1":
                    _text.text = "Z";
                    colorIndex = 0;
                    break;
                case "KeyboardPlayer2":
                    _text.text = "X";
                    colorIndex = 1;
                    break;
                case "KeyboardPlayer3":
                    _text.text = "N";
                    colorIndex = 2;
                    break;
                case "KeyboardPlayer4":
                    _text.text = "M";
                    colorIndex = 3;
                    break;
            }

            if (colorIndex < 0 || _playerColors == null || colorIndex >= _playerColors.Length)
                return;

            _text.color = _playerColors[colorIndex];
            _spriteRenderer.color = _playerColors[colorIndex];
        }

        void RefreshVisibility()
        {
            bool isPreferred = PlayersManager.Instance == null ||
                               PlayersManager.Instance.IsPreferredPlayerAction(
                                   _playerActions,
                                   _playerInput != null ? _playerInput.currentControlScheme : string.Empty
                               );

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = isPreferred;
            if (_text != null)
                _text.enabled = isPreferred;
        }
    }
}
