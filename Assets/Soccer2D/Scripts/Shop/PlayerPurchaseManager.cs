using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPurchaseManager : MonoBehaviour
{
    List<string> boughtItems = new List<string>();
    public static PlayerPurchaseManager Instance;

    private static string coinsAmountValue = "coinsAmount";
    private static string boughtItemsField = "boughtItems";

    public event Action onBoughtNewItem;
    public event Action onCoinsAmountChange;

    public enum PurchasableItemsForCoins
    {
        //Skins
        ZombieSkin,
        NinjaSkin,
        AlienSkin,
        MonkSkin,
        LizardManSkin,
        SummerguySkin,
        July4thGuySkin,
        GirlFootballerSkin,
        GoldTrophySkin,
        ManchesterSkin,
        MadridSkin,
        BarcelonaSkin,
        MulanSkin,
        MunichSkin,
        LiverpoolSkin,
        LondonSkin,
        FulhamSkin,
        ParisSkin,
        TurinSkin,
        RaptorSkin,
        FlamingoSkin,
        YetiSkin,
        RedPandaSkin,
        TigerSkin,
        Kid1Skin,
        BrazilSkin,
        Sr_HsSkin,
        Jr_HsSkin,
        MindMeldSkin,
        KumaSkin,
        ChoobieSkin,

        //Balls
        GoldenBall,
        BeachBall,
        Black8ball,
        SilverBall,
        Basketball,
        Fireball,
        RedBall,
        OrangeBall,
        PurpleBall,
        BlueBall,
        LightblueBall,
        YellowBall,
        GreenBall,

        //Default
        DefaultSkin,
        DefaulBall,
    }

    [Serializable]
    public class BoughtItemsContainer
    {
        public List<String> boughtItems = new List<string>();
    }

    void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        ZPlayerPrefs.Initialize("UforiaWorldSoccer2D_481220687124797", "salt12issalt");

#if (UNITY_EDITOR)
        ZPlayerPrefs.DeleteAll();
#endif
    }

    private void Start()
    {
        string itemsString = ZPlayerPrefs.GetString(boughtItemsField, "");
        if (!string.IsNullOrEmpty(itemsString))
            boughtItems = JsonUtility.FromJson<BoughtItemsContainer>(itemsString).boughtItems;

        GameHandler.GameController.OnGameEnd += GameController_OnGameEnd;
    }

    public int coinsAmount
    {
        get
        {
            if (ZPlayerPrefs.HasKey(coinsAmountValue))
            {
                return ZPlayerPrefs.GetInt(coinsAmountValue, 0);
            }
            else
                return 0;
        }
    }

    void GameController_OnGameEnd()
    {
        if (GameHandler.GameController.gameMode == GameController.GameMode.OnePlayer)
        {
            if(GameHandler.GameController.BlueScore >= GameHandler.GameController.endScore)
            {
                AddCoins(10);
            } else if(GameHandler.GameController.RedScore >= GameHandler.GameController.endScore)
            {
                AddCoins(GameHandler.GameController.BlueScore);
            }
        }
    }

    public void AddCoins(int amount)
    {
        ZPlayerPrefs.SetInt(coinsAmountValue, coinsAmount + amount);
        ZPlayerPrefs.Save();

        if (onCoinsAmountChange != null)
            onCoinsAmountChange();
    }

    public void RemoveCoins(int amount)
    {
        ZPlayerPrefs.SetInt(coinsAmountValue, coinsAmount - amount);
        ZPlayerPrefs.Save();

        if (onCoinsAmountChange != null)
            onCoinsAmountChange();
    }

    public void BuyItemForCoins(PurchasableItemsForCoins[] items, int coinsCost)
    {
        ZPlayerPrefs.SetInt(coinsAmountValue, coinsAmount - coinsCost);
        foreach(PurchasableItemsForCoins item in items)
        {
            string id = item.ToString("g");
            if (!boughtItems.Contains(id))
                boughtItems.Add(id);
        }

        BoughtItemsContainer v = new BoughtItemsContainer();
        v.boughtItems = boughtItems;
        string itemsString = JsonUtility.ToJson(v);
        ZPlayerPrefs.SetString(boughtItemsField, itemsString);
        ZPlayerPrefs.Save();

        if(onBoughtNewItem != null)
            onBoughtNewItem();

        if (onCoinsAmountChange != null)
            onCoinsAmountChange();
    }

    public bool itemsIsBought(PurchasableItemsForCoins item)
    {
        return boughtItems.Contains(item.ToString("g"));
    }


    public List<SkinItem> GetBoughtSkins()
    {
        List<SkinItem> skinItems = new List<SkinItem>(GameController.Instance.skins);
        List<SkinItem> boughtSkinItems = new List<SkinItem>();
        boughtSkinItems = skinItems.FindAll(new Predicate<SkinItem>((skinItem) =>
        {
            return itemsIsBought(skinItem.item);
        }));

        boughtSkinItems.Insert(0, null);
        return boughtSkinItems;
    }

    public SkinItem GetSkinItem(PurchasableItemsForCoins item)
    {
        List<SkinItem> skinItems = new List<SkinItem>(GameController.Instance.skins);
        return skinItems.Find(new Predicate<SkinItem>((skinItem) =>
        {
            return skinItem.item == item;
        }));
    }

    public List<BallItem> GetBoughtBalls()
    {
        List<BallItem> ballsItems = new List<BallItem>(GameController.Instance.balls);
        List<BallItem> boughtBallsItems = new List<BallItem>();
        boughtBallsItems = ballsItems.FindAll(new Predicate<BallItem>((ballItem) =>
        {
            return itemsIsBought(ballItem.item);
        }));

        boughtBallsItems.Insert(0, GameController.Instance.balls[0]);
        return boughtBallsItems;
    }

    public BallItem GetBallItem(PurchasableItemsForCoins item)
    {
        List<BallItem> ballItems = new List<BallItem>(GameController.Instance.balls);
        return ballItems.Find(new Predicate<BallItem>((ballItem) =>
        {
            return ballItem.item == item;
        }));
    }
}