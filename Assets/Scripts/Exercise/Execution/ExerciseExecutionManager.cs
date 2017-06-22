using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseExecutionManager : MonoBehaviour
{
	private ExerciseData _currentExerciseData;
	public RepetitionData _currentRepetition;

	public Text titleText;

	public GameObject bodyManager;
	private BodyManager _bodyManager;
	private KinectSensor _kinectSensor;
	private Body[] _bodies = null;

	public GameObject durationManager;
	private DurationManager _durationManager;

	private List<GestureDetector> _gestureDetectorList = null;

	public GameObject toggle;
	public Transform toggleGroup;
	private Toggle[] _toggleArray;

	public CanvasGroup successPanel;
	
	public Text testText;
	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentExerciseId", 0);// TODO Just for test purposes -> Delete in production
		
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));
		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		titleText.text = _currentExerciseData.exerciseName.ToUpper();
				
		_durationManager = durationManager.GetComponent<DurationManager>();

		successPanel = successPanel.GetComponent<CanvasGroup>();
		
		_toggleArray = new Toggle[_currentExerciseData.repetitions.Length];

		bool lastAccomplished = false;
		// Create and check toggles for each repetition of current exercise
		foreach (var repetition in _currentExerciseData.repetitions)
		{
			GameObject gameObjectToggle = Instantiate(toggle);
			Toggle currentToggle = gameObjectToggle.GetComponent<Toggle>();

			_toggleArray[Array.IndexOf(_currentExerciseData.repetitions, repetition)] = currentToggle;

			if (repetition == _currentExerciseData.repetitions[0] && !repetition.accomplished || lastAccomplished == true)
			{
				_currentRepetition = repetition;
				lastAccomplished = false;
			}
			else if (repetition.accomplished)
			{
				lastAccomplished = true;
				currentToggle.GetComponent<Toggle>().isOn = true;
			}
			
			gameObjectToggle.transform.SetParent(toggleGroup, false);
			Debug.Log(_currentRepetition.minTime);
		}
		
		PlayerPrefs.SetInt("CurrentRepetition", Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition));// TODO Just for test purposes -> Delete in production

		_bodyManager = bodyManager.GetComponent<BodyManager>();
		if (_bodyManager == null)
		{
			return;
		}

		_kinectSensor = _bodyManager.GetSensor();

		_bodies = _bodyManager.GetBodies();
		Debug.Log(_bodyManager + " | " + _bodies);
		
		_gestureDetectorList = new List<GestureDetector>();
		
		for (int bodyIndex = 0; bodyIndex < _bodies.Length; bodyIndex++)
		{
			_gestureDetectorList.Add(new GestureDetector(_kinectSensor));
		}
	}

	// Update is called once per frame
	void Update () {

		for (int bodyIndex = 0; bodyIndex < _bodies.Length; bodyIndex++)
		{
			var body = _bodies[bodyIndex];

			if (body != null)
			{
				var trackingId = body.TrackingId;

				if (trackingId != _gestureDetectorList[bodyIndex].TrackingId)
				{
					_gestureDetectorList[bodyIndex].TrackingId = trackingId;

					_gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
					_gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureDetected(bodyIndex);
				}
			}
		}
	}

	private EventHandler<GestureEventArgs> CreateOnGestureDetected(int bodyIndex)
	{
		return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
	}
	
	private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
	{
		var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;

		if (e.GestureType == GestureType.Discrete)
		{
			if (e.DetectionConfidence > 0.4f)
			{
				_durationManager.StartTimer();
				testText.text = "if DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
			}
			else
			{
				Debug.Log("CONFIDENCE: " + e.DetectionConfidence);
				_durationManager.StopTimer();
				
				// if tracked time is higher than given time o the repetition
				
				
				if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime) 
				{
					// TODO Make a curoutine to check and toggle
					ToggleAndCheckRepetition();
				}
				
				testText.text = "else DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
			}
//				testText.text = "DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
		}
		else if (e.GestureType == GestureType.Continuous)
		{
			if (e.Progress > 0.4f)
			{
				// todo fill progressbar until 0.8 is reached and start time
			}
			testText.text = "CONTINUOUS: " + e.Progress;
		}
		else
		{
			_durationManager.StopTimer();
		}
	}

	private void ToggleAndCheckRepetition()
	{
		_durationManager.ResetlatestTimeInSeconds();
		
		Debug.Log("ACCOMPLISHED REPETITION");
		
		// Stop the current tracking
		StopTracking();
		Debug.Log("_currentExerciseData.repetitions[0].accomplished: " + _currentExerciseData.repetitions[0].accomplished);

		// TODO toggle current repetition
		_currentRepetition.accomplished = true;
		Debug.Log("_currentExerciseData.repetitions[0].accomplished: " + _currentExerciseData.repetitions[0].accomplished);
		_toggleArray[Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition)].isOn = true;
		
		StartCoroutine("SuccessFadeIn");

		// TODO Save to json file
		SaveCurrentExerciseData();
	}

	IEnumerator SuccessFadeIn()
	{	
		Debug.Log("SuccessFadeIn");
		for (float f = 1f;  f >= 0 ; f -= 0.1f)
		{
			successPanel.alpha += 0.1f;
			yield return new WaitForSeconds(0.05f);
		}

		if (_currentRepetition == _currentExerciseData.repetitions.Last())
		{
			// TODO Exercise completed
			
			// TODO save to user json file --> currentexercisedata is completed
			FinishExercise();
		}
		else
		{
			StartCoroutine("SuccessFadeOut");
			StartTracking();
			// TODO If not then next repetition is current repetition and return to execution
			_currentRepetition =
				_currentExerciseData.repetitions[Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition) + 1];
			PlayerPrefs.SetInt("CurrentRepetition", Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition));
		}
		
		
		yield return null;
	}
	
	IEnumerator SuccessFadeOut()
	{
		Debug.Log("SuccessFadeOut");
		for (float f = 5f;  f >= 0 ; f -= 0.1f)
		{
			successPanel.alpha -= 0.1f;
			yield return new WaitForSeconds(0.05f);
		}
		yield return null;
	}
	
	public void StopTracking()
	{
		foreach (var gesture in _gestureDetectorList)
		{
//			gesture.IsPaused = true;
		}
	}    
    
	public void StartTracking()
	{
		foreach (var gesture in _gestureDetectorList)
		{
//			gesture.IsPaused = false;
		}
	}
	private void FinishExercise()
	{
		// TODO Load summary scene and show stats and then load main menu
	}
	
	// TODO Just for test purposes -> Delete in production
	public void SaveCurrentExerciseData()
	{        
        
		string currentUsersFilePath = PlayerPrefs.GetString("CurrentUserFilePath");
		Debug.Log(currentUsersFilePath);
		
		Debug.Log(UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")].exerciseName);
		Debug.Log(_currentExerciseData.exerciseName);
		
		
		Debug.Log("SaveCurrentExerciseData: " + UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")].repetitions[0].accomplished);
		UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")] = _currentExerciseData;
		Debug.Log("SaveCurrentExerciseData: " + UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")].repetitions[0].accomplished);

		
		string currentExerciseDataAsJson = JsonUtility.ToJson(UserDataObject.currentUser);
		File.WriteAllText(currentUsersFilePath, currentExerciseDataAsJson);
		

		
	}
}
