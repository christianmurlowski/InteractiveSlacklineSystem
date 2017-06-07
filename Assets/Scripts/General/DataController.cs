using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DataController : MonoBehaviour
{
	public ExerciseData[] allDefaultExercisesDataArray;
	public ExerciseObject allDefaultExercisesDataObject;

	public UserData currentUser;

	private string exerciseDataFileName = "JSONData/exerciseData.json";
	private string userDataFileName = "JSONData/christianMurlowski.json";
	
	void Start ()
	{
		LoadGameData();
		
		Debug.Log("currentuser exercise: " + currentUser.exerciseData.Length);
		
		// Initial fill current users exercises with exercise data
		if (currentUser.exerciseData.Length == 0)
		{
//			currentUser.exerciseData = ;
			Debug.Log("in if currentuser exercise: " + currentUser.exerciseData.Length);
		}
	}
	
	public ExerciseData[] GetAllExercises()
	{
		return currentUser.exerciseData;
	}
	
	// TODO: Get current exercise data
	public ExerciseData GetCurrentExercise()
	{
		return currentUser.exerciseData[0]; // change this
	}

	public int GetAllExercisesLength()
	{
		return currentUser.exerciseData.Length;
	}

	public UserData GetCurrentUser()
	{
		return currentUser;
	}

	private void LoadGameData()
	{
		string defaultExercisesFilePath = Path.Combine(Application.streamingAssetsPath, exerciseDataFileName);
		string currentUserFilePath = Path.Combine(Application.streamingAssetsPath, userDataFileName);

		if (File.Exists(defaultExercisesFilePath) && File.Exists(currentUserFilePath))
		{
			string defaultExercisesDataAsJson = File.ReadAllText(defaultExercisesFilePath );
			string currentUserDataAsJson = File.ReadAllText(currentUserFilePath );
			
			Debug.Log(currentUserDataAsJson);
			allDefaultExercisesDataObject = JsonUtility.FromJson<ExerciseObject>(defaultExercisesDataAsJson);
			currentUser = JsonUtility.FromJson<UserData>(currentUserDataAsJson);

			allDefaultExercisesDataArray = allDefaultExercisesDataObject.exerciseDataArray;
			
			
			Debug.Log("----------DEFAULT EXERCISES----------");
			Debug.Log(allDefaultExercisesDataObject.exerciseDataArray.Length);
			Debug.Log(allDefaultExercisesDataObject.exerciseDataArray[0].description);
			Debug.Log(allDefaultExercisesDataArray);
			Debug.Log(allDefaultExercisesDataArray.Length);
			Debug.Log(allDefaultExercisesDataArray[0].description);		

			Debug.Log("----------CURRENT USER----------");
			Debug.Log(currentUser.name);
			
			Debug.Log("----------CURRENT USER EXERCISES BEFORE----------");
			Debug.Log(currentUser.exerciseData.Length);
			Debug.Log(currentUser.exerciseData[0].exerciseName);

			// if current user has no exercise data (initial state) fill with default exercises
//			if (currentUser.exerciseData == null)
//			{
//				currentUser.exerciseData = allDefaultExercisesDataArray;
//				//write into jsonfile
//			}
//			
			currentUser.exerciseData = allDefaultExercisesDataArray;
			
			Debug.Log("----------CURRENT USER EXERCISES AFTER----------");
			Debug.Log(currentUser.exerciseData.Length);
			foreach (var userex in currentUser.exerciseData)
			{
				Debug.Log(userex.exerciseName);
			}
			
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
