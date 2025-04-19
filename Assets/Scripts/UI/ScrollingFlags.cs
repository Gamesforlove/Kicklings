using System.Collections.Generic;
using UnityEngine;

public class ScrollingFlags : MonoBehaviour
{
    [SerializeField] private RectTransform[] flags;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float flagWidth;
    [SerializeField] private float xOffset;



    void Update()
    {
        for (int i = 0; i < flags.Length; i++) {
            flags[i].anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);

            // If the flag has scrolled completely off screen, reset its position
            if (flags[i].anchoredPosition.x <= - flagWidth + xOffset) {
                float rightmostFlagX = GetRightmostFlagPosition();
                flags[i].anchoredPosition = new Vector2(rightmostFlagX + flagWidth, flags[i].anchoredPosition.y);
            }
        }
    }

    private float GetRightmostFlagPosition()
    {
        float rightmostX = flags[0].anchoredPosition.x;
        foreach (RectTransform flag in flags) {
            if (flag.anchoredPosition.x > rightmostX) {
                rightmostX = flag.anchoredPosition.x;
            }
        }
        return rightmostX;
    }
}
