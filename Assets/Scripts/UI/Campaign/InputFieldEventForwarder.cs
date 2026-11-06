using System;
using UnityEngine;

public class InputFieldEventForwarder : MonoBehaviour
{
    public event Action InputFieldSelected;
    public event Action InputFieldDeselected;

    public void OnInputFieldSelected()
    {
        InputFieldSelected?.Invoke();
    }
    public void OnInputFieldDeselected()
    {
        InputFieldDeselected?.Invoke();
    }
}
