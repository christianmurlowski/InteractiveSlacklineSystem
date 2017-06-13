using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishExercise : MonoBehaviour
{

	private int _exerciseAmount;
	private int _currentExerciseId;
	private int _score = 5000;
	
	// Use this for initialization
	void Start ()
	{
		_currentExerciseId = PlayerPrefs.GetInt("CurrentExerciseId");
		
		_exerciseAmount = PlayerPrefs.GetInt("ExercisesAmount");
		
		CheckCurrentLevel();
		StartCoroutine(Time());
	}

	IEnumerator Time()
	{
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("MainMenu");
	}

	void CheckCurrentLevel()
	{
		
		SaveGame();
	}

	void SaveGame()
	{
		int nextExerciseId = _currentExerciseId + 1;

		if (nextExerciseId < UserDataObject.currentUser.exerciseData.Length)
		{
			// Set next exercise and unlock
			PlayerPrefs.SetInt("CurrentExerciseId", nextExerciseId);
			UserDataObject.currentUser.exerciseData[nextExerciseId].isInteractable = true;
			UserDataObject.currentUser.exerciseData[nextExerciseId].unlocked = 1;
			
			// TODO: save it to json file

			string currentExerciseAsJson = JsonUtility.ToJson(UserDataObject.currentUser);
			File.WriteAllText(PlayerPrefs.GetString("CurrentUserFilePath"), currentExerciseAsJson);
//			PlayerPrefs.SetInt("Exercise" + "_score", _score);
		}
		else
		{
//			PlayerPrefs.SetInt("Exercise" + _currentExercise + "_score", _score);
		}
	}
	
	
}
