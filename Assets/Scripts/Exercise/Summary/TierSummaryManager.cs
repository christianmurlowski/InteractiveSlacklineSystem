using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierSummaryManager : MonoBehaviour
{

	public GameObject KinectManager;
	
	private KinectManager _kinectManager;
	private TierData _currentTierData;
	
	[Tooltip("Prefab of goal panel")]
	public Transform timeSpacer, 
					 attemptsSpacer, 
					 confidenceSpacer,
					 exerciseNumberSpacer;

	public GameObject exerciseTimeImage, 
					  exerciseAttemptsImage, 
					  exerciseConfidenceImage,
					  exerciseNumberText;

	public Text tierNameText;
	
	
	// Use this for initialization
	void Start ()
	{
		KinectManager = GameObject.Find("KinectManager");

		if (KinectManager) _kinectManager = KinectManager.GetComponent<KinectManager>();

		if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;
		
		_currentTierData = UserDataObject.GetCurrentTier();
		
		tierNameText.text = UserDataObject.GetCurrentTier().tierName + " average data";
		
		FillSummaryList();
	}

	public void LoadMainMenuScene()
	{
		if (!UserDataObject.GetCurrentExercise().isInteractable)
		{
			UserDataObject.GetCurrentExercise().isInteractable = true;
			UserDataObject.GetCurrentExercise().unlocked = 1;	
		}
		SceneManager.LoadScene("MainMenu");
	}	
	
	public void LoadTierMenuScene()
	{
		if (!UserDataObject.GetCurrentExercise().isInteractable)
		{
			UserDataObject.GetCurrentExercise().isInteractable = true;
			UserDataObject.GetCurrentExercise().unlocked = 1;	
		}
		SceneManager.LoadScene("TierMenu");
	}
	
	void FillSummaryList()
	{
		Debug.Log(_currentTierData.fileName);
		Debug.Log(_currentTierData.tips);
		Debug.Log(_currentTierData.tips[1]);
		
		foreach (var exercise in _currentTierData.exercises)
		{
			GameObject gameObjectTierExerciseTime = Instantiate(exerciseTimeImage) as GameObject;
			GameObject gameObjectTierExerciseConfidence = Instantiate(exerciseConfidenceImage) as GameObject;
			GameObject gameObjectTierExerciseAttempts = Instantiate(exerciseAttemptsImage) as GameObject;
			
			GameObject gameObjectTierExercisNumber = Instantiate(exerciseNumberText) as GameObject;

			
			TierSummaryExerciseTimeImage summaryTimeImage =
				gameObjectTierExerciseTime.GetComponent<TierSummaryExerciseTimeImage>();
			TierSummaryExerciseConfidenceImage summaryConfidenceImage =
				gameObjectTierExerciseConfidence.GetComponent<TierSummaryExerciseConfidenceImage>();
			TierSummaryExerciseAttemptImage summaryAttemptImage =
				gameObjectTierExerciseAttempts.GetComponent<TierSummaryExerciseAttemptImage>();
			
			
			summaryTimeImage.avgTimeText.text = exercise.userTime.ToString("F1");
			summaryTimeImage.GetComponent<Image>().fillAmount = exercise.userTime / UserDataObject.GetCurrentTierAllExercisesHighestTime();
			
			summaryConfidenceImage.avgConfidence.text = exercise.confidence.ToString("F1");
			summaryConfidenceImage.GetComponent<Image>().fillAmount = exercise.confidence/100;
			
			summaryAttemptImage.avgAttempt.text = exercise.attempts.ToString();
			summaryAttemptImage.GetComponent<Image>().fillAmount = exercise.attempts / UserDataObject.GetCurrentTierAllExercisesHighestAttempt();
			
			
			gameObjectTierExercisNumber.GetComponent<Text>().text += (_currentTierData.exercises.IndexOf(exercise)+1);
			
			gameObjectTierExerciseTime.transform.SetParent(timeSpacer, false);
			gameObjectTierExerciseConfidence.transform.SetParent(confidenceSpacer, false);
			gameObjectTierExerciseAttempts.transform.SetParent(attemptsSpacer, false);
			gameObjectTierExercisNumber.transform.SetParent(exerciseNumberSpacer, false);
		}
	}
}
