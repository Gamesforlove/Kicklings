using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject Target;

    public float minX;
    public float maxX;

    void OnEnable()
    {
        GameHandler.GameController.OnGameEnd += GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart += GameController_OnGameStart;
        GameHandler.GameController.OnNextRound += GameController_OnNextRound;
    }

    void GameController_OnNextRound()
    {
        Target = GameHandler.Ball;
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Target == null)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 8, Time.deltaTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, 0, -10), Time.deltaTime);
        }

        if (Target != null)
        {
            //  Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Target.transform.position.y + 11, Time.deltaTime);
            Vector3 pos = Vector3.Lerp(Camera.main.transform.position, Target.transform.position / 5 + Vector3.back * 10, Time.deltaTime);
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            Camera.main.transform.position = pos;
        }

	
	}

    void GameController_OnGameEnd()
    {
        Target = null;
    }

    void GameController_OnGameStart()
    {
        Target = GameHandler.Ball;
    }
}
