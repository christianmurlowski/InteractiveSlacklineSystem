using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TierData
{
    public string tierName;
    public string levelName;
    public bool accomplished;
    public List<GoalData> goals;
    public List<ExerciseData> exercises;
}
