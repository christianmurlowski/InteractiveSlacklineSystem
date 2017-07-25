using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierInfoManager : MonoBehaviour
{

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
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		_currentTierData = UserDataObject.GetCurrentTier();
		
		tierNameText.text = _currentTierData.tierName.ToUpper();
		
		FillGoalList();
	}

	public void LoadNextScene()
	{
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise());
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise().isInteractable);
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise().unlocked);
		
		
		if (!UserDataObject.GetFirstTierExercise().isInteractable)
		{
			Debug.Log("HELLOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
			UserDataObject.GetFirstTierExercise().isInteractable = true;
			UserDataObject.GetFirstTierExercise().unlocked = 1;	
		}
		
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise());
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise().isInteractable);
		Debug.Log("TIERINFO: " + UserDataObject.GetFirstTierExercise().unlocked);
		
		SceneManager.LoadScene("MainMenu");
	}
	
	void FillGoalList()
	{
		foreach (var goal in _currentTierData.goals)
		{
			GameObject gameObjectTierPanel = Instantiate(tierGoalPanel) as GameObject;
			GoalPanel goalPanel = gameObjectTierPanel.GetComponent<GoalPanel>();

			goalPanel.goalDescription.text = goal.description;
			
			gameObjectTierPanel.transform.SetParent(spacer, false);
		}
	}
}
