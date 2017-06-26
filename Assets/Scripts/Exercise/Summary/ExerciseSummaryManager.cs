using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseSummaryManager : MonoBehaviour {

	public Text exerciseNameText;
	private ExerciseData _currentExerciseData;

	public Transform timeSpacer;
	public Transform confidenceSpacer;
	
	public GameObject timePanel;
	public GameObject confidencePanel;

	public Button mainMenuButton;
	
	// Use this for initialization
	void Start () {
	
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production
		
		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];

		Debug.Log(_currentExerciseData.exerciseName);
		
		exerciseNameText.text = "Exercise " + _currentExerciseData.exerciseName.ToUpper() + " completed!";

		foreach (var repetition in _currentExerciseData.repetitions)
		{
			GameObject timeGameObject = Instantiate(timePanel);
			RepetitionTimePanel currentTimePanel = timeGameObject.GetComponent<RepetitionTimePanel>();
			
			GameObject confidenceGameObject = Instantiate(confidencePanel);
			RepetitionConfidencePanel currentConfidencePanel = confidenceGameObject.GetComponent<RepetitionConfidencePanel>();
			
			int repetitionId = Array.IndexOf(_currentExerciseData.repetitions, repetition);
			
			currentTimePanel.timeText.text = "Rep " + repetitionId  + ": " + repetition.userTime.ToString() + " sec";
			currentConfidencePanel.confidenceText.text = "Rep " + repetitionId  + ": " + repetition.confidence.ToString() + " %";
			
			timeGameObject.transform.SetParent(timeSpacer, false);
			confidenceGameObject.transform.SetParent(confidenceSpacer, false);
		}
		
		mainMenuButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("MainMenu");
		});
	}
}
