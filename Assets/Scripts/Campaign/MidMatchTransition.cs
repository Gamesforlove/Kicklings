using UnityEngine;
using System.Collections;

public interface StageAction {
    public IEnumerator Execute();
}

public class MidMatchTransition : MonoBehaviour, StageAction
{
    public IEnumerator Execute()
    {
        yield return null;
    }
}
