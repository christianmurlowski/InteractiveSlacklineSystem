using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class ExerciseData
{
	public string levelName;
	public string exerciseName;
	public string description;
	public bool isProgressGesture;
	
	public RepetitionData[] repetitions;
	public TipsData[] tips;
	public bool accomplished;
	public int unlocked;
	public bool isInteractable;


//	[System.Serializable]
//	public class UserExerciseData
//	{
//		public bool accomplished;
//		public int unlocked;
//		public bool IsInteractable;
//	}
}
