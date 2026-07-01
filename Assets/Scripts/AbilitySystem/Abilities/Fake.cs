using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Fake : IAbility
{
    public bool ExecutableOnKick { get { return config.ExecutableOnKick; } }

    private FakeConfig config;
    private AbilityActor owner;
    public Fake(FakeConfig config, AbilityActor owner)
    {
        this.config = config;
        this.owner = owner;
    }

    public bool CanExecute()
    {
        throw new System.NotImplementedException();
    }
    public IEnumerator ExecuteCoroutine(AbilityExecutionContext ctx)
    {
        owner.Player._playerActions.DisableKickingLeg();
        owner.Player._playerActions.Kick();
        yield return new WaitForSeconds(.1f); 
        owner.Player._playerActions.ReturnLeftLegToOriginalPosition();
        yield return new WaitForSeconds(.1f);
        owner.Player._playerActions.EnableKickingLeg();
    }
}
