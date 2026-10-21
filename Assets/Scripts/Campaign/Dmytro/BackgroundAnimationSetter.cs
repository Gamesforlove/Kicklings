using Scene_Management;
using UnityEngine;

public class BackgroundAnimationSetter : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _animator.enabled = false;
        SetBackground(MatchFlow.Match.Settings.LevelData.Background);
    }
    public void SetBackground(Sprite sprite, RuntimeAnimatorController AC = null)
    {
        if (AC)
        {
            _animator.runtimeAnimatorController = AC;
        }
        else
        {
            if (sprite)
            {
                _spriteRenderer.sprite = sprite;
                return;
            }
        }
        _animator.enabled = true;
    }
}
