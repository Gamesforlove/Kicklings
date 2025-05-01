using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameplayNotification : MonoBehaviour
{
    TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public IEnumerator ShowAndHide()
    {
        _text.gameObject.transform.DOScale(Vector3.one, 0.2f).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSeconds(1f);
        _text.gameObject.transform.DOScale(Vector3.zero, 0.2f).SetUpdate(UpdateType.Normal, true);
    }

    public void ChangeColor(Color color)
    {
        _text.color = color;
    }
}