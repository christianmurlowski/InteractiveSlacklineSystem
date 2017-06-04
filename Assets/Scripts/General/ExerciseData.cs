using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class ExerciseData
{
	public string name;
	public string description;
	
	public RepetitionData[] repetitions;
//	public bool accomplished;
//	public int unlocked;
//	public bool IsInteractable;


	[System.Serializable]
	public class UserExerciseData
	{
		public bool accomplished;
		public int unlocked;
		public bool IsInteractable;
	}
}
