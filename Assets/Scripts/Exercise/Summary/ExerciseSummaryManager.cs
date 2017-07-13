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
	
	public GameObject kinectManager;
	
	// Use this for initialization
	void Start () {
		
		// Enable handcursor in execution mode
		kinectManager = GameObject.Find("KinectManager");
		kinectManager.GetComponent<InteractionManager>().showHandCursor = true;
		Debug.Log("Enable handcursor");
		
//		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production

		_currentExerciseData = UserDataObject.GetCurrentExercise();

		Debug.Log(_currentExerciseData.exerciseName);
		
		// Set the title of current exercise
		exerciseNameText.text = "Exercise " + _currentExerciseData.exerciseName.ToUpper() + " completed!";
		
		// Output summary data from exercise
		OutputData();
		
		mainMenuButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("MainMenu");
		});
	}


	private void OutputData()
	{
		foreach (var repetition in UserDataObject.GetCurrentRepetitionsArray())
		{
			GameObject timeGameObject = Instantiate(timePanel);
			RepetitionTimePanel currentTimePanel = timeGameObject.GetComponent<RepetitionTimePanel>();
			
			GameObject confidenceGameObject = Instantiate(confidencePanel);
			RepetitionConfidencePanel currentConfidencePanel = confidenceGameObject.GetComponent<RepetitionConfidencePanel>();
			
			int repetitionId = Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), repetition);
			
			currentTimePanel.timeText.text = "Rep " + repetitionId  + ": " + repetition.userTime.ToString() + " sec";
			currentConfidencePanel.confidenceText.text = "Rep " + repetitionId  + ": " + repetition.confidence.ToString() + " %";
			
			timeGameObject.transform.SetParent(timeSpacer, false);
			confidenceGameObject.transform.SetParent(confidenceSpacer, false);
		}
	}
}
