using System.Collections.Generic;
using UnityEngine;

public class AbilityExecutionContext
{
    public AbilityActor Owner;           
    public BallScript Ball;           
    public Transform TargetTransform;     
    public List<AbilityActor> Players;      
    

    public AbilityExecutionContext(AbilityActor owner)
    {
        this.Owner = owner;
    }
}
