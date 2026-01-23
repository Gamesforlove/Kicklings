using System.Collections;

namespace UI.UiSystem.Core
{
    public interface IUIViewWithData<T>
    {
        IEnumerator Show(T data);
    }
}