using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierSummaryManager : MonoBehaviour
{

	[Tooltip("Prefab of goal panel")]
	public GameObject tierGoalPanel;
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
		Debug.Log(_currentTierData.levelName);
		Debug.Log(_currentTierData.goals);
		Debug.Log(_currentTierData.goals[1]);
		foreach (var goal in _currentTierData.goals)
		{
			GameObject gameObjectTierPanel = Instantiate(tierGoalPanel) as GameObject;
			GoalPanel goalPanel = gameObjectTierPanel.GetComponent<GoalPanel>();

			goalPanel.goalDescription.text = goal.description;
			
			gameObjectTierPanel.transform.SetParent(spacer, false);
		}
	}
}
