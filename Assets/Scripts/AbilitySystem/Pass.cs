using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Pass : IAbility
{
    public float PassFlyTime = .5f;
    private PassConfig config;    
    private AbilityActor owner;
    public Pass(PassConfig config, AbilityActor owner)
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
        var touchTask = TaskUtils.WaitForEvent(
            h => owner.BallTouched += h,
            h => owner.BallTouched -= h
        );
        await touchTask;

        Debug.Log($"Pass: + {owner.gameObject.name}");
        Transform BallTargetPosition = GetTeammate(ctx).BallPoint;
        float expieredTime = 0;
        float progress = 0;
        Vector2 newPos;
        Vector2 startPos = ctx.Ball.transform.position;
        Debug.DrawLine(startPos, BallTargetPosition.position, Color.red, 5f);


        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ctx.Ball.Rigidbody.linearVelocity = Vector2.zero;
        ctx.Ball.Collider.enabled = false;

        while (progress < 1)
        {
            expieredTime += Time.deltaTime;
            progress = expieredTime / PassFlyTime;
            newPos = ((Vector2)BallTargetPosition.position - startPos) * progress;
            ctx.Ball.transform.position = startPos + newPos;
            await Task.Yield();
        }

        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        ctx.Ball.Collider.enabled = true;
    }
    public IEnumerator ExecuteCoroutine(AbilityExecutionContext ctx)
    {
        yield return new CoroutineUtils.WaitForEvent(
                    h => owner.BallTouched += h,
                    h => owner.BallTouched -= h
                    );

        Debug.Log($"Pass: + {owner.gameObject.name}");
        Transform BallTargetPosition = GetTeammate(ctx).BallPoint;
        float expieredTime = 0;
        float progress = 0;
        Vector2 newPos;
        Vector2 startPos = ctx.Ball.transform.position;
        Debug.DrawLine(startPos, BallTargetPosition.position, Color.red, 5f);


        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ctx.Ball.Rigidbody.linearVelocity = Vector2.zero;
        ctx.Ball.Collider.enabled = false;

        while (progress < 1)
        {
            expieredTime += Time.deltaTime;
            progress = expieredTime / PassFlyTime;
            newPos = ((Vector2)BallTargetPosition.position - startPos) * progress;
            ctx.Ball.transform.position = startPos + newPos;
            yield return null;
        }

        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        ctx.Ball.Collider.enabled = true;
    }

    private AbilityActor GetTeammate(AbilityExecutionContext ctx)
    {
        AbilityActor teammate = ctx.Players.First(x => x.Team == owner.Team && x.PlayerType != owner.PlayerType);
        return teammate;
    }
}
