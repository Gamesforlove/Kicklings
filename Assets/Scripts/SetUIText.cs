using UnityEngine;
using TMPro;

public class SetUIText : MonoBehaviour
{
    public string textToSet;
    public TextMeshProUGUI UIText;
    public void SetText()
    { 
        if (UIText != null)
            UIText.text = textToSet;
    }
}
