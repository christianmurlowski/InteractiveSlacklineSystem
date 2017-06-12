﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseLevelManager : MonoBehaviour
{
	public GameObject exerciseLevelButton;
	public Transform spacer;

	private UserData _currentUser;
	
	private ExerciseData[] _allExercises;
	
	// Use this for initialization
	void Start () {

		_allExercises = UserDataObject.currentUser.exerciseData;
			
		PlayerPrefs.SetInt("ExercisesAmount", UserDataObject.currentUser.exerciseData.Length);

		FillMenu();
	  	PlayerPrefs.DeleteAll(); // Deletes playerprefs
	}

	void FillMenu()
	{
		foreach (var exercise in _allExercises)
		{
			GameObject gameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			ExerciseLevelButton button = gameObjectButton.GetComponent<ExerciseLevelButton>();

			button.buttonExerciseData = exercise;
			button.buttonText.text = exercise.levelName;
			
			Debug.Log("index of " + exercise.exerciseName + ": " + Array.IndexOf(_allExercises, exercise));

			
			// If PP key from current exercise is unlocked --> unlock and make current exercise interactive
			if (PlayerPrefs.GetInt("Exercise" + Array.IndexOf(_allExercises, exercise) + "Unlocked")  == 1)
			{
				exercise.isInteractable = true;
				exercise.unlocked = 1;
			}

			button.unlocked = exercise.unlocked;
			button.GetComponent<Button>().interactable = exercise.isInteractable;
			button.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("Exercise" + Array.IndexOf(_allExercises, exercise)));
		
			gameObjectButton.transform.SetParent(spacer, false);
			
			
		}
		SaveData();
	}

	// Initial PlayerPrefs creation for each Level --> set unlock value
	void SaveData()
	{
		if (PlayerPrefs.HasKey("Exercise0Unlocked"))
		{
			Debug.Log("Exercise0Unlocked exists");
			return;		
		}
		else
		{
			Debug.Log("Exercise0Unlocked exists NOT");
			GameObject[] allButtons = GameObject.FindGameObjectsWithTag("ExerciseButton");
			foreach (GameObject buttons in allButtons)
			{
				ExerciseLevelButton button = buttons.GetComponent<ExerciseLevelButton>();
				PlayerPrefs.SetInt("Exercise" + button.buttonText.text + "Unlocked", button.unlocked);
			}
			
		}
	}
	
}
