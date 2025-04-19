using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCustomization : MonoBehaviour {


    public Transform ballContainer;
    public int currentBall;
    private Transform currentBallTransform;

    private void Start()
    {
        SetBall();

        PlayerPurchaseManager.Instance.onBoughtNewItem += () =>
        {
            if (currentBall != 0)
                SetBall();
        };
    }

    public void SetBall()
    {
        GameHandler.GameController.currentBall = currentBall;

        if (currentBallTransform != null)
            Destroy(currentBallTransform.gameObject);

        currentBallTransform = Instantiate(PlayerPurchaseManager.Instance.GetBoughtBalls()[currentBall].uiItem, transform).transform;

        currentBallTransform.localPosition = Vector2.zero;
    }

    public void NextBall()
    {
        currentBall++;
        if (currentBall > PlayerPurchaseManager.Instance.GetBoughtBalls().Count - 1)
        {
            currentBall = 0;
        }
        SetBall();
    }

    public void PreviousBall()
    {
        currentBall--;
        if (currentBall < 0)
        {
            currentBall = PlayerPurchaseManager.Instance.GetBoughtBalls().Count - 1;
        }
        SetBall();
    }
}
