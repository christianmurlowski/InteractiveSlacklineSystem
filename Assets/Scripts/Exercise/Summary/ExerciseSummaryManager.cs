using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ExerciseSummaryManager : MonoBehaviour
{
	public GameObject CanvasHandCursor,
					  KinectManager,
					  timePanel,
					  confidencePanel,
					  attemptsPanel,
					  avgTimePanel,
					  avgConfidencePanel,
					  avgAttemptsPanel,
					  nextExercisePlatform;

	public Button buttonMainMenu,
				  buttonNextExercise;

	public Transform timeSpacer,
					 confidenceSpacer,
					 attemptsSpacer;
	
	public Text exerciseNameText,
				textStandingLeg;

	
	private KinectManager _kinectManager;

	private ExerciseData _currentExercise,
						 _nextExercise,
						 _lastExercise;
	
	private List<Image> imageTimeList, 
						imageConfidenceList, 
						imageAttemptsList;
	
	private List<float> imageTimeValue, 
						imageConfidenceValue, 
						imageAttemptsValue;

	private bool startAnim,
		startConfidenceAnim,
		startTimeAnim,
		startAttemptsAnim,
		sideNotAccomplished;
	
	Stopwatch tempTime = new Stopwatch();

	// Use this for initialization
	void Start () {
		
		// Enable handcursor in execution mode
		KinectManager = GameObject.Find("KinectManager");
//		KinectManager.GetComponent<InteractionManager>().showHandCursor = true;

		_kinectManager = KinectManager.GetComponent<KinectManager>();
		if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;

		Debug.Log("Enable handcursor");

		imageConfidenceList = new List<Image>();
		imageTimeList = new List<Image>();
		imageAttemptsList = new List<Image>();
		imageTimeValue = new List<float>();
		imageConfidenceValue = new List<float>();
		imageAttemptsValue = new List<float>();
		
		// Set the title of current exercise
		exerciseNameText.text = UserDataObject.GetCurrentExerciseName().ToUpper();
		textStandingLeg.text = UserDataObject.GetCurrentSide().direction;
		
		// Output summary data from exercise
		OutputData();

		_currentExercise = UserDataObject.GetCurrentExercise();
		_lastExercise = UserDataObject.GetLastTierExercise();
		
		Debug.Log("GetCurrentExercise: " + UserDataObject.GetCurrentExercise().fileName);
		Debug.Log("GetLastTierExercise " + UserDataObject.GetLastTierExercise().fileName);
		Debug.Log(PlayerPrefs.GetInt("CurrentExerciseId"));
		
		// If a side is not accomplished --> go to next side
		foreach (var side in _currentExercise.sides)
		{
			Debug.Log(side.direction);
			Debug.Log(side.accomplished);
			if (side.accomplished == false)
			{
				sideNotAccomplished = true;
			}
		}

		// Not last exercise --> load following exercise
		if (_currentExercise != _lastExercise || sideNotAccomplished)
		{
			Debug.Log("any sideNotAccomplished? : " + sideNotAccomplished);
			if (sideNotAccomplished)
			{
				buttonNextExercise.GetComponentInChildren<Text>().text = "Next Side";
				buttonNextExercise.onClick.AddListener(() =>
				{
					Debug.Log("currentside id : " + PlayerPrefs.GetInt("CurrentSideId"));

					if (PlayerPrefs.GetInt("CurrentSideId") == 0)
					{
						Debug.Log("LOAD LEFT");
						PlayerPrefs.SetInt("CurrentSideId", 1);
						LoadNextScene("Left");
					}
					else if (PlayerPrefs.GetInt("CurrentSideId") == 1)
					{
						Debug.Log("LOAD RIGHT");
						PlayerPrefs.SetInt("CurrentSideId", 0);
						LoadNextScene("Right");
					}
				});
			}
			else
			{
				PlayerPrefs.SetInt("CurrentExerciseId", PlayerPrefs.GetInt("CurrentExerciseId") + 1);
				buttonNextExercise.onClick.AddListener(() =>
				{
					SceneManager.LoadScene("ExerciseSideSelection");
				});
			}			
		}
		else
		{
			buttonNextExercise.GetComponentInChildren<Text>().text = "Level Summary";
			buttonNextExercise.onClick.AddListener(() =>
			{
				SceneManager.LoadScene("TierSummary");
			});
		}
	}

	public void LoadMainMenuScene()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void LoadNextScene(string side)
	{
		PlayerPrefs.SetString("CurrentSide", side);
		SceneManager.LoadScene("ExerciseInfo");		
	}

	private void Update()
	{
/*		// If left arrow pressed --> Main menu
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			LoadMainMenuScene();
		}
		// If right arrow pressed --> next exercise/side
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (PlayerPrefs.GetInt("CurrentSideId") == 0)
			{
				Debug.Log("LOAD LEFT");
				PlayerPrefs.SetInt("CurrentSideId", 1);
				LoadNextScene("Left");
			}
			else if (PlayerPrefs.GetInt("CurrentSideId") == 1)
			{
				Debug.Log("LOAD RIGHT");
				PlayerPrefs.SetInt("CurrentSideId", 0);
				LoadNextScene("Right");
			}
		}*/
		if (startAnim)
		{
			if (startTimeAnim)
			{
				foreach (var image in imageTimeList)
				{
					if (image.fillAmount < imageTimeValue[imageTimeList.IndexOf(image)])
					{
						image.fillAmount += 0.02f;
					}
					else if (image.fillAmount >= 1.0f)
					{
						startTimeAnim = false;
					}
				}
			}
			if (startConfidenceAnim)
			{	
				foreach (var image in imageConfidenceList)
				{
					if (image.fillAmount < imageConfidenceValue[imageConfidenceList.IndexOf(image)])
					{
						image.fillAmount += 0.02f;				
					}
					else if (image.fillAmount >= 1.0f)
					{
						startConfidenceAnim = false;
					}
				}
			}
			if (startAttemptsAnim)
			{	
				foreach (var image in imageAttemptsList)
				{
					if (image.fillAmount < imageAttemptsValue[imageAttemptsList.IndexOf(image)])
					{
						image.fillAmount += 0.02f;				
					}
					else if (image.fillAmount >= 1.0f)
					{
						startAttemptsAnim = false;
					}
				}
			}

			if (startTimeAnim == false && startConfidenceAnim == false && startAttemptsAnim == false)
			{
				startAnim = false;
			}
		}
	}

	private void OutputData()
	{
		foreach (var repetition in UserDataObject.GetCurrentRepetitionsArray())
		{
			GameObject timeGameObject = Instantiate(timePanel);
			RepetitionTimePanel currentTimePanel = timeGameObject.GetComponent<RepetitionTimePanel>();
			
			GameObject confidenceGameObject = Instantiate(confidencePanel);
			RepetitionConfidencePanel currentConfidencePanel = confidenceGameObject.GetComponent<RepetitionConfidencePanel>();
			
			GameObject attemptsGameObject = Instantiate(attemptsPanel);
			RepetitionAttemptsPanel currentAttemptsPanel = attemptsGameObject.GetComponent<RepetitionAttemptsPanel>();
			
			int repetitionId = Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), repetition) + 1;
			

			string reptitionText = repetitionId.ToString();
			switch (repetitionId)
			{
				case 1:
					reptitionText += "st rep";
					break;
				case 2:
					reptitionText += "nd rep";
					break;
				case 3:
					reptitionText += "rd rep";
					break;
				default:
					reptitionText += "th rep";
					break;
			}
			
			currentTimePanel.repetitionText.text = reptitionText;
			currentTimePanel.timeText.text = repetition.userTime.ToString("F1");
//			currentTimePanel.GetComponent<Image>().fillAmount = repetition.userTime / UserDataObject.GetCurrentExerciseLongestRepetitionTime();
			imageTimeList.Add(currentTimePanel.GetComponent<Image>());
			imageTimeValue.Add(repetition.userTime / UserDataObject.GetCurrentExerciseLongestRepetitionTime());
			
			currentAttemptsPanel.repetitionText.text = reptitionText;
			currentAttemptsPanel.attemptsText.text = repetition.attempts.ToString();
			imageAttemptsList.Add(currentAttemptsPanel.GetComponent<Image>());
			imageAttemptsValue.Add((float) repetition.attempts / UserDataObject.GetCurrentExerciseSideHighestAttempt());

			currentConfidencePanel.repetitionText.text = reptitionText;
			currentConfidencePanel.confidenceText.text = Mathf.Round(repetition.confidence).ToString();
//			currentConfidencePanel.GetComponent<Image>().fillAmount = repetition.confidence/100;
			imageConfidenceList.Add(currentConfidencePanel.GetComponent<Image>());
			imageConfidenceValue.Add(repetition.confidence/100);
			
			
			attemptsGameObject.transform.SetParent(attemptsSpacer, false);
			timeGameObject.transform.SetParent(timeSpacer, false);
			confidenceGameObject.transform.SetParent(confidenceSpacer, false);
		}
		
		avgTimePanel.GetComponent<AverageTimePanel>().averageTimeText.text =
			UserDataObject.GetCurrentExerciseAverageRepetitionTime().ToString("F1") + " sec";
		
		avgAttemptsPanel.GetComponent<AverageAttemptsPanel>().avgAttemptsText.text =
			UserDataObject.GetCurrentExerciseAverageRepetitionAttempts().ToString("F1");
		
		avgConfidencePanel.GetComponent<AverageConfidencePanel>().averageConfidenceText.text =
			UserDataObject.GetCurrentExerciseAverageRepetitionConfidence().ToString("F1") + " %";

		startConfidenceAnim = true;
		startTimeAnim = true;
		startAttemptsAnim = true;
		startAnim = true;
	}
}
