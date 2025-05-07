using System.Collections;
using UnityEngine;

namespace UI.UiSystem.Core
{
    public class UIViewWithData<T> : UIView, IUIViewWithData<T>
    {
        protected T ViewData;

        public virtual IEnumerator Show(T data)
        {
            ViewData = data;
            OnDataReceived(data);
            yield return base.Show();
        }
        
        protected virtual void OnDataReceived(T sideData)
        {
            Debug.Log($"Data received in view: {sideData}");
        }
    }
}