using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour {
    public string spashScreenMoviePath;
    // Use this for initialization
    void Awake () {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.PlayFullScreenMovie(spashScreenMoviePath, Color.black, FullScreenMovieControlMode.Hidden);
#endif
    }

}
