﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
   
    public static TierData GetNextTier()
    {
        TierData nextTier = GetCurrentTier();
        
        if (PlayerPrefs.GetInt("CurrentTierId") != GetAllTiers().Count - 1)
        {
            nextTier = currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId") + 1];
        }
        return nextTier;
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
    
    public static RepetitionData[] GetCurrentRepetitionsArray()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].repetitions;
    }
            
    public static RepetitionData GetCurrentRepetition()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].repetitions[PlayerPrefs.GetInt("CurrentRepetitionId")];
    }
        
    public static TipsData[] GetCurrentTipsArray()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].tips;
    }
    
//    public static ExerciseData GetCurrentTierLastExercise()
//    {
//        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises.Last();
//    }

}
