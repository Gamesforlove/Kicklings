using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public interface IAbility
{
    IEnumerator ExecuteCoroutine(AbilityExecutionContext ctx);
    bool CanExecute();
    public bool ExecutableOnKick { get; }
}
