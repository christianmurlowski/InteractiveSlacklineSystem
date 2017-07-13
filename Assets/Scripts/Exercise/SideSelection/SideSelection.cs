using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SideSelection : MonoBehaviour
{

	public Button exerciseLeftButton, exerciseRightButton;
	public Text exerciseName;
	
	// Use this for initialization
	void Start () 
	{
		exerciseLeftButton.onClick.AddListener(() =>
		{	
			PlayerPrefs.SetInt("CurrentSideId", 1);
			LoadNextScene("Left");
		});
		
		exerciseRightButton.onClick.AddListener(() =>
		{	
			PlayerPrefs.SetInt("CurrentSideId", 0);
			LoadNextScene("Right");
		});

		exerciseName.text = UserDataObject.GetCurrentExerciseName().ToUpper();
	}

	public void LoadPreviousScene()
	{
		SceneManager.LoadSceneAsync("MainMenu");
	}
	
	private void LoadNextScene(string side)
	{
		PlayerPrefs.SetString("CurrentSide", side);
		SceneManager.LoadSceneAsync("ExerciseInfo");		
	}
	
}
