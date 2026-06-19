using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static CommonDataTypes.TeamsData;

public class Pass : IAbility
{
    public bool ExecutableOnKick { get { return config.ExecutableOnKick; } }

    private PassConfig config;    
    private AbilityActor owner;
    private Coroutine passRoutine = null;
    private AbilityExecutionContext ctx;
    private Vector3 _blockerPosition;
    public Pass(PassConfig config, AbilityActor owner)
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
        Vector2 teammate = GetTeammate(ctx).transform.position;
        Vector2 oponentGoalkeeper = GetOponentGoalkeeper(ctx).transform.position;
        Vector2 oponentPlayer = GetOponentPlayer(ctx).transform.position;
        if (owner.transform.position.x > teammate.x && owner.Team == Team.Left) yield break;
        if (owner.transform.position.x < teammate.x && owner.Team == Team.Right) yield break;
        if (owner.Team == Team.Left)
        {
            if (teammate.x < oponentPlayer.x && teammate.x < oponentGoalkeeper.x) yield break;
        }
        else
        {
            if (teammate.x > oponentPlayer.x && teammate.x > oponentGoalkeeper.x) yield break;
        }
        _blockerPosition = FoundBlockerPosition(oponentGoalkeeper, oponentPlayer);
        this.ctx = ctx;
        owner.BallTouched += OnPlayerTouchedBall;
        if (!config.ExecutableOnKick)
        {
            owner.Player._playerActions.Kick();
        }
        yield return new WaitForSeconds(.5f);
        owner.BallTouched -= OnPlayerTouchedBall;
    }

    private IEnumerator MakePass(AbilityExecutionContext ctx)
    {
        Debug.Log($"{owner.gameObject.name} + performs Pass ability");
        if (ctx == null)
        {
            Debug.LogWarning("Ability exec context in pass ability is null");
        }

        //owner.Player._playerActions.ReturnLeftLegToOriginalPosition();
        BallScript.TouchedPlayer += OnBallTouchedPlayer;

        AbilityActor Teammate = GetTeammate(ctx);
        Transform BallTargetPosition = Teammate.BallPoint;
        float expieredTime = 0;
        float progress = 0;

        Vector2 startPos = ctx.Ball.transform.position;
        Vector2 middle = (startPos + (Vector2)BallTargetPosition.position) / 2;
        middle = new Vector2(_blockerPosition.x, startPos.y + config.PassCurveHeight);
        Vector2 newPos;
        Debug.DrawLine(startPos, BallTargetPosition.position, Color.red, 5f);

        ctx.Ball.Rigidbody.excludeLayers = 1 << Teammate.gameObject.layer | 1 << owner.gameObject.layer;
        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ctx.Ball.Rigidbody.linearVelocity = Vector2.zero;

        while (progress < 1)
        {
            expieredTime += Time.deltaTime;
            progress = expieredTime / config.PassFlyTime;
/*            newPos = ((Vector2)BallTargetPosition.position - startPos) * progress;
            ctx.Ball.transform.position = startPos + newPos;*/
            newPos = QuadraticCurve.Evaluate(startPos, middle, (Vector2)BallTargetPosition.position, progress);
            ctx.Ball.transform.position =  newPos;
            yield return null;
        }

        ctx.Ball.Rigidbody.linearVelocity = Vector2.zero;
        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        ctx.Ball.Collider.enabled = true;
        ctx.Ball.Rigidbody.excludeLayers = 0;
        BallScript.TouchedPlayer -= OnBallTouchedPlayer;
        passRoutine = null;
    }

    private AbilityActor GetTeammate(AbilityExecutionContext ctx)
    {
        AbilityActor teammate = ctx.Players.First(x => x.Team == owner.Team && x.PlayerType != owner.PlayerType);
        return teammate;
    }
    private AbilityActor GetOponentPlayer(AbilityExecutionContext ctx)
    {
        AbilityActor teammate = ctx.Players.First(x => x.Team != owner.Team && x.PlayerType == Gameplay.Spawners.PlayersSpawner.PlayerType.Normal);
        return teammate;
    }
    private AbilityActor GetOponentGoalkeeper(AbilityExecutionContext ctx)
    {
        AbilityActor teammate = ctx.Players.First(x => x.Team != owner.Team && x.PlayerType == Gameplay.Spawners.PlayersSpawner.PlayerType.Goalkeeper);
        return teammate;
    }
    private Vector3 FoundBlockerPosition(Vector2 oponentGoalkeeper, Vector2 oponentPlayer)
    {
        if (owner.Team == Team.Left)
        {            
            if (owner.transform.position.x - oponentPlayer.x > owner.transform.position.x - oponentGoalkeeper.x)
                return oponentPlayer;
            else
                return oponentGoalkeeper;
        }
        else
        {
            if (owner.transform.position.x - oponentPlayer.x < owner.transform.position.x - oponentGoalkeeper.x)
                return oponentPlayer;
            else
                return oponentGoalkeeper;
        }
    }
    private void OnPlayerTouchedBall()
    {
        if (passRoutine == null)
        {
            passRoutine = owner.StartCoroutine(MakePass(ctx));
        }
    }
    private void OnBallTouchedPlayer()
    {
        if (passRoutine != null) owner.StopCoroutine(passRoutine);
        passRoutine = null;
        ctx.Ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        ctx.Ball.Collider.enabled = true;
        ctx.Ball.Rigidbody.excludeLayers = 0;
    }
}
