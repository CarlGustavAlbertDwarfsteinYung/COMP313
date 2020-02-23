using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarProgress : MonoBehaviour
{
    private RectTransform _emptyBar;
    private RectTransform _fullBar;
    private static LoadingBarProgress _instance;

    /// <summary>
    /// Set the current bar progress (write-only)
    /// </summary>
    public static float Progress
    {
        set => _instance.UpdateProgress(value);
    }

    /// <summary>
    /// Cache a reference to the bar component RectTransform 
    /// </summary>
    private void Start()
    {
        _emptyBar = transform.Find("Empty").GetComponent<RectTransform>();
        _fullBar = _emptyBar.Find("Full").GetComponent<RectTransform>();

        // Cache the static instance of this variable
        _instance = this;
    }

    /// <summary>
    /// Update the current bar progress
    /// </summary>
    /// <param name="progress">Current progress percentage on a scale 0:1</param>
    public void UpdateProgress(float progress)
    {
        // Get the current size of the bar
        var emptySize = _emptyBar.sizeDelta;
        var fullSize = _fullBar.sizeDelta;

        // Update the bar
        fullSize.x = emptySize.x * progress;
        _fullBar.sizeDelta = fullSize;
    }
}
