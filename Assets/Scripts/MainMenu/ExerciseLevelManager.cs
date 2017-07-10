using System;
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
	
	private List<TierData> _allTierData;
	private ExerciseData[] _allTierExercises;
	
	// Use this for initialization
	void Start ()
	{
		_allTierData = UserDataObject.GetAllTiers();
			
		FillMenu();
//	  	PlayerPrefs.DeleteAll(); // Deletes playerprefs
	}

	public void LoadPreviousScene()
	{
		SceneManager.LoadSceneAsync("UserSelection");
		
	}

	void FillMenu()
	{
		foreach (var tier in _allTierData)
		{
			GameObject tierGameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			ExerciseLevelButton tierButton = tierGameObjectButton.GetComponent<ExerciseLevelButton>();
	
			tierButton.buttonText.text = "Basic information";
			tierButton.unlocked = 1;
			tierButton.GetComponent<Button>().interactable = true;
			tierButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt("CurrentTierId", _allTierData.IndexOf(tier));
					
				Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
	
				SceneManager.LoadScene("TierInfo");
			});
			
			tierGameObjectButton.transform.SetParent(spacer, false);
			
			foreach (var exercise in tier.exercises)
			{
				GameObject gameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
				ExerciseLevelButton button = gameObjectButton.GetComponent<ExerciseLevelButton>();
	
				button.buttonText.text = exercise.exerciseName;
				
	//			Debug.Log("index of " + exercise.exerciseName + ": " + Array.IndexOf(_allExercises, exercise));
	//
	//			if (exercise.isInteractable)
	//			{
	//				button.unlocked = exercise.unlocked;
	//				button.GetComponent<Button>().interactable = exercise.isInteractable;
	//			}
				// If PP key from current exercise is unlocked --> unlock and make current exercise interactive
	//			if (PlayerPrefs.GetInt("Exercise" + Array.IndexOf(_allExercises, exercise) + "Unlocked")  == 1)
	//			{
	//				exercise.isInteractable = true;
	//				exercise.unlocked = 1;
	//			}
				
				Debug.Log("exercise.exerciseName: " + exercise.exerciseName);
				Debug.Log("exercise.unlocked: " + exercise.unlocked);
				button.unlocked = exercise.unlocked;
				button.GetComponent<Button>().interactable = exercise.isInteractable;
				button.GetComponent<Button>().onClick.AddListener(() =>
				{
					PlayerPrefs.SetInt("CurrentTierId", _allTierData.IndexOf(tier));
					PlayerPrefs.SetInt("CurrentExerciseId", tier.exercises.IndexOf(exercise));
					
					Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
					Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));
	
					SceneManager.LoadScene("ExerciseInfo");
				});
			
				gameObjectButton.transform.SetParent(spacer, false);
			}
		}
//		SaveData();
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
