using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.UiSystem.Core
{
    public class UIViewsManager : MonoBehaviour
    {
        [SerializeField] UIView _initialPage;
        [SerializeField] GameObject _firstFocusItem;

        Canvas _rootCanvas;
        
        readonly Stack<UIView> _viewsHistory = new();

        void Awake()
        {
            _rootCanvas = GetComponent<Canvas>();
        }

        void Start()
        {
            if (_firstFocusItem != null)
                EventSystem.current.SetSelectedGameObject(_firstFocusItem);

            if (_initialPage != null)
                ShowView(_initialPage);
        }

        void OnCancel()
        {
            if (_rootCanvas.enabled && _rootCanvas.gameObject.activeInHierarchy)
            {
                if (_viewsHistory.Count != 0)
                {
                    HideView();
                }
            }
        }

        public void ShowView(UIView view) => StartCoroutine(ShowViewRoutine(view));

        public void ShowView<T>(UIView view, T data)
        {
            if (view is IUIViewWithData<T> dataView)
            {
                StartCoroutine(dataView.Show(data));
                _viewsHistory.Push(view);
            }
            else
            {
                Debug.LogWarning($"View {view.name} does not support data of type {typeof(T)}");
                ShowView(view); // fallback to default
            }
        }

        public void HideView() => StartCoroutine(HideViewRoutine());

        public void TransitionToView(UIView view) => StartCoroutine(TransitionToViewRoutine(view));
        
        public bool IsPageInStack(UIView view) => _viewsHistory.Contains(view);

        public bool IsPageOnTopOfStack(UIView view) => _viewsHistory.Count > 0 && view == _viewsHistory.Peek();

        public void PopAllPages()
        {
            for (int i = 1; i < _viewsHistory.Count; i++)
            {
                HideView();
            }
        }

        IEnumerator ShowViewRoutine(UIView uiView)
        {
            yield return uiView.Show();
            _viewsHistory.Push(uiView);
        }

        IEnumerator HideViewRoutine()
        {
            if (_viewsHistory.Count <= 1)
            {
                Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
                yield break;
            }
            
            UIView page = _viewsHistory.Pop();
            yield return page.Hide();
        }
        
        IEnumerator TransitionToViewRoutine(UIView view)
        {
            yield return HideViewRoutine();
            yield return ShowViewRoutine(view);
        }
        
#if UNITY_EDITOR
        // Internal accessor for custom editor use only
        public Stack<UIView> GetDebugStack() => _viewsHistory;
#endif
    }
}
