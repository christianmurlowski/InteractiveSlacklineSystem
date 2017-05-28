using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	private int score = 5000;
	private int LevelAmount = 7;
	private int CurrentLevel;
	
	// Use this for initialization
	void Start () {
		
		LevelAmount = PlayerPrefs.GetInt("LevelAmount");

		CheckCurrentLevel();
		StartCoroutine(Time());
	}

	IEnumerator Time()
	{
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("MainMenu");
	}
	
	// Check current level
	void CheckCurrentLevel()
	{
		for (int i = 0; i < LevelAmount; i++)
		{
			if (SceneManager.GetActiveScene().name == "Level" + i)
			{
				CurrentLevel = i;
				SaveMyGame();
			}
		}
	}

	// Unlock next level and current Level successfully finished
	void SaveMyGame()
	{
		int NextLevel = CurrentLevel + 1;
		// If not last level
		if (NextLevel < LevelAmount)
		{
			PlayerPrefs.SetInt("Level" + NextLevel, 1); // unlock next Level
			PlayerPrefs.SetInt("Level" + CurrentLevel + "_score", score);
		}
		else
		{
			PlayerPrefs.SetInt("Level" + CurrentLevel + "_score", score);			
		}
	}
	
}
