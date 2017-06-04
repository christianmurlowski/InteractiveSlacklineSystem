using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
	public UserData[] allUserData;
	public ExerciseData[] allExerciseData;
	public ExerciseData.UserExerciseData[] allUserExerciseData;

	public UserData currentUser;
	
	void Start () 
	{
		// First get the current User
		foreach (var user in allUserData)
		{
			if (user.isCurrentUser)
			{
				currentUser = user;
			}
		}
		
		currentUser.userExerciseData = allUserExerciseData;
/*//		currentUser.userExerciseData = currentUserExerciseData;
		Debug.Log("Datacontroller");
		Debug.Log("allExerciseData: " + allExerciseData.Length);
		Debug.Log("currentuser exercise: " + currentUser.userExerciseData.Length);
		
		// Initial fill current users exercises with exercise data
		if (currentUser.userExerciseData.Length == 0)
		{
//			currentUser.userExerciseData = new ExerciseData.UserExerciseData[allExerciseData.Length];		
			Debug.Log("currentuser exercise: " + currentUser.userExerciseData.Length);
			Debug.Log("currentuser accom: " + currentUser.userExerciseData[0].unlocked);
		}*/
	}
	
	public ExerciseData[] GetAllExerciseData()
	{
		return allExerciseData;
	}
	
	// TODO: Get current exercise data
	public ExerciseData GetCurrentExerciseData()
	{
		return allExerciseData[0]; // change this
	}

	public int GetAllExercisesLength()
	{
		return allExerciseData.Length;
	}

	public ExerciseData.UserExerciseData[] GetUserExerciseData()
	{
		return currentUser.userExerciseData;
	}

	// TODO: Get current exercise data from user
	public ExerciseData.UserExerciseData GetUserCurrentExerciseData()
	{
		return currentUser.userExerciseData[0]; // Change this
	}

	public UserData[] GetAllUsers()
	{
		return allUserData;
	}

	public UserData GetCurrentUserData()
	{
		return currentUser;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
