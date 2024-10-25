using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class globalWorldCup : MonoBehaviour
{
    // PAMPA Bu script calısmaya basladıgı an
    bool refreshClick = true;

    public GameObject joinToCupButton;
    public GameObject leaveToCupButton;
    public GameObject refreshButton;
    public GameObject top10List;
    public GameObject yourStatusBar;
    public GameObject countryStatsBar;
    public GameObject worldCupMainPanel;
    public GameObject chooseCountryPanel;
    public GameObject yourCountryButton;
    public GameObject currentMatchMenuWidget;
    public GameObject currentCountryFlag;
    public GameObject opponentCountryFlag;
    public GameObject currentCountryName;
    public GameObject opponentCountryName;
    public GameObject currentMatchTimeonMenu;
    public GameObject menuEndessToggleButton;
    public GameObject featuresWidget;
    public GameObject fullFixtureWidget;

    public GameObject[] countryButtons1;
    public GameObject[] countryButtons2;
    public GameObject[] countryButtons3;
    public GameObject[] countryButtons4;
    public GameObject[] countryButtons5;
    public GameObject[] countryButtons6;

    // Alfabetik sıraya göre ülkeler ve kısa isimlerinin listesi ->
    public List<string> countries = new List<string>();
    public List<string> countriesIOC = new List<string>();

    // bu 2 list ile ülke listesini ve kısaltmalarını alfabetik alabilirsin.

    // Daha sonra senden sadece 1 kez ülke kısaltması isticem "OYUNDA KULLANICI LIGE KAYDOLURKEN" CAGIRCAGIN FONKSIYON SADECE 1 KEZ -> joinLeague(string countryIOC);

    // Oyuncunun daha sonraki oyuna girişlerinde ise şu fonksiyonu 1 kez cagırman gerek -> getAllData();

    // KULLANACAGIN LISTLER VE FONKSİYONLAR emmek

    // Puan durumuna göre sıralanmış takım kısaltmaları ve puanlar listesi ->
    public List<string> scoreBoardCountryNames = new List<string>();
    public List<string> scoreBoardCountryScores = new List<string>();

    // Oyuncunun Bugünkü maçlarının saat sırasına göre listesi ->
    public List<string> todayFixtureMyTeam = new List<string>();
    public List<string> todayFixtureMyOpponents = new List<string>();
    public List<string> todayFixtureMyMatchTimes = new List<string>();

    // Listeler hariç 3 4 tanede fonksiyon kullanabilrisin onlarıda OnGUI içinde yazdım örnek hepsine

    //////////////

    // PRIVATE

    public List<string> currentMatchInfo = new List<string>();


    void OnGUI()
    {

        /*  
        if (GUI.Button(new Rect(100, 10, 200, 30), "Join Amk"))
            joinLeague("TUR"); // public void sendScore(int cpuScore, int yourScore) // Seçilmiş takım bilgisini kendi ekliyor içine
        // Score post
        if (GUI.Button(new Rect(10, 10, 200, 30), "Post Score"))
            sendScore(1, 5); // public void sendScore(int cpuScore, int yourScore) // Seçilmiş takım bilgisini kendi ekliyor içine
        
        // Adamın o anki maçlarının anlık bilgisini veren fonksiyonlar. Bunlar string döndürüyor sana çağır sadece yeter
        if (GUI.Button(new Rect(100, 100, 200, 30), "Get Current Opponent"))
        {
            // sen sadece cagır pampa o zamana serverda bakıp veriyor
            Debug.Log(getOpponentTeam());
        }

        if (GUI.Button(new Rect(100, 150, 200, 30), "Get Current Team"))
        {
            // sen sadece cagır pampa o zamana serverda bakıp veriyor
            Debug.Log(getCurrentTeam());
        }

        if (GUI.Button(new Rect(100, 200, 200, 30), "Get Current Match Time"))
        {
            // sen sadece cagır pampa o zamana serverda bakıp veriyor
            Debug.Log(getCurrentMatchTime());
        }
        */

    }

    void Start()
    {

        // her türlü cagır
        getAllCountries();
        getScoreBoard();
        StartCoroutine("widgetSwitcher");

        // ifs
        

        if (PlayerPrefs.GetInt("isInTheCup") == 1) // Lige katılmış zaten, verileri direk çekiyorum
        {
            TweenAlpha.Begin(joinToCupButton.gameObject, 0.25f, 0.2f);
            TweenAlpha.Begin(currentMatchMenuWidget.gameObject, 0.50f, 1f);
            
            joinToCupButton.gameObject.GetComponent<UIButton>().enabled = false;
            getAllData();
            Debug.Log("welcome again bro");

            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().enabled = false;
            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().Set(false);

            TweenAlpha.Begin(yourCountryButton.gameObject, 0.25f, 0.2f);
            yourCountryButton.gameObject.GetComponent<UIButton>().enabled = false;
        }

        else
        {
            TweenAlpha.Begin(leaveToCupButton.gameObject, 0.25f, 0.2f);
            leaveToCupButton.gameObject.GetComponent<UIButton>().enabled = false;

            TweenAlpha.Begin(refreshButton.gameObject, 0.25f, 0.2f);
            refreshButton.gameObject.GetComponent<UIButton>().enabled = false;

            TweenAlpha.Begin(currentMatchMenuWidget.gameObject, 0.25f, 0f);

            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().enabled = true;

            //TweenAlpha.Begin(yourCountryButton.gameObject, 0.25f, 0.2f);
            //yourCountryButton.gameObject.GetComponent<UIButton>().enabled = false;
            
        }
        
        
        //PlayerPrefs.DeleteAll();
    }

    // PRIVATE FUNCTIONS BEGIN //
    private void getMyTodayFixture()
    {
        WWWForm form = new WWWForm();

        WWW www = new WWW("http://nebugu.com/export.php?my_matches&team=" + PlayerPrefs.GetString("currentCountryIOC"));
        
        StartCoroutine(getMyTodayFixture(www));
        
    }

    private void getCurrentMatch()
    {
        WWWForm form = new WWWForm();

        WWW www = new WWW("http://www.nebugu.com/export.php?now_playing&team=" + PlayerPrefs.GetString("currentCountryIOC"));

        StartCoroutine(getCurrentMatch(www));

    }

    private void getAllCountries()
    {
        WWWForm form = new WWWForm();

        WWW www = new WWW("http://www.nebugu.com/export.php?countries");

        StartCoroutine(getAllCountries(www));

    }

    private void getScoreBoard()
    {
        WWW www = new WWW("http://www.nebugu.com/export.php?league");
        StartCoroutine(getScoreBoard(www));

    }

    // PRIVATE FUNCTIONS END //

    //************************//

    // PUBLIC FUNCTIONS BEGIN //


    public void chooseCountry(string koko)
    {
        PlayerPrefs.SetString("currentCountryIOC", koko);
        Debug.Log(koko);
        TweenAlpha.Begin(chooseCountryPanel, 0.2f, 0f);
    }

    public void joinToCup()
    {  
        if (PlayerPrefs.GetInt("isInTheCup") == 0) // bu telefondan ilk kez lige join olunuyor demek
        {
            Debug.Log("u r in bro");
            
            // bu fonk sadece cup a girişde calıscak
            // ülkeyi ayarladık zaten ülke seçtirdik
            getAllData();
            PlayerPrefs.SetInt("isInTheCup", 1);
            // join butonu disable 
            TweenAlpha.Begin(joinToCupButton.gameObject, 0.25f, 0.2f);
            joinToCupButton.gameObject.GetComponent<UIButton>().enabled = false;

            TweenAlpha.Begin(leaveToCupButton.gameObject, 0.25f, 1f);
            leaveToCupButton.gameObject.GetComponent<UIButton>().enabled = true;

            TweenAlpha.Begin(refreshButton.gameObject, 0.25f, 1f);
            refreshButton.gameObject.GetComponent<UIButton>().enabled = true;

            TweenAlpha.Begin(yourCountryButton.gameObject, 0.25f, 0.2f);
            yourCountryButton.gameObject.GetComponent<UIButton>().enabled = false;
            
            TweenAlpha.Begin(currentMatchMenuWidget.gameObject, 0.50f, 1f);

            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().enabled = false;
            menuEndessToggleButton.gameObject.GetComponent<UIToggle>().Set(false);

        }
    }

    
    public void leaveToCup() // kupadan çıkış
    {
        PlayerPrefs.SetInt("isInTheCup", 0);
        yourStatusBar.gameObject.GetComponent<UILabel>().text = "Choose Your Team !";
        // exit disable
        TweenAlpha.Begin(leaveToCupButton.gameObject, 0.25f, 0.2f);
        leaveToCupButton.gameObject.GetComponent<UIButton>().enabled = false;

        joinToCupButton.gameObject.GetComponent<UIButton>().enabled = true;
        TweenAlpha.Begin(joinToCupButton.gameObject, 0.25f, 1f);

        TweenAlpha.Begin(yourCountryButton.gameObject, 0.25f, 1f);
        yourCountryButton.gameObject.GetComponent<UIButton>().enabled = true;

        TweenAlpha.Begin(currentMatchMenuWidget.gameObject, 0.25f, 0f);

        menuEndessToggleButton.gameObject.GetComponent<UIToggle>().enabled = true;

    }

    public void refreshAllLists() // refresh
    {
        if (refreshClick)
        {
            // Refresh lists

            TweenAlpha.Begin(refreshButton.gameObject, 0.25f, 0.2f);
            refreshButton.gameObject.GetComponent<UIButton>().enabled = false;
            StartCoroutine("refreshFlagChanger");
            refreshClick = false;
        }
    }
    

    public void joinLeague(string countryIOC = "TUR")
    {
        PlayerPrefs.SetString("currentCountryIOC", countryIOC);

        WWW www = new WWW("http://www.nebugu.com/export.php?start_match");
        StartCoroutine(getSessionID(www));

        getCurrentMatch();
        getMyTodayFixture();
        getScoreBoard();
    }

    public void getAllData ()
    {
        WWW www = new WWW("http://www.nebugu.com/export.php?start_match");
        StartCoroutine(getSessionID(www));

        getCurrentMatch();
        getMyTodayFixture();
        getScoreBoard();

    }

    public void sendScore(int cpuScore, int yourScore)
    {

        WWWForm form = new WWWForm();
        form.AddField("team", PlayerPrefs.GetString("currentCountryIOC"));
        form.AddField("score", cpuScore + "-" + yourScore);
        form.AddField("sess_id", PlayerPrefs.GetString("sessionID"));
        string link = "http://www.nebugu.com/import.php?send_score";
        WWW w = new WWW(link, form);

        StartCoroutine(sendedScoreStatus(w));

    }

    public string getOpponentTeam()
    {
        return currentMatchInfo[0];

    }

    public string getCurrentTeam()
    {
        return currentMatchInfo[1];

    }

    public string getCurrentMatchTime()
    {
        return currentMatchInfo[2];

    }

    public void openWorldCupPanel()
    {
        TweenAlpha.Begin(worldCupMainPanel, 0.15f, 1f);
        if (PlayerPrefs.GetInt("firstTimeChoose") == 0)
        {
            chooseCountryPanelOpen();
            PlayerPrefs.SetInt("firstTimeChoose", 1);
        }
    }

    public void closeWorldCupPanel()
    {
        TweenAlpha.Begin(worldCupMainPanel, 0.15f, 0f);

    }

    public void chooseCountryPanelOpen()
    {

        TweenAlpha.Begin(chooseCountryPanel, 0.2f, 1f);

    }

    public void chooseCountryPanelClose()
    {
        TweenAlpha.Begin(chooseCountryPanel, 0.2f, 0f);

    }

    // PUBLIC FUNCTIONS END //

    IEnumerator widgetSwitcher()
    {
        yield return new WaitForSeconds(5f);
        TweenAlpha.Begin(featuresWidget.gameObject, 0.30f, 0f);
        TweenAlpha.Begin(fullFixtureWidget.gameObject, 0.30f, 1f);
        yield return new WaitForSeconds(5f);
        TweenAlpha.Begin(featuresWidget.gameObject, 0.30f, 1f);
        TweenAlpha.Begin(fullFixtureWidget.gameObject, 0.30f, 0f);
        StartCoroutine("widgetSwitcher");

    }

    IEnumerator getSessionID(WWW sessionID)
    {
        yield return sessionID;

        JsonData data = JsonMapper.ToObject(sessionID.text);

        Debug.Log(data[0]);

        PlayerPrefs.SetString("sessionID", data[0].ToString());
            
    }

    IEnumerator getCurrentMatch(WWW currentMatchName)
    {
        yield return currentMatchName;

        JsonData data = JsonMapper.ToObject(currentMatchName.text);

        if ((int)data[3] == 0) // Normal Maç Saati
        {
            currentMatchInfo.Clear();

            currentMatchInfo.Add(data[0].ToString()); // rakip takım
            currentMatchInfo.Add(data[1].ToString()); // kendi ülken
            currentMatchInfo.Add(data[2].ToString()); // maç tarihi ve saati

            //pastring matchTime += currentMatchInfo[2].Substring(10);
            PlayerPrefs.SetInt("isTodayMatchAvailable", 1);

            yourStatusBar.gameObject.GetComponent<UILabel>().text = PlayerPrefs.GetString("currentCountryIOC") + " Vs. " + currentMatchInfo[0] + "\n" + currentMatchInfo[2];
            currentCountryName.gameObject.GetComponent<UILabel>().text = PlayerPrefs.GetString("currentCountryIOC");
            opponentCountryName.gameObject.GetComponent<UILabel>().text = currentMatchInfo[0];
            currentMatchTimeonMenu.gameObject.GetComponent<UILabel>().text = currentMatchInfo[2];
            currentCountryFlag.gameObject.GetComponent<UISprite>().spriteName = PlayerPrefs.GetString("currentCountryIOC");
            opponentCountryFlag.gameObject.GetComponent<UISprite>().spriteName = currentMatchInfo[0];
        }

        if ((int)data[3] == 1) // Adamın o günlük maçları bitmiş ise, bir sonraki günün ilk maç saati
        {
            currentMatchInfo.Clear();

            currentMatchInfo.Add("You're next match is tomorrow Vs. " + data[0].ToString()); // rakip takım
            currentMatchInfo.Add(data[1].ToString()); // kendi ülken
            currentMatchInfo.Add("You have not more match today. Your next match is : " + data[2].ToString()); // maç tarihi ve saati

            yourStatusBar.gameObject.GetComponent<UILabel>().text = currentMatchInfo[0];
            PlayerPrefs.SetInt("isTodayMatchAvailable", 0);

            currentCountryName.gameObject.GetComponent<UILabel>().text = PlayerPrefs.GetString("currentCountryIOC");
            opponentCountryName.gameObject.GetComponent<UILabel>().text = data[0].ToString();
            currentMatchTimeonMenu.gameObject.GetComponent<UILabel>().text = currentMatchInfo[2];
            currentCountryFlag.gameObject.GetComponent<UISprite>().spriteName = PlayerPrefs.GetString("currentCountryIOC");
            opponentCountryFlag.gameObject.GetComponent<UISprite>().spriteName = data[0].ToString();

        }

        

    }

    IEnumerator refreshFlagChanger()
    {
        yield return new WaitForSeconds(10f);
        TweenAlpha.Begin(refreshButton.gameObject, 0.25f, 1f);
        refreshButton.gameObject.GetComponent<UIButton>().enabled = true;
        refreshClick = true;
    }


    IEnumerator getMyTodayFixture(WWW myWeeklyFixture)
    {
        yield return myWeeklyFixture;

        JsonData data = JsonMapper.ToObject(myWeeklyFixture.text);

        todayFixtureMyOpponents.Clear();
        todayFixtureMyTeam.Clear();
        todayFixtureMyMatchTimes.Clear();

        for (int i = 0; i < 7; i++)
        {
            todayFixtureMyOpponents.Add(data[i][0].ToString());
            todayFixtureMyTeam.Add(data[i][1].ToString());
            todayFixtureMyMatchTimes.Add(data[i][2].ToString());
        }

    }

    IEnumerator getScoreBoard(WWW scoreBoard)
    {
        yield return scoreBoard;

        JsonData data = JsonMapper.ToObject(scoreBoard.text);

        scoreBoardCountryNames.Clear();
        scoreBoardCountryScores.Clear();

        top10List.gameObject.GetComponent<UILabel>().text = "";
        fullFixtureWidget.transform.Find("Points").gameObject.GetComponent<UILabel>().text = "";
        fullFixtureWidget.transform.Find("Table").gameObject.GetComponent<UILabel>().text = "";

        for (int i = 0; i < 48; i++)
        {
            scoreBoardCountryNames.Add(data[i][0].ToString());
            scoreBoardCountryScores.Add(data[i][1].ToString());
            
        }

        for (int i = 0; i < 10; i++)
        {

            top10List.gameObject.GetComponent<UILabel>().text += (i + 1).ToString() + ". " + scoreBoardCountryNames[i] + "\n";
            
        }

        for (int i = 0; i < 11; i++)
        {

            fullFixtureWidget.transform.Find("Table").gameObject.GetComponent<UILabel>().text += (i + 1).ToString() + ". " + scoreBoardCountryNames[i];
            if (i != 10)
                fullFixtureWidget.transform.Find("Table").gameObject.GetComponent<UILabel>().text += "\n";

            fullFixtureWidget.transform.Find("Points").gameObject.GetComponent<UILabel>().text += scoreBoardCountryScores[i];
            if (i != 10)
                fullFixtureWidget.transform.Find("Points").gameObject.GetComponent<UILabel>().text += "\n";
        }


    }

    IEnumerator getAllCountries(WWW countryList)
    {
        yield return countryList;

        JsonData data = JsonMapper.ToObject(countryList.text);

        countries.Clear();
        countriesIOC.Clear();

        for (int i = 0; i < 48; i++)
        {
            countries.Add(data[i][0].ToString());
            countriesIOC.Add(data[i][1].ToString());
        }

        for (int i = 0; i < 8; i++)
        {
            countryButtons1[i].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons1[i].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons1[i].gameObject.name = countriesIOC[i];

        }

        for (int i = 8; i < 16; i++)
        {
            countryButtons2[i - 8].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i - 8].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons2[i - 8].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons2[i - 8].gameObject.name = countriesIOC[i];

        }

        for (int i = 16; i < 24; i++)
        {
            countryButtons3[i -16].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i -16].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons3[i -16].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons3[i -16].gameObject.name = countriesIOC[i];

        }

        for (int i = 24; i < 32; i++)
        {
            countryButtons4[i -24].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i -24].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons4[i -24].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons4[i -24].gameObject.name = countriesIOC[i];

        }

        for (int i = 32; i < 40; i++)
        {
            countryButtons5[i -32].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i -32].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons5[i -32].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons5[i -32].gameObject.name = countriesIOC[i];

        }

        for (int i = 40; i < 48; i++)
        {
            countryButtons6[i -40].transform.Find("CountryName").gameObject.GetComponent<UILabel>().text = countriesIOC[i];
            //countryButtons1[i -40].transform.FindChild("CountryName").gameObject.name = countriesIOC[i];
            countryButtons6[i -40].transform.Find("Sprite").gameObject.GetComponent<UISprite>().spriteName = countriesIOC[i];
            countryButtons6[i -40].gameObject.name = countriesIOC[i];

        }


    }

    IEnumerator sendedScoreStatus(WWW ScoreStatus)
    {
        yield return ScoreStatus;

        JsonData data = JsonMapper.ToObject(ScoreStatus.text);

        Debug.Log(data[0].ToString());      
    }


}
