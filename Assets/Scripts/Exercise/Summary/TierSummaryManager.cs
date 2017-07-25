using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierSummaryManager : MonoBehaviour
{

	[Tooltip("Prefab of goal panel")]
	public Transform timeSpacer, attemptsSpacer, confidenceSpacer;

	public GameObject exerciseTimeImage, exerciseAttemptsImage, exerciseConfidenceImage;

	public Text tierNameText;
	
	private TierData _currentTierData;
	
	// Use this for initialization
	void Start ()
	{
		// TODO Just for test purposes -> Delete in production

		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentTierId", 0);// TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production
		
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		_currentTierData = UserDataObject.GetCurrentTier();
		
		tierNameText.text = UserDataObject.GetCurrentTier().tierName + " average data";
		
		FillSummaryList();
	}

	public void LoadNextScene()
	{
		if (!UserDataObject.GetCurrentExercise().isInteractable)
		{
			UserDataObject.GetCurrentExercise().isInteractable = true;
			UserDataObject.GetCurrentExercise().unlocked = 1;	
		}
		SceneManager.LoadScene("MainMenu");
	}
	
	void FillSummaryList()
	{
		Debug.Log(_currentTierData.fileName);
		Debug.Log(_currentTierData.goals);
		Debug.Log(_currentTierData.goals[1]);
		
		foreach (var exercise in _currentTierData.exercises)
		{
			GameObject gameObjectTierExerciseTime = Instantiate(exerciseTimeImage) as GameObject;
			GameObject gameObjectTierExerciseConfidence = Instantiate(exerciseConfidenceImage) as GameObject;
			GameObject gameObjectTierExerciseAttempts = Instantiate(exerciseAttemptsImage) as GameObject;

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
			
			gameObjectTierExerciseTime.transform.SetParent(timeSpacer, false);
			gameObjectTierExerciseConfidence.transform.SetParent(confidenceSpacer, false);
			gameObjectTierExerciseAttempts.transform.SetParent(attemptsSpacer, false);
		}
	}
}
