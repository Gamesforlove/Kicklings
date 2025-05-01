using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
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
        
        [CustomEditor(typeof(UIViewsManager))]
        public class StackPreview : Editor {
            public override void OnInspectorGUI() {
                DrawDefaultInspector();
                
                UIViewsManager ts = (UIViewsManager)target; 
                Stack<UIView> stack = ts._viewsHistory;
                
                GUILayout.Label("Views history");
                
                foreach (UIView item in stack) {
                    GUILayout.Label(item.name); 
                }
            }
        }
    }
}
