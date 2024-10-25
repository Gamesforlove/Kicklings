using UnityEngine;
using System.Collections;

public delegate void EventHandler();
public class GameController : MonoBehaviour {

    public bool twoButtonControl;
    public GameObject UIRoot;
    public GameObject inMatchCurrentTeamFlag;
    public GameObject inMatchOpponentTeamFlag;

    public int BlueScore;
    public int RedScore;
    public GameObject Rune;
    public GameObject menuEndessToggleButton;
    public int endScore;
    public int runeSpawnTime = 30;
    public event EventHandler OnP1JumpPress;
    public event EventHandler OnP1JumpRelease;

    public event EventHandler OnP1JumpPress1;
    public event EventHandler OnP1JumpRelease1;

    public event EventHandler OnP2JumpPress;
    public event EventHandler OnP2JumpRelease;

    public event EventHandler OnP2JumpPress1;
    public event EventHandler OnP2JumpRelease1;

    public event EventHandler OnGameEnd;
    public event EventHandler OnGameStart;
    public event EventHandler OnNextRaund;

    public event EventHandler OnFiveGoal;

    public bool randomOutfits;
    public int BlueCurrentTshirt;
    public int BlueCurrentShoes;
    public int RedCurrentTshirt;
    public int RedCurrentShoes;
    public bool isReplayEnabled;
    public GameMode gameMode;
    public bool finished;
    bool ballRedOrBlue;
    public int totalGoal;
    public bool isPlaying;
    public bool isPaused;
    public bool isOnMenu;
    public bool onExitingDialog;
    KeyCode keyCode = KeyCode.Escape;
    public enum GameMode
    {
        OnePlayer, TwoPlayers, CpuVsCpu
    }

    public enum Player
    {

        Red, Blue
    }

    public void ToggleReplays(bool t)
    {
        isReplayEnabled = t;

        if (t)
        {
            GameObject.Find("ReplayButton").GetComponent<TweenAlpha>().PlayReverse();
        }
        else

            GameObject.Find("ReplayButton").GetComponent<TweenAlpha>().PlayForward();
    }
    public Sprite[] Shoes;
    public Sprite[] Thsirts;
    public bool isEndless;
    public GameObject player1_GoalKeeper;
    public GameObject player1_Player;
    public GameObject player2_GoalKeeper;
    public GameObject player2_Player;

    public GameObject player1_goal;
    public GameObject player2_goal;

    public GameObject ball;

    public GameObject menuButton;

    bool mIgnoreUp = false;
    bool mIsInput = false;
    bool mPress = false;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Initilation());

        if (PlayerPrefs.GetInt("isInTheCup") == 1) // Lig açıksa endless mod otomatik close yapıyor
        {
            isEndless = false;
            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().Set(false);
        }

       if (PlayerPrefs.GetInt("isTodayMatchAvailable") == 0)
       {
           inMatchCurrentTeamFlag.gameObject.SetActive(false);
           inMatchOpponentTeamFlag.gameObject.SetActive(false);
       }
    }

    public void TwoButton(bool tbv)
    {
        twoButtonControl = tbv;
    }

    public void Endless(bool value)
    {
        if (value)
        {
            isEndless = true;
            endScore = 10000;
        }
        else
        {
            isEndless = false;
            endScore = 5;
        }
    }
    // Update is called once per frame
    void Update()
    {
      
            if (Input.GetKeyDown(keyCode) && isPaused && isPlaying)
            {
                if (!finished && Time.timeScale != 0.2f)
                {
                    rasume();
                    GameObject.Find("PausePanel").GetComponent<TweenAlpha>().PlayReverse();
                    GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayReverse();
                    GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayReverse();
                    GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayReverse();
                    GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayReverse();
                    GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayReverse();
                }
            }
            else if (Input.GetKeyDown(keyCode) && !isPaused && isPlaying)
            {
                if (!finished && Time.timeScale != 0.2f)
                {
                    pause();
                    GameObject.Find("PausePanel").GetComponent<TweenAlpha>().PlayForward();
                    GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
                    GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();
                    GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayForward();

                    GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
                    GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();
                }
            }


            else if (Input.GetKeyDown(keyCode) && !onExitingDialog && !isPlaying)
            {
                onExitingDialog = true;
                GameObject.Find("ExitMenu").GetComponent<TweenAlpha>().PlayForward();
                StartCoroutine(exitCounter());
            }
            else if (Input.GetKeyDown(keyCode) && onExitingDialog && !isPlaying)
            {
                Debug.Log("Quit Called");
                Application.Quit();
            }



    }

    IEnumerator exitCounter()
    {
        GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(2)";
        yield return new WaitForSeconds(1);
        GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(1)";
        yield return new WaitForSeconds(1);
        GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(0)";
        GameObject.Find("ExitMenu").GetComponent<TweenAlpha>().PlayReverse();
        onExitingDialog = false;

    }

    public void OnPlayer1JumpPress()
    {
        OnP1JumpPress.Invoke();
    }

    public void OnPlayer1JumpRelease()
    {
        OnP1JumpRelease.Invoke();
    }

    public void OnPlayer1JumpPress1()
    {
        OnP1JumpPress1.Invoke();
    }

    public void OnPlayer1JumpRelease1()
    {
        OnP1JumpRelease1.Invoke();
    }

    void OnGameEndVoid()
    {
        OnGameEnd.Invoke();

    }

    void OnGameStartVoid()
    {
        OnGameStart.Invoke();
    }

    void OnNextRoundVoid()
    {
        OnNextRaund.Invoke();
    }

    public void OnPlayer2JumpPress()
    {
        OnP2JumpPress.Invoke();
    }

    public void OnPlayer2JumpRelease()
    {
        OnP2JumpRelease.Invoke();
    }

    public void OnPlayer2JumpPress1()
    {
        OnP2JumpPress1.Invoke();
    }

    public void OnPlayer2JumpRelease1()
    {
        OnP2JumpRelease1.Invoke();
    }

    public void PlayOnePlayer()
    {
        if (PlayerPrefs.GetInt("isInTheCup") == 0) { inMatchCurrentTeamFlag.gameObject.SetActive(false); inMatchOpponentTeamFlag.gameObject.SetActive(false); }
        gameMode = GameMode.OnePlayer;
        
        if (twoButtonControl)
        {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().from.x;
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().to.x;
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().leftAnchor.absolute);
            
        }
        else
        {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);
        }

        if (PlayerPrefs.GetInt("isInTheCup") == 1 && PlayerPrefs.GetInt("isTodayMatchAvailable") == 1) // Lig açıksa ve suan macı varsa inits 
        {
            inMatchCurrentTeamFlag.gameObject.SetActive(true);
            inMatchOpponentTeamFlag.gameObject.SetActive(true);

            inMatchCurrentTeamFlag.gameObject.GetComponent<UISprite>().spriteName = PlayerPrefs.GetString("currentCountryIOC");
            
            inMatchOpponentTeamFlag.gameObject.GetComponent<UISprite>().spriteName = UIRoot.gameObject.GetComponent<globalWorldCup>().currentMatchInfo[0];
        }

        else if (PlayerPrefs.GetInt("isTodayMatchAvailable") == 0)
        {
            inMatchCurrentTeamFlag.gameObject.SetActive(false);
            inMatchOpponentTeamFlag.gameObject.SetActive(false);
        }

        GoPlay();
    }

    public void PlayTwoPlayer()
    {
        gameMode = GameMode.TwoPlayers;

        if (twoButtonControl)
        {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayReverse();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);

            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = (GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x - 182.71f);
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().to.x;
        }
        else
        {

            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);

            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = (GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x - 182.71f);
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().to.x;
        }

        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }

    public void PlayCpuVsCpuPlayer()
    {
        gameMode = GameMode.CpuVsCpu;
        GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }

    public void pause()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void rasume()
    {
        isPaused = false;
        Time.timeScale = 1.5f;
    }

    public void GoPlay()
    {
        isOnMenu = false;
        isPlaying = true;
        finished = false;
        totalGoal = 0;
        SpawnObjects();

        StopCoroutine("Timer");
        StartCoroutine("Timer");
        OnGameStartVoid();
    }

    void Show5Add()
    {
        if (GameHandler.PluginContainer.GetComponent<AdmobHandler>().enabled)
        {
            OnFiveGoal.Invoke();
        }
    }

    public void GoMenu()
    {

        if (PlayerPrefs.GetInt("isInTheCup") == 1 && gameMode == GameMode.OnePlayer && PlayerPrefs.GetInt("isTodayMatchAvailable") == 1) // Lig açıksa cıkıs yaptıgı için -5 puan
        {
            UIRoot.gameObject.GetComponent<globalWorldCup>().sendScore(5, 0); // -5 puan send
            Debug.Log("-5 puan kırıldı");
            isPaused = false;
            isOnMenu = true;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            isPlaying = false;
            BlueScore = 0;
            RedScore = 0;
            totalGoal = 0;
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
            Time.timeScale = 1.5f;
            DestroyImmediate(player1_GoalKeeper);
            DestroyImmediate(player2_GoalKeeper);
            DestroyImmediate(player1_Player);
            DestroyImmediate(player2_Player);
            DestroyImmediate(player1_goal);
            DestroyImmediate(player2_goal);
            DestroyImmediate(ball);
            Rune.SetActive(false);
            StopCoroutine(Timer());
            OnGameEndVoid();
        }

        else
        {
            Debug.Log("normal cıkıs in game");
            isPaused = false;
            isOnMenu = true;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            isPlaying = false;
            BlueScore = 0;
            RedScore = 0;
            totalGoal = 0;
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
            Time.timeScale = 1.5f;
            DestroyImmediate(player1_GoalKeeper);
            DestroyImmediate(player2_GoalKeeper);
            DestroyImmediate(player1_Player);
            DestroyImmediate(player2_Player);
            DestroyImmediate(player1_goal);
            DestroyImmediate(player2_goal);
            DestroyImmediate(ball);
            Rune.SetActive(false);
            StopCoroutine(Timer());
            OnGameEndVoid();

        }
    }

    public void GoMenuEndGame() // oyun sonu turn menu fonksiyonu
    {
        Debug.Log("normal cıkıs game end");
        isPaused = false;
        isOnMenu = true;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        isPlaying = false;
        BlueScore = 0;
        RedScore = 0;
        totalGoal = 0;
        GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
        Time.timeScale = 1.5f;
        DestroyImmediate(player1_GoalKeeper);
        DestroyImmediate(player2_GoalKeeper);
        DestroyImmediate(player1_Player);
        DestroyImmediate(player2_Player);
        DestroyImmediate(player1_goal);
        DestroyImmediate(player2_goal);
        DestroyImmediate(ball);
        Rune.SetActive(false);
        StopCoroutine(Timer());
        OnGameEndVoid();
        
    }

    public void Goal(Player player)
    {
        if (player == Player.Red)
        {
            StartCoroutine(StopRecording());
            ballRedOrBlue = true;
            BlueScore++;
            totalGoal++;
            Time.timeScale = 0.2f;


            GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("BlueGoal").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("BlueGoal").GetComponent<TweenPosition>().Toggle();
            GameObject.Find("ReplayButton").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("ReplayButton").GetComponent<TweenPosition>().Toggle();
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
            if (BlueScore == endScore)
                StartCoroutine(GameEnd(BlueWins));
            GameHandler.Effect.PlayGoal();
            if (totalGoal % 7 == 0 && isEndless)
            {
                Show5Add();
            }

        }
        if (player == Player.Blue)
        {
            StartCoroutine(StopRecording());
            ballRedOrBlue = false;
            RedScore++;
            totalGoal++;
            Time.timeScale = 0.2f;

            GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("RedGoal").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("RedGoal").GetComponent<TweenPosition>().Toggle();
            GameObject.Find("ReplayButton").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("ReplayButton").GetComponent<TweenPosition>().Toggle();
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
            if (RedScore == endScore)
                StartCoroutine(GameEnd(RedWins));
            GameHandler.Effect.PlayGoal();
            if (totalGoal % 7 == 0 && isEndless)
            {
                Show5Add();
            }
        }
    }

    public void Out(Player player)
    {

        if (player == Player.Red)
        {
            ballRedOrBlue = true;
            GameObject.Find("Out").GetComponent<TweenPosition>().Toggle();

        }
        if (player == Player.Blue)
        {
            ballRedOrBlue = false;
            GameObject.Find("Out").GetComponent<TweenPosition>().Toggle();
        }
    }

    public void nextRound()
    {
        if (isPlaying)
        {
            if (!finished)
            {

                Time.timeScale = 1.5f;
                resetObjects();

                if (ballRedOrBlue)
                {
                    ball.transform.position = new Vector3(-2.3f, 4, 0);
                }
                if (!ballRedOrBlue)
                {
                    ball.transform.position = new Vector3(2.3f, 4, 0);
                }
                GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayReverse();
                OnNextRoundVoid();
            }
        }
    }

    public void Rematch()
    {

        if (PlayerPrefs.GetInt("isInTheCup") == 1 && gameMode == GameMode.OnePlayer && PlayerPrefs.GetInt("isTodayMatchAvailable") == 1) // Lig açıksa cıkıs yaptıgı için -5 puan
        {
            UIRoot.gameObject.GetComponent<globalWorldCup>().sendScore(5, 0); // -5 puan send
            Debug.Log("rematch yüzünden -5 puan kırıldı");
        }

        runeSpawnTime = 30;
        isPaused = false;
        isPlaying = true;
        finished = false;
        BlueScore = 0;
        RedScore = 0;
        GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
        Time.timeScale = 1.5f;
        resetObjects();
        ball.name = "Top";

        if (ballRedOrBlue)
        {
            ball.transform.position = new Vector3(-2.3f, 4, 0);
        }
        if (!ballRedOrBlue)
        {
            ball.transform.position = new Vector3(2.3f, 4, 0);
        }
        if (gameMode == GameMode.OnePlayer)
        {
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().pair = player1_Player;
            player1_Player.GetComponent<Jump>().pair = player1_GoalKeeper;         
            
        }

        if (gameMode == GameMode.CpuVsCpu)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().pair = player1_Player;
            player1_Player.GetComponent<Jump>().pair = player1_GoalKeeper;

            player2_Player.GetComponent<Jump>().isCpu = true;
            player2_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player2_GoalKeeper.GetComponent<Jump>().pair = player2_Player;
            player2_Player.GetComponent<Jump>().pair = player2_GoalKeeper;
        }


        
        OnGameStartVoid();
    }

    public void BlueWins()
    {
        GameHandler.Effect.PlayBlueWins();
        GameObject.Find("BlueWins").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Rematch").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Menu").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueGoal").GetComponent<TweenPosition>().tweenFactor = 1;
        GameObject.Find("ReplayButton").GetComponent<TweenPosition>().tweenFactor = 1;


        if (PlayerPrefs.GetInt("isInTheCup") == 1 && gameMode == GameMode.OnePlayer && PlayerPrefs.GetInt("isTodayMatchAvailable") == 1) // Lige katılmış zaten, verileri direk çekiyorum
        {
            UIRoot.gameObject.GetComponent<globalWorldCup>().sendScore(RedScore, BlueScore);
            Debug.Log("Score posted.");
        }
    }

    public void RedWins()
    {
        GameHandler.Effect.PlayRedWins();
        GameObject.Find("RedWins").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Rematch").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Menu").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("MenuButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedGoal").GetComponent<TweenPosition>().tweenFactor = 1;
        GameObject.Find("ReplayButton").GetComponent<TweenPosition>().tweenFactor = 1;


        if (PlayerPrefs.GetInt("isInTheCup") == 1 && gameMode == GameMode.OnePlayer && PlayerPrefs.GetInt("isTodayMatchAvailable") == 1) // Lige katılmış zaten, verileri direk çekiyorum
        {
            UIRoot.gameObject.GetComponent<globalWorldCup>().sendScore(RedScore, BlueScore);
            Debug.Log("Score posted.");
        }
    }

    IEnumerator Initilation()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("MenuButton").GetComponent<TweenPosition>().from.x = GameObject.Find("MenuButton").transform.localPosition.x;
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x = GameObject.Find("BlueJumpButton").transform.localPosition.x;
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton").transform.localPosition.x;

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("BlueJumpButton1").transform.localPosition.x;
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton1").transform.localPosition.x;
    }

    IEnumerator GameEnd(EventHandler go)
    {
        yield return new WaitForSeconds(0.3f);
        go.Invoke();
        OnGameEndVoid();
        finished = true;
    }

    IEnumerator StopRecording()
    {
        yield return new WaitForSeconds(1);
        Everyplay.StopRecording();
    }
    public void RedGoalGetBig()
    {
        if (player1_goal)
        {
            player1_goal.GetComponent<TweenScale>().PlayForward();
            StartCoroutine(RedGoalGetSmall());
            GameHandler.Effect.PlayBlueGoalExtend();
        }
    }
    IEnumerator RedGoalGetSmall()
    {
        yield return new WaitForSeconds(10);
        if (player1_goal)
        player1_goal.GetComponent<TweenScale>().PlayReverse();
    }
    public void BlueGoalGetBig()
    {
        if (player2_goal)
        {
            player2_goal.GetComponent<TweenScale>().PlayForward();
            StartCoroutine(BlueGoalGetSmall());
            GameHandler.Effect.PlayBlueGoalExtend();
        }
    }
    IEnumerator BlueGoalGetSmall()
    {
        yield return new WaitForSeconds(10);
        if (player2_goal)
        player2_goal.GetComponent<TweenScale>().PlayReverse();
    }
    void SpawnObjects()
    {
        ball = Instantiate(Resources.Load("PlayerPrefabs/Top")) as GameObject;
        ball.name = "Top";
        int randomShoes = Random.Range(0, Shoes.Length);
        int randomShoes2;
        int randomTshirts = Random.Range(0, Thsirts.Length);
        int randomTshirts2;

        if (randomOutfits)
        {
            if (randomShoes >= Shoes.Length - 1)
            {
                randomShoes2 = randomShoes - 1;
            }
            else if (randomShoes <= 0)
            {
                randomShoes2 = randomShoes + 1;
            }
            else
            {
                randomShoes2 = randomShoes + 1;
            }

            if (randomTshirts >= Thsirts.Length - 1)
            {
                randomTshirts2 = randomTshirts - 1;
            }
            else if (randomTshirts <= 0)
            {
                randomTshirts2 = randomTshirts + 1;
            }
            else
            {
                randomTshirts2 = randomTshirts + 1;
            }
        }
        else
        {
            randomShoes = RedCurrentShoes;
            randomTshirts = RedCurrentTshirt;
            randomShoes2 = BlueCurrentShoes;
            randomTshirts2 = BlueCurrentTshirt;
        }
        //Player1
        ///Player1_GoalKeeper
        player1_GoalKeeper = Instantiate(Resources.Load("PlayerPrefabs/Player1_GoalKeeper")) as GameObject;
        player1_GoalKeeper.GetComponent<Jump>().player = Player.Red;
        player1_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        if (twoButtonControl)
        {
            player1_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player1_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            player1_GoalKeeper.GetComponent<Jump>().OnDestroy();
            player1_GoalKeeper.GetComponent<Jump>().OnEnable();
        }
        ///Player1_player
        player1_Player = Instantiate(Resources.Load("PlayerPrefabs/Player1_player")) as GameObject;
        player1_Player.GetComponent<Jump>().player = Player.Red;
        player1_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        if (twoButtonControl)
        {
            player1_Player.GetComponent<Jump>().twoButtonControl = true;
            player1_Player.GetComponent<Jump>().isGoalKeeper = false;
            player1_Player.GetComponent<Jump>().OnDestroy();
            player1_Player.GetComponent<Jump>().OnEnable();
        }
        //Player2
        ///Player2_Goalkeeper
        player2_GoalKeeper = Instantiate(Resources.Load("PlayerPrefabs/Player2_GoalKeeper")) as GameObject;
        player2_GoalKeeper.GetComponent<Jump>().player = Player.Blue;
        player2_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        if (twoButtonControl)
        {
            player2_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player2_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            player2_GoalKeeper.GetComponent<Jump>().OnDestroy();
            player2_GoalKeeper.GetComponent<Jump>().OnEnable();
        }
        ///Player2_Player
        player2_Player = Instantiate(Resources.Load("PlayerPrefabs/Player2_player")) as GameObject;
        player2_Player.GetComponent<Jump>().player = Player.Blue;
        player2_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        if (twoButtonControl)
        {
            player2_Player.GetComponent<Jump>().twoButtonControl = true;
            player2_Player.GetComponent<Jump>().isGoalKeeper = false;
            player2_Player.GetComponent<Jump>().OnDestroy();
            player2_Player.GetComponent<Jump>().OnEnable();
        }

        player1_goal = Instantiate(Resources.Load("PlayerPrefabs/KaleRed")) as GameObject;
        player2_goal = Instantiate(Resources.Load("PlayerPrefabs/KaleBlue")) as GameObject;
        


        if (gameMode == GameMode.OnePlayer)
        {
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().pair = player1_Player;
            player1_Player.GetComponent<Jump>().pair = player1_GoalKeeper;
        }

        if (gameMode == GameMode.CpuVsCpu)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().pair = player1_Player;
            player1_Player.GetComponent<Jump>().pair = player1_GoalKeeper;

            player2_Player.GetComponent<Jump>().isCpu = true;
            player2_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player2_GoalKeeper.GetComponent<Jump>().pair = player2_Player;
            player2_Player.GetComponent<Jump>().pair = player2_GoalKeeper;
        }
    }

    void resetObjects()
    {
        int randomShoes = Random.Range(0, Shoes.Length);
        int randomShoes2;
        int randomTshirts = Random.Range(0, Thsirts.Length);
        int randomTshirts2;

        if (randomOutfits)
        {
            if (randomShoes >= Shoes.Length - 1)
            {
                randomShoes2 = randomShoes - 1;
            }
            else if (randomShoes <= 0)
            {
                randomShoes2 = randomShoes + 1;
            }
            else
            {
                randomShoes2 = randomShoes + 1;
            }

            if (randomTshirts >= Thsirts.Length - 1)
            {
                randomTshirts2 = randomTshirts - 1;
            }
            else if (randomTshirts <= 0)
            {
                randomTshirts2 = randomTshirts + 1;
            }
            else
            {
                randomTshirts2 = randomTshirts + 1;
            }
        }
        else
        {
            randomShoes = RedCurrentShoes;
            randomTshirts = RedCurrentTshirt;
            randomShoes2 = BlueCurrentShoes;
            randomTshirts2 = BlueCurrentTshirt;
        }

        player1_GoalKeeper.GetComponent<Jump>().resetPlayer();
        player1_Player.GetComponent<Jump>().resetPlayer();
        player2_GoalKeeper.GetComponent<Jump>().resetPlayer();
        player2_Player.GetComponent<Jump>().resetPlayer();


        player1_GoalKeeper.GetComponent<Jump>().player = Player.Red;
        player1_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        if (twoButtonControl)
        {
            player1_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player1_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            player1_GoalKeeper.GetComponent<Jump>().OnDestroy();
            player1_GoalKeeper.GetComponent<Jump>().OnEnable();
        }

        player1_Player.GetComponent<Jump>().player = Player.Red;
        player1_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
        player1_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        if (twoButtonControl)
        {
            player1_Player.GetComponent<Jump>().twoButtonControl = true;
            player1_Player.GetComponent<Jump>().isGoalKeeper = false;
            player1_Player.GetComponent<Jump>().OnDestroy();
            player1_Player.GetComponent<Jump>().OnEnable();
        }

        player2_GoalKeeper.GetComponent<Jump>().player = Player.Blue;
        player2_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        if (twoButtonControl)
        {
            player2_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player2_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            player2_GoalKeeper.GetComponent<Jump>().OnDestroy();
            player2_GoalKeeper.GetComponent<Jump>().OnEnable();
        }

        player2_Player.GetComponent<Jump>().player = Player.Blue;
        player2_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
        player2_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        if (twoButtonControl)
        {
            player2_Player.GetComponent<Jump>().twoButtonControl = true;
            player2_Player.GetComponent<Jump>().isGoalKeeper = false;
            player2_Player.GetComponent<Jump>().OnDestroy();
            player2_Player.GetComponent<Jump>().OnEnable();
        }
        ball.GetComponent<BallScript>().resetBall();
    }

    public void SetBlueShoes(int shoes)
    {
        BlueCurrentShoes = shoes;
    }

    public void SetBlueTshirts(int tshirt)
    {
        BlueCurrentTshirt = tshirt;
    }

    public void SetRedShoes(int shoes)
    {
        RedCurrentShoes = shoes;
    }

    public void SetRedTshirts(int tshirt)
    {
        RedCurrentTshirt = tshirt;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.5f);
        if (Time.timeScale != 0.2f)
        runeSpawnTime--;
       // GameHandler.RuneCounter(runeSpawnTime);
        if (runeSpawnTime == 0)
        {
            GameHandler.Effect.PlayRunespawn();
            runeSpawnTime = 30;
            Rune.SetActive(true);
        }
        StartCoroutine("Timer");
        if (!isPlaying)
        {
            runeSpawnTime = 30;
            StopCoroutine("Timer");
        }
        if (isOnMenu)
        {
            runeSpawnTime = 30;
            StopCoroutine("Timer");
        }
    }
}
