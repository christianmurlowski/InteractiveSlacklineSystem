using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserDataObject {

    public static UserData currentUser;
    
    public void SetCurrentUser(UserData user)
    {
        currentUser = user;
        currentUser.isCurrentUser = true;
    }    
    
    public UserData GetCurrentUser()
    {
        return currentUser;
    }

    public ExerciseData[] GeAllExercises()
    {
        return currentUser.exerciseData;
    }

    // TODO: Get current exercise data
    public ExerciseData GetCurrentExercise()
    {
        return currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
    }

    public int GetAllErcisesLength()
    {
        return currentUser.exerciseData.Length;
    }
}
