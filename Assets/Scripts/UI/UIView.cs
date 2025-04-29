using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public enum EntryMode { DoNothing, Slide, Zoom, Fade}
    public enum Direction { None, Up, Down, Left, Right }
    
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UIView : MonoBehaviour
    {
        RectTransform _rectTransform;
        CanvasGroup _canvasGroup;
        
        [SerializeField] float _animationSpeed = 1f;
        [SerializeField] EntryMode _entryMode =  EntryMode.Slide;
        [SerializeField] Direction _entryDirection = Direction.Left;
        [SerializeField] EntryMode _exitMode =  EntryMode.Slide;
        [SerializeField] Direction _exitDirection = Direction.Left;
        
        [SerializeField] UnityEvent _preShowAction;
        [SerializeField] UnityEvent _postShowAction;
        [SerializeField] UnityEvent _preHideAction;
        [SerializeField] UnityEvent _postHideAction;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            gameObject.SetActive(false);
            switch (_entryMode)
            {
                case EntryMode.Slide:
                    RectTransform parent = _rectTransform.parent as RectTransform;
                    Vector2 offset = GetSlideOffset(_entryDirection, parent);
                    _rectTransform.anchoredPosition = offset;
                    break;
                case EntryMode.Zoom:
                    _rectTransform.localScale = Vector3.zero;
                    break;
                case EntryMode.Fade:
                    _canvasGroup.alpha = 0;
                    break;
            }
        }

        public IEnumerator Show()
        {
            gameObject.SetActive(true);
            _preShowAction?.Invoke();

            switch (_entryMode)
            {
                case EntryMode.Slide:
                    yield return SlideIn().WaitForCompletion();
                    break;
                case EntryMode.Zoom:
                    yield return ZoomIn().WaitForCompletion();
                    break;
                case EntryMode.Fade:
                    yield return FadeIn().WaitForCompletion();
                    break;
            }
            
            _postShowAction?.Invoke();
        }

        public IEnumerator Hide()
        {
            _preHideAction?.Invoke();
            
            switch (_exitMode)
            {
                case EntryMode.Slide:
                    yield return SlideOut().WaitForCompletion();
                    break;
                case EntryMode.Zoom:
                    yield return ZoomOut().WaitForCompletion();
                    break;
                case EntryMode.Fade:
                    yield return FadeOut().WaitForCompletion();
                    break;
            }
            
            _postHideAction?.Invoke();
            gameObject.SetActive(false);
        }
        
        Tween SlideIn()
        {
            return _rectTransform.DOAnchorPos(Vector2.zero, _animationSpeed);
        }
        
        Tween ZoomIn()
        {
            return _rectTransform.DOScale(Vector3.one, _animationSpeed);
        }

        Tween FadeIn()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            
            return _canvasGroup.DOFade(1, _animationSpeed);
        }
        
        Tween SlideOut()
        {
            RectTransform parent = _rectTransform.parent as RectTransform;
            Vector2 offset = GetSlideOffset(_exitDirection, parent);

            return _rectTransform.DOAnchorPos(offset, _animationSpeed);
        }
        
        Tween ZoomOut()
        {
            return _rectTransform.DOScale(Vector3.zero, _animationSpeed);
        }

        Tween FadeOut()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            
            return _canvasGroup.DOFade(0, _animationSpeed);
        }
        
        Vector2 GetSlideOffset(Direction direction, RectTransform parent)
        {
            Vector2 size = parent.rect.size;

            return direction switch
            {
                Direction.Up => new Vector2(0, size.y),
                Direction.Down => new Vector2(0, -size.y),
                Direction.Left => new Vector2(-size.x, 0),
                Direction.Right => new Vector2(size.x, 0),
                _ => Vector2.zero
            };
        }
    }
}
