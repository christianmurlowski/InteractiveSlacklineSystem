﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseInfoManager : MonoBehaviour
{

	public GameObject exerciseTipPanel;
	public Button startButton;
	
	public Transform spacer;

	public Text exerciseName;
	
	private ExerciseData _currentExercise;
	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production

		_currentExercise = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		_currentExercise = UserDataObject.currentUser.exerciseData[0];// TODO Just for test purposes -> Delete in production

		exerciseName.text = _currentExercise.exerciseName.ToUpper();

		startButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("ExerciseExecution");
		});
		
		FillTipList();
	}


	void FillTipList()
	{
		foreach (var tip in _currentExercise.tips)
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipName.text = tip.name;
			tipPanel.tipDescription.text = tip.description;
//			tipPanel.tipImage = tip.image; TODO add image reference
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
/*
	TODO
	- next button to go into exercise
	- DELETE ALL EVENTSYSTEMS IN OTHER SCENES
	- create default images
	- load preview videofile into content
*/

}
