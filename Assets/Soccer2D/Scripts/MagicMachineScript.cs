using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMachineScript : MonoBehaviour
{
    /*public TweenAlpha noMoneyWindow;
    public UI2DSprite prize;
    public UI2DSprite popup;
    public UI2DSprite openChest;
    public UI2DSprite effect;
    public UI2DSpriteAnimation popupANimation;
    public UIWidget prizeContainer;
    public TweenAlpha flashSprite;
    private Transform spawnedItem;

    public GameObject moneyPrefab;

    public Transform dropPoint;
    public Transform spawnPoint;

    private Transform selectedItem;
    public Transform itemsContainer;

    private bool canGetPrize = true;

    public void GetPrize()
    {
        if (PlayerPurchaseManager.Instance.coinsAmount < 250 || !canGetPrize)
        {

            if (PlayerPurchaseManager.Instance.coinsAmount < 250)
                noMoneyWindow.PlayReverse();
                
            return;
        }


        Effectrplayer.instance.PlayPrizeMachineSound();
        canGetPrize = false;
        PlayerPurchaseManager.Instance.RemoveCoins(250);

        PlayerPurchaseManager.PurchasableItemsForCoins item = PlayerPurchaseManager.PurchasableItemsForCoins.TigerSkin;
        if (Random.Range(0, 1000) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.NinjaSkin;
        if (Random.Range(0, 150) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.ManchesterSkin;
        if (Random.Range(0, 100) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.BrazilSkin;
        if (Random.Range(0, 100) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.MadridSkin;
        if (Random.Range(0, 75) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.RedBall;
        if (Random.Range(0, 75) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.BarcelonaSkin;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.OrangeBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }


        item = PlayerPurchaseManager.PurchasableItemsForCoins.PurpleBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.BlueBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.LightblueBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.YellowBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.GreenBall;
        if (Random.Range(0, 50) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.MulanSkin;
        if (Random.Range(0, 40) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.MulanSkin;
        if (Random.Range(0, 30) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.LiverpoolSkin;
        if (Random.Range(0, 25) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.LondonSkin;
        if (Random.Range(0, 20) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }


        item = PlayerPurchaseManager.PurchasableItemsForCoins.FulhamSkin;
        if (Random.Range(0, 15) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.ParisSkin;
        if (Random.Range(0, 15) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        item = PlayerPurchaseManager.PurchasableItemsForCoins.TurinSkin;
        if (Random.Range(0, 5) == 0 && !PlayerPurchaseManager.Instance.itemsIsBought(item))
        {
            showBubbleCase(item);
            return;
        }

        int coinsPrize = Random.Range(5, 101);
        showBubbleCase(coinsPrize);
        //TODO Give coins
    }

    private PlayerPurchaseManager.PurchasableItemsForCoins item;
    public void showBubbleCase(PlayerPurchaseManager.PurchasableItemsForCoins item)
    {
        PlayerPurchaseManager.Instance.BuyItemForCoins(new PlayerPurchaseManager.PurchasableItemsForCoins[] { item }, 0);
        this.item = item;
        money = 0;
        StartCoroutine(DropItem());
    }

    public void showBubbleCase(int money)
    {
        PlayerPurchaseManager.Instance.AddCoins(money);
        this.money = money;
        StartCoroutine(DropItem());
    }

    private int money = 0;
    public void popupBuble()
    {
        Effectrplayer.instance.PoppingbubbleEffect();
        prize.GetComponent<TweenAlpha>().PlayReverse();
        popup.GetComponent<TweenAlpha>().PlayForward();
        popupANimation.enabled = true;
        TimerStart();
    }

    public void TimerStart()
    {
        StartCoroutine(FlashEffect());
        popup.GetComponent<TweenAlpha>().PlayReverse();
        openChest.alpha = 1f;
        effect.GetComponent<TweenAlpha>().PlayForward();
        if (money != 0)
        {
            spawnedItem = Instantiate(moneyPrefab, prizeContainer.transform).transform;
            _.Find<UILabel>(spawnedItem, "Coins Label").text = money.ToString();
            spawnedItem.transform.localPosition = Vector2.zero;
        }
        else
        {
            if (item.ToString("g").EndsWith("Skin"))
            {
                spawnedItem = Instantiate(PlayerPurchaseManager.Instance.GetSkinItem(item).uiItem, prizeContainer.transform).transform;
                spawnedItem.localScale = new Vector3(5, 5, 0f);
                spawnedItem.localPosition = new Vector2(-31f, 68f);

                UI2DSprite sprite = _.Find<UI2DSprite>(spawnedItem, "Sprite");
                if (sprite != null)
                {
                    sprite.depth = 300;
                    _.Find<UI2DSprite>(spawnedItem, "Outline").alpha = 0f;
                }
                else
                {
                    UI2DSprite[] sprites = spawnedItem.GetComponentsInChildren<UI2DSprite>();

                    foreach (UI2DSprite childSprite in sprites)
                    {
                        childSprite.depth += 300;
                    }
                }

            }
            else
            {
#if UNITY_EDITOR
                Debug.Log(item.ToString("g"));
#endif
                spawnedItem = Instantiate(PlayerPurchaseManager.Instance.GetBallItem(item).uiItem, prizeContainer.transform).transform;
                spawnedItem.localScale = new Vector3(1, 1, 0f);
                spawnedItem.localPosition = Vector2.zero;
                _.Find<UI2DSprite>(spawnedItem, "Ball Sprite").depth = 300;
                _.Find<UI2DSprite>(spawnedItem, "Outline").depth = 299;
            }
        }

    }

    public void HidePrize()
    {
        canGetPrize = true;
        effect.GetComponent<TweenAlpha>().PlayReverse();
        openChest.alpha = 0f;
        if (spawnedItem != null)
            Destroy(spawnedItem.gameObject);
    }

    IEnumerator DropItem()
    {
        selectedItem = itemsContainer.GetChild(Random.Range(0, itemsContainer.childCount));
        yield return new WaitForSeconds(2f);

        selectedItem.transform.position = dropPoint.position;
        prize.sprite2D = selectedItem.GetComponent<UI2DSprite>().sprite2D;

        yield return new WaitForSeconds(1f);
        selectedItem.GetComponent<TweenAlpha>().PlayReverse();
        prize.GetComponent<TweenAlpha>().PlayForward();

        yield return new WaitForSeconds(0.5f);

        selectedItem.transform.position = spawnPoint.position;
        selectedItem.GetComponent<TweenAlpha>().PlayForward();
    }

    IEnumerator FlashEffect()
    {
        flashSprite.PlayForward();
        yield return new WaitForSeconds(0.1f);
        flashSprite.PlayReverse();
    }*/
}
