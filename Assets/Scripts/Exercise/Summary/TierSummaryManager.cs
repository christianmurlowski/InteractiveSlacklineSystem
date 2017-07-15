using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierSummaryManager : MonoBehaviour
{

	[Tooltip("Prefab of goal panel")]
	public GameObject exercisePanelGameObject;
	public Transform spacer;

	public Text tierNameText;
	
	private TierData _currentTierData;
	
	// Use this for initialization
	void Start ()
	{
		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser(); 
//		PlayerPrefs.SetInt("CurrentTierId", 0);
//		PlayerPrefs.SetInt("CurrentExerciseId", 3);
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		_currentTierData = UserDataObject.GetCurrentTier();
		
		tierNameText.text = _currentTierData.tierName.ToUpper();
		
		FillGoalList();
	}

	public void LoadNextScene()
	{
		if (!UserDataObject.GetCurrentExercise().isInteractable)
		{
			UserDataObject.GetCurrentExercise().isInteractable = true;
			UserDataObject.GetCurrentExercise().unlocked = 1;	
		}
		SceneManager.LoadSceneAsync("MainMenu");
	}
	
	void FillGoalList()
	{
		Debug.Log(_currentTierData.fileName);
		Debug.Log(_currentTierData.goals);
		Debug.Log(_currentTierData.goals[1]);
		foreach (var exercise in _currentTierData.exercises)
		{
			GameObject gameObjectTierExercisePanel = Instantiate(exercisePanelGameObject) as GameObject;
			TierSummaryExercisePanel summaryExercisePanel = gameObjectTierExercisePanel.GetComponent<TierSummaryExercisePanel>();

			summaryExercisePanel.tierSummaryExerciseName.text = exercise.exerciseName;
			summaryExercisePanel.averageConfidence.text = "Avg Confidence: " + exercise.confidence.ToString();
			summaryExercisePanel.averageTime.text = "Avg Time: " + exercise.userTime.ToString();
			
			gameObjectTierExercisePanel.transform.SetParent(spacer, false);
		}
	}
}
