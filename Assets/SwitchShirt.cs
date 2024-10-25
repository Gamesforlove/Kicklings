using UnityEngine;
using System.Collections;

public class SwitchShirt : MonoBehaviour {

    public enum Player{
        RED,BLUE
    }

    public Player player;

    public int CurrentShirt;

    public void NextShirt()
    {
        CurrentShirt++;
        if (CurrentShirt > GameHandler.GameController.Thsirts.Length-1)
            CurrentShirt = 0;
        GetComponent<SpriteRenderer>().sprite = GameHandler.GameController.Thsirts[CurrentShirt];
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedTshirts(CurrentShirt);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueTshirts(CurrentShirt);
        }
    }
    public void PreviousShirt()
    {
        CurrentShirt--;
        if (CurrentShirt < 0)
            CurrentShirt = GameHandler.GameController.Thsirts.Length-1;
        GetComponent<SpriteRenderer>().sprite = GameHandler.GameController.Thsirts[CurrentShirt];
        if (player == Player.RED)
        {
            GameHandler.GameController.SetRedTshirts(CurrentShirt);
        }
        if (player == Player.BLUE)
        {
            GameHandler.GameController.SetBlueTshirts(CurrentShirt);
        }
    }
}
