using System;
using UnityEngine;

[Serializable]
public class DDRSongData
{
    public string songId;
    public string audioFileName;
    public float bpm = 120f;
    public float startOffsetSeconds = 0f;
    public DDRStepData[] steps;
}

[Serializable]
public class DDRStepData
{
    public float beat;
    public string key;
    public int animationValue;
}
