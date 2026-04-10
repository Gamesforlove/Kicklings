using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Pass", menuName = "Scriptable Objects/Abilities/Pass")]
public class PassConfig : AbilityConfig
{
    public override IAbility CreateAbility(AbilityActor owner) => new Pass(this, owner);
}
