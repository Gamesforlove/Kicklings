using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConsoleBase : MonoBehaviour
{
    protected string status = "Ready";

    protected string lastResponse = "";
    public GUIStyle textStyle = new GUIStyle();
    protected Texture2D lastResponseTexture;

    protected Vector2 scrollPosition = Vector2.zero;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
    protected int buttonHeight = 60;
    protected int mainWindowWidth = Screen.width - 30;
    protected int mainWindowFullWidth = Screen.width;
#else
    protected int buttonHeight = 24;
    protected int mainWindowWidth = 500;
    protected int mainWindowFullWidth = 530;
#endif

    virtual protected void Awake()
    {
        textStyle.alignment = TextAnchor.UpperLeft;
        textStyle.wordWrap = true;
        textStyle.padding = new RectOffset(10, 10, 10, 10);
        textStyle.stretchHeight = true;
        textStyle.stretchWidth = false; 
    }

    protected bool Button(string label)
    {
        return GUILayout.Button(
            label, 
            GUILayout.MinHeight(buttonHeight), 
            GUILayout.MaxWidth(mainWindowWidth)
        );
    }

    protected void LabelAndTextField(string label, ref string text)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.MaxWidth(150));
        text = GUILayout.TextField(text);
        GUILayout.EndHorizontal();
    }

    protected bool IsHorizontalLayout()
    {
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        return Screen.orientation == ScreenOrientation.Landscape;
        #else
        return true;
        #endif
    }

    protected int TextWindowHeight
    {
        get
        {
            #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
            return IsHorizontalLayout() ? Screen.height : 85;
            #else
            return Screen.height;
            #endif
        }
    }


   
   
   

}
