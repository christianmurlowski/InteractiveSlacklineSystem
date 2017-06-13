using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseInfoManager : MonoBehaviour
{

	public GameObject exerciseTipPanel;
	public Transform spacer;
	
	private ExerciseData _currentExercise;
	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser();
		_currentExercise = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		// TODO Just for test purposes -> Delete in production
		_currentExercise = UserDataObject.currentUser.exerciseData[0];

		FillTipList();
	}


	void FillTipList()
	{
		foreach (var tip in _currentExercise.tips)
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipName.text = tip.name;
			tipPanel.tipDescription.text = tip.description;
//			tipPanel.tipImage = tip.image; TODO add image reference
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
/*
	TODO
	- create default images
	+ add per tip an image + name + desription text
	+ add it to spacer
	- load preview videofile into content
*/

}
