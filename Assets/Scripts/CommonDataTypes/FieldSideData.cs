using UnityEngine;

namespace CommonDataTypes
{
    [CreateAssetMenu(fileName = "FieldSideDataSO", menuName = "Scriptable Objects/FieldSideDataSO")]
    public class FieldSideData : ScriptableObject
    {
        [field: SerializeField] public FieldSideType SideType { get; private set; }
        [field: SerializeField] public Color Color {get; private set;}
    }
}