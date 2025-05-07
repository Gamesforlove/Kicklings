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

        public bool IsPageInStack(UIView Page)
        {
            return _viewsHistory.Contains(Page);
        }

        public bool IsPageOnTopOfStack(UIView Page)
        {
            return _viewsHistory.Count > 0 && Page == _viewsHistory.Peek();
        }

        public void ShowView(UIView uiView)
        {
            StartCoroutine(uiView.Show());
            _viewsHistory.Push(uiView);
        }
        
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

        public void HideView()
        {
            if (_viewsHistory.Count <= 1)
            {
                Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
                return;
            }
            
            UIView page = _viewsHistory.Pop();
            StartCoroutine(page.Hide());
        }

        public void PopAllPages()
        {
            for (int i = 1; i < _viewsHistory.Count; i++)
            {
                HideView();
            }
        }
        
#if UNITY_EDITOR
        // Internal accessor for custom editor use only
        public Stack<UIView> GetDebugStack() => _viewsHistory;
#endif
    }
}
