using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyEffectManager : MonoBehaviour {

    /*public TweenAlpha effect;
    public UIWidget itemContiainer;
    public TweenAlpha noMoneyEffect;

    private GameObject spawnedItem;

    public void ShowEffect(UIWidget showItem, bool isCoins = false)
    {
        StartCoroutine(FlashEffect(showItem, isCoins));
    }


    IEnumerator FlashEffect(UIWidget showItem, bool isCoins)
    {
        effect.PlayForward();

        spawnedItem = Instantiate(showItem.gameObject, itemContiainer.transform);
        spawnedItem.GetComponent<UIWidget>().SetAnchor((Transform)null);
        spawnedItem.GetComponent<UIWidget>().ResetAndUpdateAnchors();
        spawnedItem.transform.localPosition = Vector3.zero;
        spawnedItem.transform.localScale = new Vector2(1.5f, 1.5f);


        var allChildren = spawnedItem.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.GetComponent<UILabel>() && !isCoins)
                child.GetComponent<UILabel>().alpha = 0f;

            if (child.GetComponent<UIWidget>())
                child.GetComponent<UIWidget>().depth = 300;
        }


        yield return new WaitForSeconds(2.5f);

        _.DestroyAllChildren(itemContiainer.transform);

        effect.PlayReverse();
    }*/
}
