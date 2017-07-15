using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TierData
{
    public bool accomplished;
    public string fileName;
    public string tierName;
    public bool isInteractable;

    public List<GoalData> goals;
    public List<ExerciseData> exercises;
}
