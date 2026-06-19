using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "FakeConfig", menuName = "Scriptable Objects/Abilities/Fake")]
public class FakeConfig : AbilityConfig
{
    public override IAbility CreateAbility(AbilityActor owner) => new Fake(this, owner);
}
