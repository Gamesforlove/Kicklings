using Gameplay.CharacterComponents;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class RearKick : IAbility
{
    private RearKickConfig config;
    private AbilityActor owner;
    private bool _executed = false;
    public RearKick(RearKickConfig config, AbilityActor owner)
    {
        this.config = config;
        this.owner = owner;
    }

    public bool CanExecute()
    {
        throw new System.NotImplementedException();
    }

    public Task Execute(AbilityExecutionContext ctx)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ExecuteCoroutine(AbilityExecutionContext ctx)
    {
        _executed = false;
        owner.EntityTouched += OnEntityTouched;
        /*  var waitForEvent = new CoroutineUtils.WaitForEvent<Rigidbody2D>(
                    h => owner.EntityTouched += h,
                    h => owner.EntityTouched -= h
                    );
        yield return waitForEvent;
        Rigidbody2D entity = waitForEvent.Value;*/
        owner.Player._playerActions.Kick();
        yield return new WaitForSeconds(config.ReturnLegTime);
        owner.Player._playerActions.ReturnLeftLegToOriginalPosition();
        owner.EntityTouched -= OnEntityTouched;
    }
    private void OnEntityTouched(Rigidbody2D rigidbody)
    {
        if (!_executed)
        {
            rigidbody.AddForce(Vector2.up * config.KickForce, ForceMode2D.Impulse);
            _executed = true; 
        }
    }
}
