﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCamera : MonoBehaviour //stolen from Vicente
{

    [SerializeField]
    bool EnableCapture = false;
    [SerializeField]
    float _interval = 0.2f;

    string folder = @"D:\Unity\ALIS_Aggregation\ScreenCapture";

    void Start()
    {
        Time.captureFramerate = 25;
    }

    void Update()
    {
        if (EnableCapture)
            StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        yield return new WaitForSeconds(_interval);

        string filename = $@"{folder}\image_{Time.frameCount:00000}.png";

        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}