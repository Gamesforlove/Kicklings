using System.Threading.Tasks;
using UnityEngine;

public interface IAbility
{
    Task Execute(AbilityExecutionContext ctx);
    bool CanExecute();
}
