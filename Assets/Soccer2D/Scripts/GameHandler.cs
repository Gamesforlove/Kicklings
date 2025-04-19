using UnityEngine;
using System.Collections;

public class GameHandler {


    public static GameObject Ball
    {
        get
        {
            return GameObject.Find("Top");
        }
    }

    public static UILabel ScoreLabel
    {
        get
        {
            return GameObject.Find("ScoreLabel").GetComponent<UILabel>();
        }
    }

    public static string Score
    {
        get
        {
            return GameObject.Find("ScoreLabel").GetComponent<UILabel>().text;
        }

        set
        {
           GameObject.Find("ScoreLabel").GetComponent<UILabel>().text = value;
           GameObject.Find("ScoreLabel2").GetComponent<UILabel>().text = value;
        }
    }

    public static GameController GameController
    {
        get
        {
            return GameController.Instance;
        }
    }

    public static Effectrplayer Effect
    {
        get { return GameObject.Find("EffectPlayer").GetComponent<Effectrplayer>(); }
    }

    public static GameObject PluginContainer
    {
        get { return GameObject.Find("PluginContainer"); }
    }

    public static GameObject Rune
    {
        get { return GameObject.Find("Rune"); }
    }

    public static void RuneCounter(int sec)
    {

        GameObject.Find("RuneCounter2").GetComponent<UILabel>().text = sec.ToString();
        GameObject.Find("RuneCounter").GetComponent<UILabel>().text = sec.ToString();
    }
}
