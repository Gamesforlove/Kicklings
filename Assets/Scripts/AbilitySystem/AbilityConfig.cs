using System;
using UnityEngine;

[Serializable]
public abstract class AbilityConfig : ScriptableObject
{
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public AbilityName AbilityName { get; private set; }
    public abstract IAbility CreateAbility(AbilityActor owner);
}

public enum AbilityName
{
    Pass,
    Fake,
    TimeFreeze
}

