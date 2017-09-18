using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandPushManager : MonoBehaviour {

	public void LoadNextScene()
	{
		SceneManager.LoadScene("TutHandClick");
	}
}
