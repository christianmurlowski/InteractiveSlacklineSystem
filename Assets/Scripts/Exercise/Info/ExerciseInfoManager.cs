using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseInfoManager : MonoBehaviour
{

	public GameObject exerciseTipPanel;
	
	public Transform spacer;

	public Text exerciseName;
	
	// Use this for initialization
	void Start ()
	{
//		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser(); 
//		PlayerPrefs.SetInt("CurrentTierId", 0);
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);
//		PlayerPrefs.SetInt("CurrentSideId", 0);
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		exerciseName.text = UserDataObject.GetCurrentExerciseAndSideName().ToUpper();
		
		FillTipList();
	}

	public void LoadNextScene()
	{
		SceneManager.LoadSceneAsync("ExerciseExecution");
	}
	public void LoadPreviousScene()
	{
		SceneManager.LoadSceneAsync("ExerciseSideSelection");
	}
	
	void FillTipList()
	{
		foreach (var tip in UserDataObject.GetCurrentTipsArray())
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipName.text = tip.name;
			tipPanel.tipDescription.text = tip.description;
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
	}
}
