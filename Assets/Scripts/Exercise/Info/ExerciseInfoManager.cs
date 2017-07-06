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
	
	private ExerciseData _currentExercise;
	
	// Use this for initialization
	void Start ()
	{
//		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
//		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));

		_currentExercise = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		exerciseName.text = _currentExercise.exerciseName.ToUpper();
		
		FillTipList();
	}

	public void LoadNextScene()
	{
		SceneManager.LoadSceneAsync("ExerciseExecution");
	}
	public void LoadPreviousScene()
	{
		SceneManager.LoadSceneAsync("MainMenu");
	}
	
	void FillTipList()
	{
		foreach (var tip in _currentExercise.tips)
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipName.text = tip.name;
			tipPanel.tipDescription.text = tip.description;
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
	}
}
