using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public interface IAbility
{
    Task Execute(AbilityExecutionContext ctx);
    IEnumerator ExecuteCoroutine(AbilityExecutionContext ctx);
    bool CanExecute();
}
