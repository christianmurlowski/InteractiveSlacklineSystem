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

	public Text exerciseNameText;

	public Transform timeSpacer,
					 confidenceSpacer,
					 attemptsSpacer;
	
	public GameObject timePanel,
					  confidencePanel,
					  attemptsPanel;
	
	public GameObject avgTimePanel,
					  avgConfidencePanel,
					  avgAttemptsPanel;

	private bool startAnim, startConfidenceAnim, startTimeAnim, startAttemptsAnim;
	private List<Image> imageTimeList, imageConfidenceList, imageAttemptsList;
	private List<float> imageTimeValue, imageConfidenceValue;
	private List<float> imageAttemptsValue;
	
	public Button mainMenuButton;
	
	public GameObject kinectManager;
	Stopwatch tempTime = new Stopwatch();

	// Use this for initialization
	void Start () {
		
		// Enable handcursor in execution mode
//		kinectManager = GameObject.Find("KinectManager");
//		kinectManager.GetComponent<InteractionManager>().showHandCursor = true;
		Debug.Log("Enable handcursor");
		
//		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
//		PlayerPrefs.SetInt("CurrentTierId", 0);// TODO Just for test purposes -> Delete in production
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production

		imageConfidenceList = new List<Image>();
		imageTimeList = new List<Image>();
		imageAttemptsList = new List<Image>();
		imageTimeValue = new List<float>();
		imageConfidenceValue = new List<float>();
		imageAttemptsValue = new List<float>();
		
		// Set the title of current exercise
		exerciseNameText.text = UserDataObject.GetCurrentExerciseAndSideName().ToUpper();
		
		// Output summary data from exercise
		OutputData();
		
		mainMenuButton.onClick.AddListener(() =>
		{
			SceneManager.LoadScene("MainMenu");
		});
	}


	private void Update()
	{
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
			
			int repetitionId = Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), repetition);
			
			currentTimePanel.timeText.text = repetition.userTime.ToString("F1");

			string reptitionText = "";
			switch (repetitionId)
			{
				case 0:
					reptitionText = "1st";
					break;
				case 1:
					reptitionText = "2nd";
					break;
				case 2:
					reptitionText = "3rd";
					break;
				case 3:
					reptitionText = "4th";
					break;
				case 4:
					reptitionText = "5th";
					break;
			}
			
			currentTimePanel.repetitionText.text = reptitionText;
//			currentTimePanel.GetComponent<Image>().fillAmount = repetition.userTime / UserDataObject.GetCurrentExerciseLongestRepetitionTime();
			imageTimeList.Add(currentTimePanel.GetComponent<Image>());
			imageTimeValue.Add(repetition.userTime / UserDataObject.GetCurrentExerciseLongestRepetitionTime());
			
			currentAttemptsPanel.attemptsText.text = repetition.attempts.ToString();
			imageAttemptsList.Add(currentAttemptsPanel.GetComponent<Image>());
			imageAttemptsValue.Add((float) repetition.attempts / UserDataObject.GetCurrentExerciseSideHighestAttempt());

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
