using System.Threading.Tasks;
using UnityEngine;

public class Fake : IAbility
{
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

    public async Task Execute(AbilityExecutionContext ctx)
    {
        owner.Player._playerActions.DisableKickingLeg();
        owner.Player._playerActions.Kick();
        await Task.Delay(100);
        owner.Player._playerActions.ReturnLeftLegToOriginalPosition();
        await Task.Delay(100);
        owner.Player._playerActions.EnableKickingLeg();
    }
}
