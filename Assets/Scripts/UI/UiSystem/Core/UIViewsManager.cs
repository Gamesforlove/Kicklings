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
        
        readonly Stack<UIView> _viewsHistory = new();

        void Start()
        {
            if (_firstFocusItem != null)
                EventSystem.current.SetSelectedGameObject(_firstFocusItem);

            if (_initialPage != null)
                ShowView(_initialPage);
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

        public void HideView(UIView view) => StartCoroutine(HideViewRoutine(view));
        
        public void HideCurrentView() => StartCoroutine(HideViewRoutine(_viewsHistory.Peek()));

        public void TransitionToView(UIView view) => StartCoroutine(TransitionToViewRoutine(view));

        IEnumerator ShowViewRoutine(UIView view)
        {
            yield return view.Show();
            if (view.KeepOnHistory)
                _viewsHistory.Push(view);
        }

        IEnumerator HideViewRoutine(UIView view)
        {
            if (_viewsHistory.Contains(view))
                _viewsHistory.Pop();
            
            yield return view.Hide();
        }
        
        IEnumerator TransitionToViewRoutine(UIView view)
        {
            yield return HideViewRoutine(_viewsHistory.Peek());
            yield return ShowViewRoutine(view);
        }
        
#if UNITY_EDITOR
        // Internal accessor for custom editor use only
        public Stack<UIView> GetDebugStack() => _viewsHistory;
#endif
    }
}
