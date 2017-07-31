using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TierData
{
    public string fileName;
    public string tierName;
    public bool accomplished;
    public bool isInteractable;

    public List<GoalData> goals;
    public List<TipsData> tips;
    public List<ExerciseData> exercises;
}
