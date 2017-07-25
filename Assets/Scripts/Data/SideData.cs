using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SideData
{
    public bool accomplished;
    public string direction;
    
    public RepetitionData[] repetitions;
    public TipsData[] tips;
    
    public float userTime;
    public float confidence;
    public int attempts;
    public int unlocked;
    public bool isInteractable;
    
}
