using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandClickManager : MonoBehaviour {
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
		SceneManager.LoadScene("TutStartingPosition");
	}
}
