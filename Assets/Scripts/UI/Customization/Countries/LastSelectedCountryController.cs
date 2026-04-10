using UnityEngine;
using CommonDataTypes;

public class LastSelectedCountryController : MonoBehaviour
{
    public FieldSideType lastSelectedFieldSideType = FieldSideType.None;
    public void SetFieldSideTypeNone() { lastSelectedFieldSideType = FieldSideType.None; }
    public void SetFieldSideTypeLeftSide() { lastSelectedFieldSideType = FieldSideType.Left; }
    public void SetFieldSideTypeRightSide() { lastSelectedFieldSideType = FieldSideType.Right; }
}