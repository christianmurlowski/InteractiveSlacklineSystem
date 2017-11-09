using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierInfoManager : MonoBehaviour
{

	public GameObject tierGoalPanel,
					  tierInfoPanel;
	
	public Transform goalSpacer,
					 infoSpacer,
					 goalsPanel;

	public Text tierNameText;
	
	private TierData _currentTierData;
	
	// Use this for initialization
	void Start ()
	{
		_currentTierData = UserDataObject.GetCurrentTier();
		
		tierNameText.text = _currentTierData.tierName.ToUpper();
		
		FillGoalList();
	}

	private void Update()
	{
		// If right arrow pressed --> start exercise
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			LoadNextScene();
		}
	}

	public void LoadNextScene()
	{
		if (!UserDataObject.GetFirstTierExercise().isInteractable)
		{
			UserDataObject.GetFirstTierExercise().isInteractable = true;
			UserDataObject.GetFirstTierExercise().unlocked = 1;	
		}
		
		SceneManager.LoadScene("MainMenu");
	}	
	
	void FillGoalList()
	{
		foreach (var goal in _currentTierData.goals)
		{
			// adjust or add foreach to tip for general tips panel and goals for tier goals
			GameObject gameObjectTierGoalPanel = Instantiate(tierGoalPanel) as GameObject;
			GoalPanel goalPanel = gameObjectTierGoalPanel.GetComponent<GoalPanel>();	

			goalPanel.goalDescription.text = goal.description;

			gameObjectTierGoalPanel.transform.SetParent(goalSpacer, false);
		}
		
		foreach (var tip in _currentTierData.tips)
		{
			GameObject gameObjectTierInfoPanel = Instantiate(tierInfoPanel) as GameObject;
			InformationPanel InfoPanel = gameObjectTierInfoPanel.GetComponent<InformationPanel>();

			
			InfoPanel.infoDescription.text = tip.description;
			InfoPanel.infoNumber.text = (_currentTierData.tips.IndexOf(tip)+1).ToString();
			
			gameObjectTierInfoPanel.transform.SetParent(infoSpacer, false);
		}
		
	}
}
