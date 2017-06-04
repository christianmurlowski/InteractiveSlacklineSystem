using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string name;
    public bool isCurrentUser;

//    public ExerciseData[] exercises;
    public ExerciseData.UserExerciseData[] userExerciseData;
}
