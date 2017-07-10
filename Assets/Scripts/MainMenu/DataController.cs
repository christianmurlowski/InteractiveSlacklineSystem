using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DataController : MonoBehaviour
{
	public List<TierData> defaultDataArray;
	public TierDataObject defaultDataObject;
	
	
	public List<ExerciseData> allDefaultExercisesDataArray;
	public ExerciseObject allDefaultExercisesDataObject;

	public UserData currentUser;

	private string exerciseDataFileName = "JSONData/exerciseDataProduction.json";
	
	void Start ()
	{
//		UserSelectionManager.TestSetCurrentUser();    // TODO Just for test purposes -> Delete in production

		currentUser = UserDataObject.currentUser;

		Debug.Log("START " + currentUser.name);
		
		LoadGameData();
	}

	private void LoadGameData()
	{
		string defaultExercisesFilePath = Path.Combine(Application.streamingAssetsPath, exerciseDataFileName);

		if (File.Exists(defaultExercisesFilePath) && currentUser != null)
		{
			string defaultExercisesDataAsJson = File.ReadAllText(defaultExercisesFilePath );
			
			defaultDataObject = JsonUtility.FromJson<TierDataObject>(defaultExercisesDataAsJson);
			
			defaultDataArray = defaultDataObject.tierDataList;
			
//			allDefaultExercisesDataObject = JsonUtility.FromJson<ExerciseObject>(defaultExercisesDataAsJson);
//			allDefaultExercisesDataArray = allDefaultExercisesDataObject.exerciseDataArray;
			
//			
//			Debug.Log("----------DEFAULT EXERCISES----------");
//			Debug.Log(allDefaultExercisesDataObject.exerciseDataArray.Length);
//			Debug.Log(allDefaultExercisesDataObject.exerciseDataArray[0].description);
//			Debug.Log(allDefaultExercisesDataArray);
//			Debug.Log(allDefaultExercisesDataArray.Length);
//			Debug.Log(allDefaultExercisesDataArray[0].description);		
//
//			Debug.Log("----------CURRENT USER----------");
//			Debug.Log(currentUser.name);
//			
//			Debug.Log("----------CURRENT USER EXERCISES BEFORE----------");
//			Debug.Log(currentUser.exerciseData.Length);
//			foreach (var userexer in currentUser.exerciseData)
//			{
//				Debug.Log(userexer.exerciseName);
//			}
			// if current user has no exercise data (initial state) fill with default exercises
//			if (currentUser.exerciseData == null)
//			{
//				currentUser.exerciseData = allDefaultExercisesDataArray;
//				//write into jsonfile
//			}
//			
			// Initial fill current users exercises with exercise data
			if (currentUser.tierData.Count == 0)
			{
				currentUser.tierData = defaultDataArray;
				Debug.Log("CURRENT USER HAS NO EXERCISES");
				Debug.Log("Currentuser tier length: " + currentUser.tierData.Count);
				foreach (var tier in currentUser.tierData)
				{			
					Debug.Log("Tier level name" + tier.levelName);
					Debug.Log("Tier name" + tier.tierName);
				}
				SaveUserData(currentUser);

			}
			
//			Debug.Log("----------CURRENT USER EXERCISES AFTER----------");
//			Debug.Log(currentUser.exerciseData.Length);
//			foreach (var userex in currentUser.exerciseData)
//			{
//				Debug.Log(userex.exerciseName);
//			}
//			
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}
	
	private void SaveUserData(UserData currentUserData)
	{
		string currentUserDataFilePath = PlayerPrefs.GetString("CurrentUserFilePath");
		string currentUserDataAsJson = JsonUtility.ToJson(currentUserData);
		string filePath = currentUserDataFilePath;
        
		File.WriteAllText(filePath, currentUserDataAsJson);
	}

	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
