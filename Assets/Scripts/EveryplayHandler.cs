using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class EveryplayHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StartCoroutine(InitCoroutine());
        
	}
	
	// Update is called once per frame
	void Update () {
        
	}



    void GameController_OnGameEnd()
    {

        if (GameHandler.GameController.isReplayEnabled) { 
        Everyplay.SetMetadata("score", GameHandler.GameController.RedScore.ToString() + "-" + GameHandler.GameController.BlueScore.ToString());
        Everyplay.SetMetadata("device", SystemInfo.deviceModel);
        StopRecordingEvery();}
    }

    void GameController_OnGameStart()
    {
        if (GameHandler.GameController.isReplayEnabled)
        {
            StartRecordingEvery();
        }
    }

    void GameController_OnNextRaund()
    {


        if (GameHandler.GameController.isReplayEnabled)
        {

            Everyplay.SetMetadata("score", GameHandler.GameController.RedScore.ToString() + "-" + GameHandler.GameController.BlueScore.ToString());
            Everyplay.SetMetadata("device", SystemInfo.deviceModel);
            RestartRecording();
        }
    }

    public void StopRecording()
    {
        if (GameHandler.GameController.isReplayEnabled)
        OpenEveryPlay();
    }

    public void OpenEveryPlay()
    {
        Everyplay.Show();
    }


    public bool showUploadStatus = true;
    private bool isRecording = false;
    private bool isPaused = false;
    private bool isRecordingFinished = true;
    private TextMesh uploadStatusLabel;
    private Texture2D previousThumbnail;

    void Awake()
    {
        if (enabled && showUploadStatus)
        {
            CreateUploadStatusLabel();
        }

        DontDestroyOnLoad(gameObject);
    }
    

    void Destroy()
    {
        if (uploadStatusLabel != null)
        {
            Everyplay.UploadDidStart -= UploadDidStart;
            Everyplay.UploadDidProgress -= UploadDidProgress;
            Everyplay.UploadDidComplete -= UploadDidComplete;
        }

        Everyplay.RecordingStarted -= RecordingStarted;
        Everyplay.RecordingStopped -= RecordingStopped;

        Everyplay.ThumbnailReadyAtFilePath -= ThumbnailReadyAtFilePath;
    }

    private void RecordingStarted()
    {
        isRecording = true;
        isPaused = false;
        isRecordingFinished = false;
    }

    private void RecordingStopped()
    {

        Debug.Log(GameHandler.GameController.RedScore.ToString() + "-" + GameHandler.GameController.BlueScore.ToString());
        Everyplay.SetMetadata("score", GameHandler.GameController.RedScore.ToString() + "-" + GameHandler.GameController.BlueScore.ToString());
        Everyplay.SetMetadata("device", SystemInfo.deviceModel);
        isRecording = false;
        isRecordingFinished = true;
    }


    public void RestartRecording()
    {
        if (isEnabled)
        {
            StartCoroutine(Reset());
        }
    }
    public bool isEnabled = true;

    public void Toggle()
    {
        isEnabled = !isEnabled;
    }

    public void StartRecordingEvery()
    {
        if (isEnabled)
        {
            if (Everyplay.IsRecordingSupported())
            {
                if (Everyplay.IsRecording())
                {
                    StartCoroutine(Reset());
                    return;
                }

                Everyplay.StartRecording();

            }
        }
    }

    public void ResumeRecord()
    {
        if (isEnabled)
        {
            if (Everyplay.IsPaused())
                Everyplay.ResumeRecording();
        }
    }
    public void PauseRecord()
    {
        if (isEnabled)
        {
            if (isRecording)
                Everyplay.PauseRecording();
        }
    }
    public void StopRecordingEvery()
    {
        if (isEnabled)
        {
            if (isRecording)
            {
                Everyplay.StopRecording();
            }
        }
    }

    public void Watch()
    {
        if (isEnabled)
        {
            Everyplay.PlayLastRecording();
        }
    }

    private void CreateUploadStatusLabel()
    {
        GameObject uploadStatusLabelObj = new GameObject("UploadStatus", typeof(TextMesh));

        if (uploadStatusLabelObj)
        {
            uploadStatusLabelObj.transform.parent = transform;
            uploadStatusLabel = uploadStatusLabelObj.GetComponent<TextMesh>();

            if (uploadStatusLabel != null)
            {
                uploadStatusLabel.anchor = TextAnchor.LowerLeft;
                uploadStatusLabel.alignment = TextAlignment.Left;
              //  uploadStatusLabel.text = "Not uploading";
            }
        }
    }

    private void UploadDidStart(int videoId)
    {
      //  uploadStatusLabel.text = "Upload " + videoId + " started.";
    }

    private void UploadDidProgress(int videoId, float progress)
    {
     //   uploadStatusLabel.text = "Upload " + videoId + " is " + Mathf.RoundToInt((float)progress * 100) + "% completed.";
    }

    private void UploadDidComplete(int videoId)
    {
      //  uploadStatusLabel.text = "Upload " + videoId + " completed.";

        StartCoroutine(ResetUploadStatusAfterDelay(2.0f));
    }

    private IEnumerator ResetUploadStatusAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
      //  uploadStatusLabel.text = "Not uploading";
    }

    private void ThumbnailReadyAtFilePath(string path)
    {
        // We are loading the thumbnail during the recording for demonstration purposes only.
        // Normally you should start the load after you have stopped the recording to make sure the rendering does not stutter.
        Everyplay.LoadThumbnailFromFilePath(path, ThumbnailSuccess, ThumbnailError);
    }

    private void ThumbnailSuccess(Texture2D texture)
    {
        if (texture != null)
        {
            previousThumbnail = texture;
        }
    }

    private void ThumbnailError(string error)
    {
     //   Debug.Log("Thumbnail loading failed: " + error);
    }

    IEnumerator Reset()
    {
        Everyplay.StopRecording();
        yield return new WaitForSeconds(2);
        Everyplay.StartRecording();
    }


    void OnHideUnity(bool unityIsHidden)
    {


    }



    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);

        yield return new WaitForSeconds(0.1f);
        if (uploadStatusLabel != null)
        {
            Everyplay.UploadDidStart += UploadDidStart;
            Everyplay.UploadDidProgress += UploadDidProgress;
            Everyplay.UploadDidComplete += UploadDidComplete;
        }

        yield return new WaitForSeconds(0.1f);
        Everyplay.RecordingStarted += RecordingStarted;
        Everyplay.RecordingStopped += RecordingStopped;

        Everyplay.ThumbnailReadyAtFilePath += ThumbnailReadyAtFilePath;

        yield return new WaitForSeconds(0.1f);
        GameHandler.GameController.OnGameEnd += GameController_OnGameEnd;
        GameHandler.GameController.OnGameStart += GameController_OnGameStart;
        GameHandler.GameController.OnNextRaund += GameController_OnNextRaund;
    }

}
