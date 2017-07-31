using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseInfoManager : MonoBehaviour
{

	public GameObject exerciseTipPanel;
	public Transform buttonExerciseStart;
	
	public Transform spacer;

	public Text exerciseName;
	public Bounds spacerheight;
	
	// Use this for initialization
	void Start ()
	{
//		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser(); 
//		PlayerPrefs.SetInt("CurrentTierId", 1);
//		PlayerPrefs.SetInt("CurrentExerciseId", 3);
//		PlayerPrefs.SetInt("CurrentSideId", 0);
//		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
//		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		exerciseName.text = UserDataObject.GetCurrentExerciseAndSideName();
		
		FillTipList();
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene("ExerciseExecution");
	}
	public void LoadPreviousScene()
	{
		SceneManager.LoadScene("ExerciseSideSelection");
	}
	
	void FillTipList()
	{
		foreach (var tip in UserDataObject.GetCurrentTipsArray())
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipDescription.text = tip.description;
			tipPanel.tipNumber.text = (Array.IndexOf(UserDataObject.GetCurrentTipsArray(), tip)+1).ToString();
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
		if (UserDataObject.GetCurrentTipsArray().Length > 7 )
		{
		}
			buttonExerciseStart.transform.SetParent(spacer, false);
	}
}
