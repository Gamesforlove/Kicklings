using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void EventHandler();
public class GameController : MonoBehaviour {

    public static GameController Instance { get; private set; }

    #region VARIABLES
    public bool twoButtonControl;
    public GameObject UIRoot;
    public GameObject inMatchCurrentTeamFlag;
    public GameObject inMatchOpponentTeamFlag;

    public const int player1Layer = 8;
    public const int player2Layer = 9;
    public const int player1GoalkeeperLayer = 10;
    public const int player2GoalkeeperLayer = 11;

    public int BlueScore;
    public int RedScore;
    public GameObject Rune;
    public GameObject menuEndessToggleButton;
    public int endScore;
    public int runeSpawnTime = 30;
    public Text scoreToWinDisplay;

    public bool randomOutfits;

    public int currentBall;

    public int BlueCurrentTshirt;
    public int BlueCurrentShoes;
    public int BlueCurrentSkin;

    public int RedCurrentTshirt;
    public int RedCurrentShoes;
    public int RedCurrentSkin;

    public GameMode gameMode;
    public bool finished;
    bool ballRedOrBlue;
    public int totalGoal;
    public bool isPlaying;
    public bool isPaused;
    public bool isOnMenu;
    public bool onExitingDialog;
    KeyCode keyCode = KeyCode.Escape;

    public GameObject uiDefaultBlueSkin;
    public GameObject uiDefaultRedSkin;
    public Sprite[] Shoes;
    public Sprite[] Thsirts;
    public SkinItem[] skins;
    public BallItem[] balls;

    public Transform redPlayerSpawn;
    public Transform redGoalkeeperPlayerSpawn;
    public Transform bluePlayerSpawn;
    public Transform blueGoalkeeperPlayerSpawn;

    public bool isEndless;
    public GameObject player1_GoalKeeper;
    public GameObject player1_Player;
    public GameObject player2_GoalKeeper;
    public GameObject player2_Player;

    public GameObject player1_goal;
    public GameObject player2_goal;

    public GameObject ball;

    public GameObject menuButton;

    public Transform platform;
    public Transform groundCollider;

    private bool playersRedOut;
    private bool playersRedGoalkeeperOut;
    private bool playersBlueOut;
    private bool playersBlueGoalkeeperOut;
    private bool roundEnded;

    private float slowMotionTime = 0.3f;

    bool mIgnoreUp = false;
    bool mIsInput = false;
    bool mPress = false;

    [SerializeField] private GameObject guidePanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Text endScoreNotification;
    // Use this for initialization
    #endregion

    #region ENUMS
    public enum GameMode
    {
        OnePlayer, TwoPlayers, CpuVsCpu, OnePlayerPlatform, TwoPlayerPlatform, FourPlayer
    }

    public enum Player
    {
        Red, Blue
    }
    #endregion

    #region EVENTS
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
    public event EventHandler OnNextRound;

    public event EventHandler OnFiveGoal;


    #endregion


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(Initialization());

       if (PlayerPrefs.GetInt("isTodayMatchAvailable") == 0)
       {
           inMatchCurrentTeamFlag.gameObject.SetActive(false);
           inMatchOpponentTeamFlag.gameObject.SetActive(false);
       }
    }

    #region GAME SETTING
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
            UpdateEndScoreDisplay();
        }
        else
        {
            isEndless = false;
            UpdateEndScoreDisplay();
        }
    }
    #endregion


    public void IncreaseEndScore()
    {
        Debug.Log("End Score Increased");
        endScore++;
        UpdateEndScoreDisplay();
    }
    public void DecreaseEndScore()
    {
        Debug.Log("End Score Decreased");
        endScore = Mathf.Max(1, endScore - 1);
        UpdateEndScoreDisplay();
    }
    private void UpdateEndScoreDisplay()
    {
        scoreToWinDisplay.text = endScore.ToString();
        endScoreNotification.text = "First to " + endScore + " wins!";
    }

    public void ToggleGuidePanel()
    {
        guidePanel.SetActive(!guidePanel.activeSelf);
        if (guidePanel.activeSelf) creditsPanel.SetActive(false);
    }

    public void ToggleCreditsPanel()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);
        if (creditsPanel.activeSelf) guidePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(keyCode) && isPaused && isPlaying)
        {
            if (!finished && Time.timeScale != 0.2f)
            {
                Resume();
                /*GameObject.Find("PausePanel").GetComponent<TweenAlpha>().PlayReverse();
                GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayReverse();*/
            }
        }
        else if (Input.GetKeyDown(keyCode) && !isPaused && isPlaying)
        {
            if (!finished && Time.timeScale != 0.2f)
            {
                Pause();
                /*GameObject.Find("PausePanel").GetComponent<TweenAlpha>().PlayForward();
                GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
                GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();
                GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayForward();
                GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayForward();

                GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
                GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();*/
            }
        }


        else if (Input.GetKeyDown(keyCode) && !onExitingDialog && !isPlaying)
        {
            onExitingDialog = true;
            //GameObject.Find("ExitMenu").GetComponent<TweenAlpha>().PlayForward();
            StartCoroutine(ExitCounter());
        }
        else if (Input.GetKeyDown(keyCode) && onExitingDialog && !isPlaying)
        {
            Debug.Log("Quit Called");
            Application.Quit();
        }
    }

    IEnumerator ExitCounter()
    {
        /*GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(2)";
        yield return new WaitForSeconds(1);
        GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(1)";
        GameObject.Find("ExitLabel").GetComponent<UILabel>().text = " Press Back Again \n(0)";
        GameObject.Find("ExitMenu").GetComponent<TweenAlpha>().PlayReverse();*/
        yield return new WaitForSeconds(1);
        onExitingDialog = false;

    }

    #region EVENT INVOCATION
    public void OnPlayer1Jump1Press() => OnP1JumpPress.Invoke();
    public void OnPlayer1Jump1Release() => OnP1JumpRelease.Invoke();
    public void OnPlayer1Jump1Press1() => OnP1JumpPress1.Invoke();
    public void OnPlayer1Jump1Release1() => OnP1JumpRelease1.Invoke();
    void OnGameEndVoid() => OnGameEnd.Invoke();
    void OnGameStartVoid() => OnGameStart.Invoke();
    void OnNextRoundVoid() => OnNextRound.Invoke();
    public void OnPlayer2Jump1Press() => OnP2JumpPress.Invoke();
    public void OnPlayer2Jump1Release() => OnP2JumpRelease.Invoke();
    public void OnPlayer2Jump1Press1() => OnP2JumpPress1.Invoke();
    public void OnPlayer2Jump1Release1() => OnP2JumpRelease1.Invoke();
    #endregion

    #region PLAYMODE SELECTION
    public void PlayOnePlayer()
    {
        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        gameMode = GameMode.OnePlayer;

        //GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

        if (twoButtonControl)
        {
            /*GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();

            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().from.x;
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().to.x;
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().leftAnchor.absolute);
            */

        }
        else
        {
            /*GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);*/
        }
        //ShowFourPlayerButtons(false);
        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }
    /*public void PlayTwoPlayer()
    {
        gameMode = GameMode.TwoPlayers;

        GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
        GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = (GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x - 182.71f);
        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().to.x;

        if (twoButtonControl)
        {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
        }
        else
        {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();
        }

        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);
        ShowFourPlayerButtons(false);
        GoPlay();
    }*/
    /*public void PlayCpuVsCpuPlayer()
    {
        gameMode = GameMode.CpuVsCpu;
        GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);
        ShowFourPlayerButtons(false);
        GoPlay();
    }*/
    /*public void PlayFourPlayer()
    {
        gameMode = GameMode.FourPlayer;

        ShowFourPlayerButtons(true);

        GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

        GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
        GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = (GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x - 182.71f);
        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().to.x;

        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }*/

    /*public void PlayOnePlayerPlatform()
    {
        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        gameMode = GameMode.OnePlayerPlatform;

        if (twoButtonControl) {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().from.x;
            GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().to.x;
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(0f, -GameObject.Find("BlueJumpButton").GetComponent<UISprite>().leftAnchor.absolute);

        } else {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);
        }
        ShowFourPlayerButtons(false);
        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }*/
    /*public void PlayTwoPlayerPlatform()
    {
        gameMode = GameMode.TwoPlayerPlatform;

        if (twoButtonControl) {
            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayReverse();
        } else {

            GameObject.Find("BlueJumpButton1").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton1").GetComponent<TweenAlpha>().PlayForward();

            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().leftAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().rightAnchor.absolute);
            GameObject.Find("BlueJumpButton1").GetComponent<UISprite>().rightAnchor.Set(1f, -GameObject.Find("RedJumpButton1").GetComponent<UISprite>().leftAnchor.absolute);
        }

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = (GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x - 182.71f);
        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().to.x = GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().to.x;
        ShowFourPlayerButtons(false);
        inMatchCurrentTeamFlag.gameObject.SetActive(false);
        inMatchOpponentTeamFlag.gameObject.SetActive(false);

        GoPlay();
    }*/
    /*public void ShowFourPlayerButtons(bool state)
    {
        if (state) {
            GameObject.Find("BlueJumpButton2").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("RedJumpButton2").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("ReturnToMenuButton").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("MenuButton4Players").GetComponent<TweenAlpha>().PlayReverse();
        } else {
            GameObject.Find("BlueJumpButton2").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("RedJumpButton2").GetComponent<TweenAlpha>().PlayForward();
            GameObject.Find("ReturnToMenuButton").GetComponent<TweenAlpha>().PlayReverse();
            GameObject.Find("MenuButton4Players").GetComponent<TweenAlpha>().PlayForward();
        }
    }*/
    #endregion

    #region SCENE CONTROL
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1.5f;
    }

    public void GoPlay()
    {
        Debug.Log("Go To Play!");
        isOnMenu = false;
        isPlaying = true;
        finished = false;
        totalGoal = 0;
        SpawnObjects();

        StopCoroutine("Timer");
        StartCoroutine("Timer");
        OnGameStartVoid();
    }

    public void GoMenu()
    {
        Debug.Log("Return To The Menu!");
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

    public void GoMenuEndGame()
    {
        Debug.Log("End Game!");
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
    #endregion

    #region GAME EVENT
    public void Goal(Player player)
    {
        if (player == Player.Red) {
            ballRedOrBlue = true;
            BlueScore++;
            totalGoal++;
            Time.timeScale = 0.2f;


            /*GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("BlueGoal").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("BlueGoal").GetComponent<TweenPosition>().Toggle();*/
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";

            if (BlueScore == endScore) StartCoroutine(GameEnd(BlueWins));

            GameHandler.Effect.PlayGoal();
            BGAnimatorController.Instance.Goal();

            StartCoroutine(SlowMotionCourutine());
        }

        if (player == Player.Blue) {
            ballRedOrBlue = false;
            RedScore++;
            totalGoal++;
            Time.timeScale = 0.2f;

            /*GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayForward();
            GameObject.Find("RedGoal").GetComponent<TweenPosition>().ResetToBeginning();
            GameObject.Find("RedGoal").GetComponent<TweenPosition>().Toggle();*/
            GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";

            if (RedScore == endScore)  StartCoroutine(GameEnd(RedWins));

            GameHandler.Effect.PlayGoal();
            BGAnimatorController.Instance.Goal();

            StartCoroutine(SlowMotionCourutine());
        }
    }

    public void Out(Player player)
    {
        if (player == Player.Red) {
            ballRedOrBlue = true;
            //GameObject.Find("Out").GetComponent<TweenPosition>().Toggle();
        }

        if (player == Player.Blue) {
            ballRedOrBlue = false;
            //GameObject.Find("Out").GetComponent<TweenPosition>().Toggle();
        }
    }

    public void PlayerOut(GameObject player)
    {
        if (player == player1_Player) playersRedOut = true;
        if (player == player2_Player) playersBlueOut = true;
        if (player == player1_GoalKeeper) playersRedGoalkeeperOut = true;
        if (player == player2_GoalKeeper) playersBlueGoalkeeperOut = true;

        if (!roundEnded) {
            if (playersRedOut && playersRedGoalkeeperOut) {
                Goal(Player.Red);
                roundEnded = true;
            }

            if (playersBlueOut && playersBlueGoalkeeperOut) {
                Goal(Player.Blue);
                roundEnded = true;
            }
        }
    }

    public void RedGoalGetBig()
    {
        if (player1_goal) {
            //player1_goal.GetComponent<TweenScale>().PlayForward();
            StartCoroutine(RedGoalGetSmall());
            GameHandler.Effect.PlayBlueGoalExtend();
        }
    }
    IEnumerator RedGoalGetSmall()
    {
        yield return new WaitForSeconds(10);
        //if (player1_goal)
            //player1_goal.GetComponent<TweenScale>().PlayReverse();
    }

    public void BlueGoalGetBig()
    {
        if (player2_goal) {
            //player2_goal.GetComponent<TweenScale>().PlayForward();
            StartCoroutine(BlueGoalGetSmall());
            GameHandler.Effect.PlayBlueGoalExtend();
        }
    }
    IEnumerator BlueGoalGetSmall()
    {
        yield return new WaitForSeconds(10);
        //if (player2_goal)
            //player2_goal.GetComponent<TweenScale>().PlayReverse();
    }

    public void Rematch()
    {
        //UICamera.selectedObject = null;
        runeSpawnTime = 30;
        isPaused = false;
        isPlaying = true;
        finished = false;
        BlueScore = 0;
        RedScore = 0;
        GameHandler.Score = "[ff0005]" + RedScore + "[-]-[00ffff]" + BlueScore + "[-]";
        Time.timeScale = 1.5f;
        ResetObjects();
        ball.name = "Top";

        if (ballRedOrBlue) {
            ball.transform.position = new Vector3(-2.3f, 4, 0);
        }
        if (!ballRedOrBlue) {
            ball.transform.position = new Vector3(2.3f, 4, 0);
        }
        if (gameMode == GameMode.OnePlayer) {
            player1_Player.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().isCpu = true;
            player1_GoalKeeper.GetComponent<Jump>().pair = player1_Player;
            player1_Player.GetComponent<Jump>().pair = player1_GoalKeeper;
        }

        if (gameMode == GameMode.CpuVsCpu) {
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
        /*GameObject.Find("BlueWins").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Rematch").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Menu").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueGoal").GetComponent<TweenPosition>().tweenFactor = 1;*/
    }

    public void RedWins()
    {
        GameHandler.Effect.PlayRedWins();
        /*GameObject.Find("RedWins").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Rematch").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("Menu").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().PlayForward();

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayForward();
        GameObject.Find("RedGoal").GetComponent<TweenPosition>().tweenFactor = 1;*/
    }

    #endregion

    IEnumerator SlowMotionCourutine()
    {
        yield return new WaitForSeconds(slowMotionTime); 
        NextRound();
    }

    public void NextRound()
    {
        if (isPlaying)
        {
            if (!finished)
            {
                Time.timeScale = 1.5f;
                ResetObjects();

                if (ballRedOrBlue) ball.transform.position = new Vector3(-2.3f, 4, 0);
                if (!ballRedOrBlue) ball.transform.position = new Vector3(2.3f, 4, 0);

                /*GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().PlayReverse();
                GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().PlayReverse();*/
                OnNextRoundVoid();
            }
        }
    }


    IEnumerator Initialization()
    {
        yield return new WaitForSeconds(1);
        /*GameObject.Find("ReturnToMenuButton").GetComponent<TweenPosition>().from.x = GameObject.Find("ReturnToMenuButton").transform.localPosition.x;
        GameObject.Find("MenuButton4Players").GetComponent<TweenPosition>().from.y = GameObject.Find("MenuButton4Players").transform.localPosition.y;
        GameObject.Find("BlueJumpButton").GetComponent<TweenPosition>().from.x = GameObject.Find("BlueJumpButton").transform.localPosition.x;
        GameObject.Find("RedJumpButton").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton").transform.localPosition.x;

        GameObject.Find("BlueJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("BlueJumpButton1").transform.localPosition.x;
        GameObject.Find("RedJumpButton1").GetComponent<TweenPosition>().from.x = GameObject.Find("RedJumpButton1").transform.localPosition.x;*/
    }

    IEnumerator GameEnd(EventHandler go)
    {
        yield return new WaitForSeconds(0.3f);
        go.Invoke();
        OnGameEndVoid();
        finished = true;
    }

    

    void SpawnObjects()
    {
        //UICamera.selectedObject = null;

        if (gameMode != GameMode.OnePlayerPlatform && gameMode != GameMode.TwoPlayerPlatform)
        {
            ball = Instantiate(PlayerPurchaseManager.Instance.GetBoughtBalls()[currentBall].ballPrefab);
            ball.name = "Top";
            platform.gameObject.SetActive(false);
            groundCollider.gameObject.SetActive(true);
        }
        else
        {
            platform.gameObject.SetActive(true);
            groundCollider.gameObject.SetActive(false);
        }

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
        ///
        if (RedCurrentSkin == 0)
        {
            player1_GoalKeeper = Instantiate(Resources.Load("Prefabs/PlayerPrefabs/Player1_Default_GoalKeeper")) as GameObject;
            player1_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        }
        else
        {
            player1_GoalKeeper = Instantiate(PlayerPurchaseManager.Instance.GetBoughtSkins()[RedCurrentSkin].playerGoalkeeperPrefab) as GameObject;
        }

        player1_GoalKeeper.GetComponent<Jump>().isGaolKeeperSkin = true;
        player1_GoalKeeper.GetComponent<Jump>().player = Player.Red;

        player1_GoalKeeper.transform.position = redGoalkeeperPlayerSpawn.position;
        player1_GoalKeeper.SetLayer(player1GoalkeeperLayer, true);
        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player1_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player1_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
        }

        ///Player1_player
        if (RedCurrentSkin == 0)
        {
            player1_Player = Instantiate(Resources.Load("Prefabs/PlayerPrefabs/Player1_Default_player")) as GameObject;
            player1_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        }
        else
        {
            player1_Player = Instantiate(PlayerPurchaseManager.Instance.GetBoughtSkins()[RedCurrentSkin].playerPrefab) as GameObject;
        }

        player1_Player.GetComponent<Jump>().player = Player.Red;

        player1_Player.transform.position = redPlayerSpawn.position;
        player1_Player.SetLayer(player1Layer, true);
        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player1_Player.GetComponent<Jump>().twoButtonControl = true;
            player1_Player.GetComponent<Jump>().isGoalKeeper = false;
        }

        //Player2
        ///Player2_Goalkeeper

        if (BlueCurrentSkin == 0)
        {
            player2_GoalKeeper = Instantiate(Resources.Load("Prefabs/PlayerPrefabs/Player2_Default_GoalKeeper")) as GameObject;
            player2_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        }
        else
        {
            player2_GoalKeeper = Instantiate(PlayerPurchaseManager.Instance.GetBoughtSkins()[BlueCurrentSkin].playerGoalkeeperPrefab) as GameObject;
        }

        player1_GoalKeeper.GetComponent<Jump>().isGaolKeeperSkin = true;
        player2_GoalKeeper.GetComponent<Jump>().player = Player.Blue;

        player2_GoalKeeper.transform.position = blueGoalkeeperPlayerSpawn.position;
        player2_GoalKeeper.SetLayer(player2GoalkeeperLayer, true);
        if (twoButtonControl)
        {
            player2_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player2_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
        }
        ///Player2_Player

        if (BlueCurrentSkin == 0)
        {
            player2_Player = Instantiate(Resources.Load("Prefabs/PlayerPrefabs/Player2_Default_player")) as GameObject;
            player2_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        }
        else
        {
            player2_Player = Instantiate(PlayerPurchaseManager.Instance.GetBoughtSkins()[BlueCurrentSkin].playerPrefab) as GameObject;
        }

        player2_Player.GetComponent<Jump>().player = Player.Blue;
        player2_Player.transform.position = bluePlayerSpawn.position;
        player2_Player.SetLayer(player2Layer, true);

        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player2_Player.GetComponent<Jump>().twoButtonControl = true;
            player2_Player.GetComponent<Jump>().isGoalKeeper = false;
        }

        if (gameMode != GameMode.OnePlayerPlatform && gameMode != GameMode.TwoPlayerPlatform)
        {
            player1_goal = Instantiate(Resources.Load("Prefabs/KaleRed")) as GameObject;
            player2_goal = Instantiate(Resources.Load("Prefabs/KaleBlue")) as GameObject;
        }

        if (gameMode == GameMode.OnePlayer || gameMode == GameMode.OnePlayerPlatform)
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

    void ResetObjects()
    {
        //UICamera.selectedObject = null;
        playersRedOut = false;
        playersRedGoalkeeperOut = false;
        playersBlueOut = false;
        playersBlueGoalkeeperOut = false;
        roundEnded = false;

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

        player1_GoalKeeper.GetComponent<Jump>().isGaolKeeperSkin = true;
        player1_GoalKeeper.GetComponent<Jump>().player = Player.Red;
        if (RedCurrentSkin == 0)
        {
            player1_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        }

        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player1_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player1_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            //player1_GoalKeeper.GetComponent<Jump>().OnDestroy();
            //player1_GoalKeeper.GetComponent<Jump>().OnEnable();
        }

        player1_Player.GetComponent<Jump>().player = Player.Red;
        if (RedCurrentSkin == 0)
        {
            player1_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes];
            player1_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts];
        }
        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player1_Player.GetComponent<Jump>().twoButtonControl = true;
            player1_Player.GetComponent<Jump>().isGoalKeeper = false;
            //player1_Player.GetComponent<Jump>().OnDestroy();
            //player1_Player.GetComponent<Jump>().OnEnable();
        }
        player2_GoalKeeper.GetComponent<Jump>().isGaolKeeperSkin = true;
        player2_GoalKeeper.GetComponent<Jump>().player = Player.Blue;
        if (BlueCurrentSkin == 0)
        {
            player2_GoalKeeper.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_GoalKeeper.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_GoalKeeper.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        }
        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player2_GoalKeeper.GetComponent<Jump>().twoButtonControl = true;
            player2_GoalKeeper.GetComponent<Jump>().isGoalKeeper = true;
            //player2_GoalKeeper.GetComponent<Jump>().OnDestroy();
            //player2_GoalKeeper.GetComponent<Jump>().OnEnable();
        }

        player2_Player.GetComponent<Jump>().player = Player.Blue;
        if (BlueCurrentSkin == 0)
        {
            player2_Player.GetComponent<Jump>().Shoe1.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_Player.GetComponent<Jump>().Shoe2.GetComponent<SpriteRenderer>().sprite = Shoes[randomShoes2];
            player2_Player.GetComponent<Jump>().Tshirt.GetComponent<SpriteRenderer>().sprite = Thsirts[randomTshirts2];
        }
        if (twoButtonControl || gameMode == GameMode.FourPlayer)
        {
            player2_Player.GetComponent<Jump>().twoButtonControl = true;
            player2_Player.GetComponent<Jump>().isGoalKeeper = false;
            //player2_Player.GetComponent<Jump>().OnDestroy();
            //player2_Player.GetComponent<Jump>().OnEnable();
        }
        if (gameMode != GameMode.OnePlayerPlatform && gameMode != GameMode.TwoPlayerPlatform)
            ball.GetComponent<BallScript>().Reset();
    }

    #region SET OUTFITS
    public void SetBlueShoes(int shoes) => BlueCurrentShoes = shoes;
    public void SetBlueTshirts(int tshirt) => BlueCurrentTshirt = tshirt;
    public void SetBlueSkin(int intSkin) => BlueCurrentSkin = intSkin;

    public void SetRedShoes(int shoes) => RedCurrentShoes = shoes;
    public void SetRedTshirts(int tshirt) => RedCurrentTshirt = tshirt;
    public void SetRedSkin(int intSkin) => RedCurrentSkin = intSkin;
    #endregion

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.5f);

        if (Time.timeScale != 0.2f) runeSpawnTime--;

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
