﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetData : MonoBehaviour {

	void Awake()
	{
		// TODO Just for test purposes -> Delete in production
		UserSelectionManager.TestSetCurrentUser();
		PlayerPrefs.SetInt("CurrentTierId", 1);
		PlayerPrefs.SetInt("CurrentExerciseId", 4);
		PlayerPrefs.SetInt("CurrentSideId", 0);
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));
		Debug.Log("CurrentSideId: " + PlayerPrefs.GetInt("CurrentSideId"));
	}
}
