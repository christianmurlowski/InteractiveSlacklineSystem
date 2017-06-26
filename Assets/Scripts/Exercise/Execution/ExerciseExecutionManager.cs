using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseExecutionManager : MonoBehaviour
{
	private ExerciseData _currentExerciseData;
	private RepetitionData _currentRepetition;

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
	public Animation successAnimation;
	private float progress = 0.0f;
	private float _currentRepetitionConfidence = 0.0f;
	
	public Text testText; // TODO Just for test purposes -> Delete in production

	private int confidenceIterator;
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
		successAnimation = successPanel.GetComponent<Animation>();
		
		
		_toggleArray = new Toggle[_currentExerciseData.repetitions.Length];
		
		// Create and check toggles for each repetition of current exercise
		foreach (var repetition in _currentExerciseData.repetitions)
		{
			GameObject gameObjectToggle = Instantiate(toggle);
			Toggle currentToggle = gameObjectToggle.GetComponent<Toggle>();

			_toggleArray[Array.IndexOf(_currentExerciseData.repetitions, repetition)] = currentToggle;
			
			if (repetition.accomplished)
			{
				currentToggle.GetComponent<Toggle>().isOn = true;
				Debug.Log("IF");
			}
			else if (_currentRepetition == null)
			{
				Debug.Log("ELSE");
				_currentRepetition = repetition;
			}
			gameObjectToggle.transform.SetParent(toggleGroup, false);
		}
		
		PlayerPrefs.SetInt("CurrentRepetitionId", Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition));

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

		// Discrete Gesture tracking
		if (e.GestureType == GestureType.Discrete)
		{

			if (e.DetectionConfidence > 0.4f)
			{
				_durationManager.StartTimer();
				testText.text = "if DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
				
				confidenceIterator++;
				_currentRepetitionConfidence += e.DetectionConfidence * 100;
			}
			else
			{
				_durationManager.StopTimer();
				
				// if tracked time is greater than given time of the repetition
				if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime) 
				{
					ToggleAndCheckRepetition();
				}
				
				testText.text = "else DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
			}
//				testText.text = "DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
		}
		else if (e.GestureType == GestureType.Continuous) // TODO implement continous gestures
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
		Debug.Log("ACCOMPLISHED REPETITION");
		
		// Save the time and confidence for the current repetition
		_currentRepetition.userTime = _durationManager.GetlatestTimeInSeconds();
		_currentRepetition.confidence = _currentRepetitionConfidence / confidenceIterator;
		
		Debug.Log("time: " + _currentRepetition.userTime + " || confidence: " +  _currentRepetition.confidence);
		
		// toggle current repetition
		_currentRepetition.accomplished = true;
		_toggleArray[Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition)].isOn = true;
		
		// Check if last repetition
		if (_currentRepetition == _currentExerciseData.repetitions.Last())
		{
			// Exercise completed and accomplished
			_currentExerciseData.accomplished = true;
			
			// Unlock next exercise
			UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId") + 1].isInteractable = true;
			UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId") + 1].unlocked = 1;
			
			// Load the summary scene
			LoadSummaryScene();
		}
		else
		{
			//  If not last rep --> next rep is currentrepetition
			_currentRepetition =
				_currentExerciseData.repetitions[Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition) + 1];
			PlayerPrefs.SetInt("CurrentRepetitionId", Array.IndexOf(_currentExerciseData.repetitions, _currentRepetition));
		}

		// Save data to user json file
		SaveCurrentExerciseData();
		
		_currentRepetitionConfidence = 0;				
		confidenceIterator = 0;
		_durationManager.ResetlatestTimeInSeconds();
	}

//	
//	IEnumerator SuccessFadeIn()
//	{
//		float time = 4f;
//
//		while (successPanel.alpha < 1)
//		{
//			successPanel.alpha += Time.deltaTime / time;
////			Debug.Log("while> " + successPanel.alpha);
//			yield return new WaitForSeconds(0.01f);
//
//		}
//		
//		Debug.Log("SuccessFadeIn");
////		for (float f = 1f;  f >= 0 ; f -= 0.1f)
////		{
////			successPanel.alpha += 0.1f;
////			yield return new WaitForSeconds(0.05f);
////		}
//		yield return null;
//	}
//
//	private void StartSuccessAnimation()
//	{
//		successAnimation.Play();
//
//		if (!successAnimation.isPlaying)
//		{
//			
//		}
//	}
//	
//	IEnumerator SuccessFadeOut()
//	{
//		Debug.Log("SuccessFadeOut");
//		for (float f = 5f;  f >= 0 ; f -= 0.1f)
//		{
//			successPanel.alpha -= 0.1f;
//			yield return new WaitForSeconds(0.05f);
//		}
//		yield return null;
//	}
//	
//	public void StopTracking()
//	{
//		foreach (var gesture in _gestureDetectorList)
//		{
////			gesture.IsPaused = true;
//		}
//	}    
//    
//	public void StartTracking()
//	{
//		foreach (var gesture in _gestureDetectorList)
//		{
////			gesture.IsPaused = false;
//		}
//	}
//	
	private void LoadSummaryScene()
	{	
		SceneManager.LoadScene("ExerciseSummary");
	}
	
	// TODO CHECK Adjust it, call it from an extra json saving class
	public void SaveCurrentExerciseData()
	{        
		string currentUsersFilePath = PlayerPrefs.GetString("CurrentUserFilePath");
		
		UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")] = _currentExerciseData;
		
		string currentExerciseDataAsJson = JsonUtility.ToJson(UserDataObject.currentUser);
		File.WriteAllText(currentUsersFilePath, currentExerciseDataAsJson);
	}
}
