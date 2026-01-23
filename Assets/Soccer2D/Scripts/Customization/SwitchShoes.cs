using UnityEngine;
using System.Collections;

public class SwitchShoes : MonoBehaviour {


    public enum Player
    {
        RED, BLUE
    }

    public Player player;
    public int CurrentShoes;

    public void NextShoes()
    {
        CurrentShoes++;
        if (CurrentShoes > GameHandler.GameController.Shoes.Length - 1)
            CurrentShoes = 0;
        GetComponent<SpriteRenderer>().sprite = GameHandler.GameController.Shoes[CurrentShoes];

        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedShoes(CurrentShoes);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueShoes(CurrentShoes);
        }
    }
    public void PreviousShoes()
    {
        CurrentShoes--;
        if (CurrentShoes < 0)
            CurrentShoes = GameHandler.GameController.Shoes.Length - 1;
        GetComponent<SpriteRenderer>().sprite = GameHandler.GameController.Shoes[CurrentShoes];
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedShoes(CurrentShoes);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueShoes(CurrentShoes);
        }
    }
}
