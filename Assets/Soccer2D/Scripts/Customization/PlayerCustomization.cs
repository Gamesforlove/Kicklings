using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour {

    public enum Player
    {
        RED, BLUE
    }

    public Player player;

    public int currentSkin;

    public int CurrentShirt;
    public int CurrentShoes;

    private Transform currentSkinTranform;

    private SpriteRenderer shirt;
    private SpriteRenderer shoe1;
    private SpriteRenderer shoe2;

    private void Start()
    {
        SetSkin();

        PlayerPurchaseManager.Instance.onBoughtNewItem += () =>
        {
            if (currentSkin != 0)
                SetSkin();
        };
    }

    public void SetTshirt()
    {
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedTshirts(CurrentShirt);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueTshirts(CurrentShirt);
        }
    }

    public void SetSkin()
    {
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedSkin(currentSkin);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueSkin(currentSkin);
        }

        if(currentSkinTranform != null)
            Destroy(currentSkinTranform.gameObject);

        if(currentSkin == 0)
        {
            currentSkinTranform = Instantiate(player == Player.BLUE ? GameHandler.GameController.uiDefaultBlueSkin : GameHandler.GameController.uiDefaultRedSkin, transform).transform;
        }
        else
        {
            currentSkinTranform = Instantiate(PlayerPurchaseManager.Instance.GetBoughtSkins()[currentSkin].uiItem, transform).transform;
        }

        currentSkinTranform.localPosition = Vector2.zero;

        if (currentSkin == 0)
        {
            shirt = _.Find<SpriteRenderer>(currentSkinTranform, "Universal Player ShirtDesen");
            shoe1 = _.Find<SpriteRenderer>(currentSkinTranform, "Universal Player_Foot Left");
            shoe2 = _.Find<SpriteRenderer>(currentSkinTranform, "Universal Player_Foot Right");
        }
    }

    public void SetShoes()
    {
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedShoes(CurrentShoes);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueShoes(CurrentShoes);
        }
    }

    public void NextSkin()
    {
        currentSkin++;
        if (currentSkin > PlayerPurchaseManager.Instance.GetBoughtSkins().Count - 1)
        {
            currentSkin = 0;
        }
        CurrentShirt = 0;
        CurrentShoes = 0;
        SetSkin();
    }

    public void PreviousSkin()
    {
        currentSkin--;
        if (currentSkin < 0)
        {
            currentSkin = PlayerPurchaseManager.Instance.GetBoughtSkins().Count - 1;
        }
        CurrentShirt = GameHandler.GameController.Thsirts.Length - 1;
        CurrentShoes = GameHandler.GameController.Shoes.Length - 1;
        SetSkin();
    }

    public void NextShirt()
    {
        if (currentSkin != 0)
        {
            NextSkin();
            return;
        }

        CurrentShirt++;
        if (CurrentShirt > GameHandler.GameController.Thsirts.Length - 1)
        {
            if (PlayerPurchaseManager.Instance.GetBoughtSkins().Count > 1)
            {
                CurrentShirt = GameHandler.GameController.Thsirts.Length - 1;
                NextSkin();
            }
            else
            {
                CurrentShirt = 0;
            }
        }

        shirt.sprite = GameHandler.GameController.Thsirts[CurrentShirt];
        SetTshirt();
    }

    public void PreviousShirt()
    {
        if (currentSkin != 0)
        {
            PreviousSkin();
            return;
        }

        CurrentShirt--;
        if (CurrentShirt < 0)
        {
            if (PlayerPurchaseManager.Instance.GetBoughtSkins().Count > 1)
            {
                CurrentShirt = 0;
                PreviousSkin();
            }
            else
                CurrentShirt = GameHandler.GameController.Thsirts.Length - 1;
        }

        shirt.sprite = GameHandler.GameController.Thsirts[CurrentShirt];
        SetTshirt();
    }

    public void NextShoes()
    {
        if(currentSkin != 0)
        {
            NextSkin();
            return;
        }

        CurrentShoes++;
        if (CurrentShoes > GameHandler.GameController.Shoes.Length - 1)
        {
            if (PlayerPurchaseManager.Instance.GetBoughtSkins().Count > 1)
            {
                CurrentShoes = GameHandler.GameController.Shoes.Length - 1;
                NextSkin();
            }
            else
            {
                CurrentShoes = 0;
            }
        }

        shoe1.sprite = GameHandler.GameController.Shoes[CurrentShoes];
        shoe2.sprite = GameHandler.GameController.Shoes[CurrentShoes];
        SetShoes();
    }

    public void PreviousShoes()
    {
        if (currentSkin != 0)
        {
            PreviousSkin();
            return;
        }

        CurrentShoes--;
        if (CurrentShoes < 0)
        {
            if (PlayerPurchaseManager.Instance.GetBoughtSkins().Count > 1)
            {
                CurrentShoes = 0;
                PreviousSkin();
            }
            else
                CurrentShoes = GameHandler.GameController.Shoes.Length - 1;
        }
        shoe1.sprite = GameHandler.GameController.Shoes[CurrentShoes];
        shoe2.sprite = GameHandler.GameController.Shoes[CurrentShoes];
        SetShoes();
    }
}
