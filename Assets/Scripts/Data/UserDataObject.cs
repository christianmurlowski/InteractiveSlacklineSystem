using System.Collections;
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

    public static List<ExerciseData> GetCurrentTierAllExercises()
    {
        return GetCurrentTier().exercises;
    }
    
    public static float GetCurrentTierAllExercisesHighestTime()
    {
        float highestTime = 0.0f;
        foreach (var exercise in GetCurrentTierAllExercises())
        {
            if (highestTime < exercise.userTime)
            {
                highestTime = exercise.userTime;
            }
        }

        return highestTime;
    }    
    public static float GetCurrentTierAllExercisesHighestAttempt()
    {
        int highestAttempt = 0;
        foreach (var exercise in GetCurrentTierAllExercises())
        {
            if (highestAttempt < exercise.attempts)
            {
                highestAttempt = exercise.attempts;
            }
        }

        return highestAttempt;
    }
        
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
        
    public static float GetCurrentExerciseStartingHeightDifference()
    {
        return GetCurrentExercise().startingHeightDifference;
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

    public static float GetCurrentExerciseLongestRepetitionTime()
    {
        float longestRep = 0f;

        foreach (var repetition in GetCurrentRepetitionsArray())
        {
            if (longestRep < repetition.userTime)
            {
                longestRep = repetition.userTime;
            }
        }
        return longestRep;
    }
        
    public static int GetCurrentExerciseSideHighestAttempt()
    {
        int highestAttempt = 0;

        foreach (var repetition in GetCurrentRepetitionsArray())
        {
            if (repetition.attempts > highestAttempt)
            {            
                highestAttempt = repetition.attempts;
            }
        }
        return highestAttempt;
    }
    
    public static float GetCurrentExerciseAverageRepetitionAttempts()
    {
        float avgAttempt = 0;

        foreach (var repetition in GetCurrentRepetitionsArray())
        {
            avgAttempt += repetition.attempts;
        }
        return avgAttempt / GetCurrentRepetitionsArray().Length;
    }
        
    public static float GetCurrentExerciseAverageRepetitionTime()
    {
        float avgTime = 0f;

        foreach (var repetition in GetCurrentRepetitionsArray())
        {
            avgTime += repetition.userTime;
        }
        return avgTime / GetCurrentRepetitionsArray().Length;
    }
    
        
    public static float GetCurrentExerciseAverageRepetitionConfidence()
    {
        float avgConfidence = 0f;

        foreach (var repetition in GetCurrentRepetitionsArray())
        {
            avgConfidence += repetition.confidence;
        }
        return avgConfidence / GetCurrentRepetitionsArray().Length;
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
