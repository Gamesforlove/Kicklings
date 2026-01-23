using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameHandler {


    public static GameObject Ball
    {
        get
        {
            return GameObject.Find("Top");
        }
    }

    public static Text ScoreLabel
    {
        get
        {
            return GameObject.Find("ScoreLabel").GetComponent<Text>();
        }
    }

    public static string Score
    {
        get
        {
            return GameObject.Find("ScoreLabel").GetComponent<Text>().text;
        }

        set
        {
           GameObject.Find("ScoreLabel").GetComponent<Text>().text = value;
           GameObject.Find("ScoreLabel2").GetComponent<Text>().text = value;
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

        GameObject.Find("RuneCounter2").GetComponent<Text>().text = sec.ToString();
        GameObject.Find("RuneCounter").GetComponent<Text>().text = sec.ToString();
    }
}
