using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SideSelection : MonoBehaviour
{

	public Button exerciseLeftButton, exerciseRightButton;
	public Text leftButtonText, rightButtonText;
	public Text exerciseName;
	
	// Use this for initialization
	void Start () 
	{
		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser();
//		PlayerPrefs.SetInt("CurrentTierId", 0);
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);

		Color32 green = new Color32(32, 147, 92, 255);
		
		foreach (var side in UserDataObject.GetCurrentExercise().sides)
		{
			if (side.accomplished)
			{
				if(side.direction == "Left")
				{
					leftButtonText.text = side.direction + " side\n(accomplished)";
					leftButtonText.color = green;
				}
				else if(side.direction == "Right")
				{
					rightButtonText.text = side.direction + " side\n(accomplished)";
					rightButtonText.color = green;
				}
			}
		}
		
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
		SceneManager.LoadScene("MainMenu");
	}
	
	private void LoadNextScene(string side)
	{
		PlayerPrefs.SetString("CurrentSide", side);
		SceneManager.LoadScene("ExerciseInfo");		
	}
	
}
