using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishExercise : MonoBehaviour
{

	private int _exerciseAmount;
	private int _currentExercise;
	private int _score = 5000;
	
	// Use this for initialization
	void Start ()
	{
		_exerciseAmount = PlayerPrefs.GetInt("ExerciseAmount");
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
		for (int i = 0; i < _exerciseAmount; i++)
		{
			if (SceneManager.GetActiveScene().name == "Exercise" + i)
			{
				_currentExercise = i;
				SaveGame();
			}
		}
	}

	void SaveGame()
	{
		int nextExercise = _currentExercise + 1;

		if (nextExercise < _exerciseAmount)
		{
			PlayerPrefs.SetInt("Exercise" + nextExercise, 1); // unlock next exercise
			PlayerPrefs.SetInt("Exercise" + _currentExercise + "_score", _score);
		}
		else
		{
			PlayerPrefs.SetInt("Exercise" + _currentExercise + "_score", _score);
		}
	}
	
	
}
