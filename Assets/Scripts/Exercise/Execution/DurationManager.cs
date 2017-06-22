using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class DurationManager : MonoBehaviour
{

	public Image durationImage;
	public Text counterText;
	
	private float tempTimer;

	private Color32 red, darkOrange, lightOrange, green;

	private Stopwatch _attemptExecutionTime;
	private int _lastAttemptExecutionTime = 0;
	private Stopwatch _attemptOverallTime;
	private int _attemptExecutionTimeInt;
	
	private ExerciseData _currentExerciseData;


//	private ExerciseExecutionManager exMan;
	// TODO fillring
	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production
		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		durationImage.GetComponent<Image>();
		counterText.GetComponent<Text>();
		
		durationImage.type = Image.Type.Filled;
		durationImage.fillMethod = Image.FillMethod.Radial360;
		durationImage.fillAmount = 0f;
		tempTimer = 0f;		
		
		// Colors
		red = new Color32(227, 63, 34, 255);
		darkOrange = new Color32(227, 126, 34, 255);
		lightOrange = new Color32(231, 201, 80, 255);
		green = new Color32(91, 175, 76, 255);
		
		_attemptExecutionTime = new Stopwatch();
//		exMan = new GameObject().AddComponent<ExerciseExecutionManager>();
//		Debug.Log(" exMan._currentRepetition.minTime: " + exMan._currentRepetition.minTime);
	}

	public void StartTimer()
	{
		if (!_attemptExecutionTime.IsRunning)
		{
			_attemptExecutionTime.Start();		
		}
		_attemptExecutionTimeInt = Mathf.RoundToInt(_attemptExecutionTime.ElapsedMilliseconds * 0.001f);
		_lastAttemptExecutionTime = _attemptExecutionTimeInt;

		counterText.text = _attemptExecutionTimeInt.ToString();

		durationImage.fillAmount = _attemptExecutionTime.ElapsedMilliseconds * 0.001f / _currentExerciseData.repetitions[PlayerPrefs.GetInt("CurrentRepetition")].minTime;

		if (durationImage.fillAmount >= 1.0)
		{
			changeColor(green);
		}
		else if (durationImage.fillAmount > 0.66)
		{
			changeColor(lightOrange);
		}
		else if (durationImage.fillAmount > 0.33)
		{
			changeColor(darkOrange);
		}
	}

	public void StopTimer()
	{
//		_userExerciseData.repetitions
		
		_attemptExecutionTime.Reset();
		counterText.text = "0";
		durationImage.fillAmount = 0.0f;
		changeColor(red);
	}
	
	public void changeColor(Color32 color)
	{
		durationImage.color = color;
		counterText.color = color;
	}

	public int GetlatestTimeInSeconds()
	{
		return _lastAttemptExecutionTime;
	}
	public void ResetlatestTimeInSeconds()
	{
		_lastAttemptExecutionTime = 0;
	}

// Update is called once per frame
	void Update () {
//		StartTimer();		
	}
}
