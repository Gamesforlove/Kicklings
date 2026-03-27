using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.UiSystem.Core
{
    public class UIViewsManager : MonoBehaviour
    {
        public bool IsReady { get; private set; }
        
        [SerializeField] UIView _initialPage;
        [SerializeField] GameObject _firstFocusItem;
        
        readonly Stack<UIView> _viewsHistory = new();

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

        public void BackToPreviousView()
        {
            UIView currentView = _viewsHistory.Pop();
            UIView previousView = _viewsHistory.Peek();
    
            StartCoroutine(BackToPreviousViewRoutine(currentView, previousView));
        }
        
        public void TransitionToViewWithHistory(UIView historyView, UIView targetView)
        {
            if (_viewsHistory.Count > 0)
                _viewsHistory.Clear();
            
            _viewsHistory.Push(historyView);
            StartCoroutine(ShowViewRoutine(targetView));
        }


        public static UIViewsManager Instance { get; private set; }
        void Awake()
        {
            Instance = this;

            if (_firstFocusItem)
                EventSystem.current.SetSelectedGameObject(_firstFocusItem);

            if (_initialPage)
                ShowView(_initialPage);
        }

        IEnumerator ShowViewRoutine(UIView view)
        {
            yield return view.Show();
            if (!IsReady) IsReady = true;
            
            if (view.KeepOnHistory && !_viewsHistory.Contains(view))
                _viewsHistory.Push(view);
        }

        IEnumerator HideViewRoutine(UIView view)
        {
            if (_viewsHistory.Contains(view) && !view.KeepOnHistory)
                _viewsHistory.Pop();
            
            yield return view.Hide();
        }
        
        IEnumerator TransitionToViewRoutine(UIView view)
        {
            yield return HideViewRoutine(_viewsHistory.Peek());
            yield return ShowViewRoutine(view);
        }
        
        IEnumerator BackToPreviousViewRoutine(UIView currentView, UIView previousView)
        {
            yield return currentView.Hide();
            yield return previousView.Show();
        }

        
#if UNITY_EDITOR
        // Internal accessor for custom editor use only
        public Stack<UIView> GetDebugStack() => _viewsHistory;
#endif
    }
}
