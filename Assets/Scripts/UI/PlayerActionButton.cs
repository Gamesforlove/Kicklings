using EventBusSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(Button))]
public class PlayerActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData){
        EventBus<PlayerActionPerformed>.Raise();
    }

    public void OnPointerUp(PointerEventData eventData){
        EventBus<PlayerActionCanceled>.Raise();
    }
}
