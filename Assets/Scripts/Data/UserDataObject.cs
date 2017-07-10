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

//    public ExerciseData[] GeAllExercises()
//    {
//        return currentUser.exerciseData;
//    }

    public static List<TierData> GetAllTiers()
    {
        return currentUser.tierData;
    }
    public static TierData GetCurrentTier()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")];
    }

    public static int GetCurrentTierErcisesLength()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises.Count;
    }

    public static ExerciseData GetCurrentExercise()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")];
    }
    
    public static string GetCurrentExerciseName()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].levelName;
    }
    
    
    public static ExerciseData GetNextExercise()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId") + 1];
    }

}
