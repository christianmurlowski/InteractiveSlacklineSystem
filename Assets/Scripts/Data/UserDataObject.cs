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

    // -----------------------------------------
    // ----------------- TIER ------------------
    // -----------------------------------------
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
   
    public static string GetCurrentTierFileName()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].fileName;
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
    
    // -----------------------------------------
    // --------------- EXERCISE ----------------
    // -----------------------------------------
    public static ExerciseData GetFirstTierExercise()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[0];
    }
    
    public static ExerciseData GetCurrentExercise()
    {
        // todo maybe check if current exercise exists in this tier or is in ragne of the tier
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")];
    }
        
    public static string GetCurrentExerciseName()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].exerciseName;
    }
            
    public static string GetCurrentExerciseAndSideName()
    {
        return GetCurrentExerciseName() +  " " + GetCurrentSide().direction;
    }
    
    public static string GetCurrentExerciseFileName()
    {
        string fileName = currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")]
            .exercises[PlayerPrefs.GetInt("CurrentExerciseId")].fileName;
        
        return fileName;
    }     
    
    public static string GetCurrentExerciseDatabaseName()
    {
        string fileName = GetCurrentExerciseFileName();
        
        if (GetCurrentExercise().isProgressGesture)
        {
            fileName += "Progress";
        }
        
        return fileName;
    } 
    
    public static ExerciseData GetNextExercise()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId") + 1];
    }    
    
    // -----------------------------------------
    // ----------------- SIDE ------------------
    // -----------------------------------------
    public static SideData GetCurrentSide()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")];
    }
    
    // -----------------------------------------
    // -------------- REPETITION ---------------
    // -----------------------------------------
    public static RepetitionData[] GetCurrentRepetitionsArray()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].repetitions;
    }
            
    public static RepetitionData GetCurrentRepetition()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].repetitions[PlayerPrefs.GetInt("CurrentRepetitionId")];
    }
    
    // -----------------------------------------
    // ----------------- TIPS ------------------
    // -----------------------------------------
    public static TipsData[] GetCurrentTipsArray()
    {
        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises[PlayerPrefs.GetInt("CurrentExerciseId")].sides[PlayerPrefs.GetInt("CurrentSideId")].tips;
    }
    
//    public static ExerciseData GetCurrentTierLastExercise()
//    {
//        return currentUser.tierData[PlayerPrefs.GetInt("CurrentTierId")].exercises.Last();
//    }

}
