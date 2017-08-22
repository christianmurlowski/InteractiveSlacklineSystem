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
	public GameObject progressGroup;
	public Image durationImage;
	public Text counterText;
	public Slider slider;
	
	private Color32 red, darkOrange, lightOrange, green;
	
	private Stopwatch _attemptExecutionTime,
					  _attemptOverallTime;
	
	private ExerciseData _currentExerciseData;

	private float tempTimer,
				  _lastAttemptExecutionTime = 0f;

	private int _attemptExecutionTimeInt;
	
	void Start ()
	{
		// -----------------------------------------
		// ------------ INITIALIZATIONS ------------
		// -----------------------------------------
		
		// Reference to exercise data of current user
//		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		_currentExerciseData = UserDataObject.GetCurrentExercise();
		
		// Todo no functionality yet --> track overall time for exercise or overall time needed for rep
		tempTimer = 0f;		
		
		
		// -----------------------------------------
		// ------------- UI COMPONENTS -------------
		// -----------------------------------------
		
		// Execution time
		_attemptExecutionTime = new Stopwatch();
		
		// Duration donut
		durationImage.GetComponent<Image>();
		counterText.GetComponent<Text>();
		
		durationImage.type = Image.Type.Filled;
		durationImage.fillMethod = Image.FillMethod.Radial360;
		durationImage.fillAmount = 0f;
		
		// Colors for duration donut
		red = new Color32(227, 63, 34, 255);
		darkOrange = new Color32(227, 126, 34, 255);
		lightOrange = new Color32(231, 201, 80, 255);
		green = new Color32(91, 175, 76, 255);
		
		
		// Slider for progress
		slider = slider.GetComponent<Slider>();		
		Debug.Log("isProgressGesture: " + _currentExerciseData.isProgressGesture);
//		if (!_currentExerciseData.isProgressGesture)
//		{
//			progressGroup.SetActive(false);
//		}
			
	}

	public void StartTimer()
	{
		// Check if timer is running
		if (!_attemptExecutionTime.IsRunning)
		{
			_attemptExecutionTime.Start();		
		}
		
		_attemptExecutionTimeInt = Mathf.FloorToInt(_attemptExecutionTime.ElapsedMilliseconds * 0.001f);
		_lastAttemptExecutionTime = _attemptExecutionTime.ElapsedMilliseconds * 0.001f;

		// Update timer within progress circle
		counterText.text = _attemptExecutionTimeInt.ToString();

		// Update progress circle
		durationImage.fillAmount = _attemptExecutionTime.ElapsedMilliseconds * 0.001f / 
		                           (UserDataObject.GetCurrentRepetition().minTime);

		if (durationImage.fillAmount >= 1.0)
		{
			ChangeColor(green);
		}
		else if (durationImage.fillAmount > 0.66)
		{
			ChangeColor(lightOrange);
		}
		else if (durationImage.fillAmount > 0.33)
		{
			ChangeColor(darkOrange);
		}
	}
	
	public void StopTimer()
	{
//		_userExerciseData.repetitions
		
		_attemptExecutionTime.Reset();
		counterText.text = "0";
		durationImage.fillAmount = 0.0f;
		ChangeColor(red);
	}

	public bool IsTimerRunning()
	{
		return _attemptExecutionTime.IsRunning;
	}
	
	public void ChangeColor(Color32 color)
	{
		durationImage.color = color;
		counterText.color = color;
	}

	public float GetlatestTimeInSeconds()
	{
		return _lastAttemptExecutionTime;
	}
	public void ResetlatestTimeInSeconds()
	{
		_lastAttemptExecutionTime = 0f;
	}

	public void SetProgress(float progress)
	{
		slider.value = Mathf.Lerp(0, 1, progress);
	}

	public void ResetProgress()
	{
		slider.value = 0;
	}
}
