using UnityEngine;

public class DemoOpenMenu : MonoBehaviour
{
    [SerializeField] GameObject TargetMenu;

    public void OpenMenu(){
        TargetMenu.SetActive(true);
    }
}
