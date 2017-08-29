using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandClickManager : MonoBehaviour {

	public void LoadNextScene()
	{
		SceneManager.LoadScene("TutStartingPosition");
	}
}
