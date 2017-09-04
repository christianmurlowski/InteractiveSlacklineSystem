using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SideSelection : MonoBehaviour
{
	public GameObject KinectManager;
	
	public Button exerciseLeftButton, 
				  exerciseRightButton;
	
	public Text exerciseName;

	public Toggle toggleLeft,
				  toggleRight;
	public Image imageToggleLeftBG,
				 imageToggleLeftBGShadow,		 
				 imageToggleRightBG,
				 imageToggleRightBGShadow;


	private KinectManager _kinectManager;
	// Use this for initialization
	void Start () 
	{
		KinectManager = GameObject.Find("KinectManager");

		_kinectManager = KinectManager.GetComponent<KinectManager>();

		if (_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = false;

		Color32 green = MainColors.GreenLight();
		
		foreach (var side in UserDataObject.GetCurrentExercise().sides)
		{
			if (side.accomplished)
			{
				if(side.direction == "Left")
				{
					toggleLeft.isOn = true;
					imageToggleLeftBG.sprite = Resources.Load<Sprite>("Images/Toggle");
					imageToggleLeftBG.color = MainColors.ToggleIsOn();
					imageToggleLeftBGShadow.color = MainColors.SideSelectionShadow();
				}
				else if(side.direction == "Right")
				{
					toggleRight.isOn = true;
					imageToggleRightBG.sprite = Resources.Load<Sprite>("Images/Toggle");
					imageToggleRightBG.color = MainColors.ToggleIsOn();
					imageToggleRightBGShadow.color = MainColors.SideSelectionShadow();
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
