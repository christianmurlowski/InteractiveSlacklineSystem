using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandPushManager : MonoBehaviour {
	public AudioSource audioSuccess;

	void Start()
	{
		audioSuccess.Play();
	}

	private void Update()
	{
		// If right arrow pressed --> start tutorial hand click
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			LoadNextScene();
		}
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene("TutHandClick");
	}
}
