using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "RearKick", menuName = "Scriptable Objects/Abilities/RearKick")]
public class RearKickConfig : AbilityConfig
{
    public float KickForce = 50f;
    public float ReturnLegTime = .2f;
    public override IAbility CreateAbility(AbilityActor owner) => new RearKick(this, owner);
}
