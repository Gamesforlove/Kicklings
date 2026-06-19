using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Pass", menuName = "Scriptable Objects/Abilities/Pass")]
public class PassConfig : AbilityConfig
{
    [field: SerializeField] public float PassFlyTime { get; private set; } = .5f;
    [field: SerializeField] public float PassCurveHeight { get; private set; } = 10f;
    public override IAbility CreateAbility(AbilityActor owner) => new Pass(this, owner);
}
