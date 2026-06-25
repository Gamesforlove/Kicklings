using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Pass", menuName = "Scriptable Objects/Abilities/Pass")]
public class PassConfig : AbilityConfig
{
    [field: SerializeField] public float PassFlyTime { get; private set; } = .5f;
    [field: SerializeField] public float PassCurveHighHeight { get; private set; } = 10f;
    [field: SerializeField] public float PassCurveLowHeight { get; private set; } = 2f;
    public override IAbility CreateAbility(AbilityActor owner) => new Pass(this, owner);
}
