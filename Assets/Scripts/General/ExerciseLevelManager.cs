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

	public GameObject dataController;
	private DataController _dataController;

	private UserData currentUser;
	
	private ExerciseData[] _allExercises;
	
	// Use this for initialization
	void Start () {
	  	PlayerPrefs.DeleteAll(); // Deletes playerprefs

		_dataController = dataController.GetComponent<DataController>();
		_allExercises = _dataController.GetAllExercises();
		currentUser = _dataController.GetCurrentUser();
			
		PlayerPrefs.SetInt("ExerciseAmount", _dataController.GetAllExercisesLength());

		FillMenu();
	}

	void FillMenu()
	{
		foreach (var exercise in _allExercises)
		{
			GameObject gameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			ExerciseLevelButton button = gameObjectButton.GetComponent<ExerciseLevelButton>();
			
			button.buttonText.text = exercise.levelName;

			// If PP key from current exercise is unlocked --> unlock and make current exercise interactive
			if (PlayerPrefs.GetInt("Exercise" + button.buttonText.text)  == 1)
			{
				exercise.isInteractable = true;
				exercise.unlocked = 1;
			}

			button.unlocked = exercise.unlocked;
			button.GetComponent<Button>().interactable = exercise.isInteractable;
			button.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("Exercise" + button.buttonText.text));
		
			gameObjectButton.transform.SetParent(spacer, false);
		}
		SaveData();
	}

	// Initial PlayerPrefs creation for each Level --> set unlock value
	void SaveData()
	{
		if (PlayerPrefs.HasKey("Exercise0"))
		{
			return;		
		}
		else
		{
			GameObject[] allButtons = GameObject.FindGameObjectsWithTag("ExerciseButton");
			foreach (GameObject buttons in allButtons)
			{
				ExerciseLevelButton button = buttons.GetComponent<ExerciseLevelButton>();
				PlayerPrefs.SetInt("Exercise" + button.buttonText.text, button.unlocked);
			}
			
		}
	}
	
}
