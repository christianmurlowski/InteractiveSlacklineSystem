using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	private int _score = 5000;
	private int _levelAmount = 7;
	private int _currentLevel;
	
	// Use this for initialization
	void Start () {
		
		_levelAmount = PlayerPrefs.GetInt("_levelAmount");
//		Debug.Log("AMOUNT:" + LevelManager.LevelList.Count);
		CheckCurrentLevel();
		StartCoroutine(Time());
	}

	IEnumerator Time()
	{
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene("MainMenu");
	}
	
	// Updates current level and get
	void CheckCurrentLevel()
	{
		for (int i = 0; i < _levelAmount; i++)
		{
			if (SceneManager.GetActiveScene().name == "Level" + i)
			{
				_currentLevel = i;
				SaveMyGame();
			}
		}
	}

	// Unlock next level and current Level successfully finished
	void SaveMyGame()
	{
		int NextLevel = _currentLevel + 1;
		// If not last level
		if (NextLevel < _levelAmount)
		{
			PlayerPrefs.SetInt("Level" + NextLevel, 1); // unlock next Level
			PlayerPrefs.SetInt("Level" + _currentLevel + "_score", _score);
		}
		else
		{
			PlayerPrefs.SetInt("Level" + _currentLevel + "_score", _score);			
		}
	}
	
}
