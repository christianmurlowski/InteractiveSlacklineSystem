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
	private float progress = 0.0f;
	private float _currentRepetitionConfidence = 0.0f;
	
	public Text testText; // TODO Just for test purposes -> Delete in production

	private int confidenceIterator; // for calculating average confidence

	private bool _firstCheckpoint,
				_secondCheckpoint,
				_thirdCheckpoint;
	
	private List<float> pufferList = new List<float>();

	public int sideAccomplishedCounter;
	
	private GameObject _kinectManager;

	private bool strartTrackingAgain;

	// Use this for initialization
	void Start ()
	{
		// -----------------------------------------
		// ------------ INITIALIZATIONS ------------
		// -----------------------------------------
		
		// Disable handcursor in execution mode TODO all disable in testmode, enable in production
		bodyManager = GameObject.Find("BodyManager");
		
//		_kinectManager = GameObject.Find("KinectManager");
//		if (_kinectManager.GetComponent<InteractionManager>())
//		{
//			_kinectManager.GetComponent<InteractionManager>().showHandCursor = false;
//		}
//		
		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser();
//		PlayerPrefs.SetInt("CurrentTierId", 0);
//		PlayerPrefs.SetInt("CurrentExerciseId", 2);
//		PlayerPrefs.SetInt("CurrentSideId", 0);
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));
		
		// Reference to exercise data of current user
//		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		_currentExerciseData = UserDataObject.GetCurrentExercise();
		
		// Duration manager
		_durationManager = durationManager.GetComponent<DurationManager>();

		
		// -----------------------------------------
		// ------------- UI COMPONENTS -------------
		// -----------------------------------------
		
		// Exercise name
		titleText.text = UserDataObject.GetCurrentExerciseAndSideName().ToUpper();
		successPanel = successPanel.GetComponent<CanvasGroup>(); // TODO implement success animation for rep
		
		// Array of Toggles
		_toggleArray = new Toggle[UserDataObject.GetCurrentRepetitionsArray().Length];
		
		// Create and check toggles for each rep of current exercise
		foreach (var repetition in UserDataObject.GetCurrentRepetitionsArray())
		{
			GameObject gameObjectToggle = Instantiate(toggle);
			Toggle currentToggle = gameObjectToggle.GetComponent<Toggle>();

			_toggleArray[Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), repetition)] = currentToggle;
			
			// Check if exercise not already accomplished
			if (!_currentExerciseData.accomplished)
			{
				// look for accomplished reps, check regarding toggles and set current rep
				if (repetition.accomplished)
				{
					currentToggle.GetComponent<Toggle>().isOn = true;
				}
				else if (_currentRepetition == null)
				{
					_currentRepetition = repetition;
				}			
			}
			else // If exercise already accomplished
			{	
				// Set first rep as current rep
				if (_currentRepetition == null)
				{
					_currentRepetition = repetition;
				}
			}
			// Append GO to group
			gameObjectToggle.transform.SetParent(toggleGroup, false);
		}
		
		// Set ID of current repetition
		PlayerPrefs.SetInt("CurrentRepetitionId", Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition));
		
		
		// -----------------------------------------
		// ---------------- KINECT -----------------
		// -----------------------------------------
		
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
					// Pause the gestures that have no id
					_gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
					_gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureDetected(bodyIndex);
				}
			}
		}
	}
	
	
	// -----------------------------------------
	// ----------- GESTURE DETECTION ----------- 
	// -----------------------------------------
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

			if (GestureDetected(e.DetectionConfidence, 0.4f, 1f))
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
		else if (e.GestureType == GestureType.Continuous)
		{
			_durationManager.SetProgress(e.Progress);

			if (_thirdCheckpoint)
			{
				_durationManager.StartTimer();

				if (e.Progress <= 0.4f)
				{
					_durationManager.StopTimer();
					
					_thirdCheckpoint = false;
					
					// if tracked time is greater than given time of the repetition
					if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime)
					{
						ToggleAndCheckRepetition();
					}
				}
			}
			else
			{
				if (_secondCheckpoint && GestureDetected(e.Progress, 0.7f, 1f))
				{
					_thirdCheckpoint = true;
				}
				else if (_firstCheckpoint && GestureDetected(e.Progress, 0.5f, 0.7f))
				{
					_secondCheckpoint = true;
				}
				else if (GestureDetected(e.Progress, 0.2f, 0.5f))
				{
					_firstCheckpoint = true;
				}
			}
			testText.text = "if CONTINUOUS: " + e.Progress;
		}
		else
		{
			testText.text = "else CONTINUOUS: " + e.Progress;

			_durationManager.StopTimer();
		}
	}
	
	
	// -----------------------------------------
	// ---------------- HELPER -----------------
	// -----------------------------------------
	private bool GestureDetected(float progress, float minGoal, float maxGoal)
	{
		pufferList.Add(progress);
		var success = false;

		if (pufferList.Count > 5)
		{
			pufferList.RemoveAt(0);
			foreach (var pufferValue in pufferList)
			{
				if (pufferValue >= minGoal &&  pufferValue <= maxGoal)
				{
					success = true;
				}
			}
		}
		
		return success;
	}
	
	private void ToggleAndCheckRepetition()
	{
		StopTracking();
		Debug.Log("ACCOMPLISHED REPETITION");
		
		// Save the time and confidence for the current repetition
		_currentRepetition.userTime = _durationManager.GetlatestTimeInSeconds();
		_currentRepetition.confidence = _currentRepetitionConfidence / confidenceIterator;
		
		Debug.Log("Repetition time: " + _currentRepetition.userTime + " || confidence: " +  _currentRepetition.confidence);
		
		// Toggle current repetition
		_currentRepetition.accomplished = true;
		_toggleArray[Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition)].isOn = true;
		
		// Check if last repetition
		if (_currentRepetition == UserDataObject.GetCurrentRepetitionsArray().Last())
		{
			// Exercise completed and accomplished
			_currentExerciseData.accomplished = true;
			UserDataObject.GetCurrentSide().accomplished = true;

			
			// set side average confidence and time regarding repetitions
			UserDataObject.GetCurrentSide().userTime = 0.0f;
			UserDataObject.GetCurrentSide().confidence = 0.0f;
			foreach (var repetition in UserDataObject.GetCurrentRepetitionsArray())
			{
				UserDataObject.GetCurrentSide().userTime += repetition.userTime;
				UserDataObject.GetCurrentSide().confidence += repetition.confidence;
			}

			UserDataObject.GetCurrentSide().userTime /= UserDataObject.GetCurrentRepetitionsArray().Length;
			UserDataObject.GetCurrentSide().confidence /= UserDataObject.GetCurrentRepetitionsArray().Length;
			
			
			// set exercise average confidence and time regarding sides
			sideAccomplishedCounter = 0;
			_currentExerciseData.confidence = 0.0f;
			_currentExerciseData.userTime = 0.0f;
			foreach (var side in UserDataObject.GetCurrentExercise().sides)
			{
				if (side.accomplished)
				{
					sideAccomplishedCounter++;
					_currentExerciseData.confidence += side.confidence;
					_currentExerciseData.userTime += side.userTime;
				}
			}
			// Average confidence and time of entire exercise (both sides)
			_currentExerciseData.confidence /= sideAccomplishedCounter;
			_currentExerciseData.userTime /= sideAccomplishedCounter;

			// If last exercise --> accomplish current tier
			if (PlayerPrefs.GetInt("CurrentExerciseId") == UserDataObject.GetCurrentTierErcisesLength() - 1)
			{
				// todo All exercises accomplished congratulations or so
				UserDataObject.GetCurrentTier().accomplished = true;
				
				// If last tier reached --> do nothing
				if (PlayerPrefs.GetInt("CurrentTierId") == UserDataObject.GetAllTiers().Count - 1)
				{
					
				}
				else // If not last tier --> unlock next tier
				{
					TierData nextTier = UserDataObject.GetNextTier();
					nextTier.isInteractable = true;
				}
			}
			else // If not last exercise --> unlock next exercise
			{
				ExerciseData nextExerciseData = UserDataObject.GetNextExercise();

				nextExerciseData.isInteractable = true;
				nextExerciseData.unlocked = 1;

//				UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId") + 1].isInteractable = true;
//				UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId") + 1].unlocked = 1;
			}
			
			//		 Save data to user json file
			SaveCurrentExerciseData();
			
			// Load the summary scene
			LoadSummaryScene();
		} // If not last repetition --> current repetition is next repetition
		else
		{
			_currentRepetition =
				UserDataObject.GetCurrentRepetitionsArray()[Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition) + 1];
			PlayerPrefs.SetInt("CurrentRepetitionId", Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition));
			strartTrackingAgain = true;
			Debug.Log("STARTTRAKCINGAGAIN" + strartTrackingAgain);
			//		 Save data to user json file
			SaveCurrentExerciseData();
		}
		
		Debug.Log("-----------------------");
		Debug.Log("TOGGLEANDSAVE");
		Debug.Log("-----------------------");

		// Reset all variables needed for next repetition
		_currentRepetitionConfidence = 0;				
		confidenceIterator = 0;
		
		_durationManager.ResetlatestTimeInSeconds();
		_durationManager.ResetProgress();
		
		_firstCheckpoint = false;
		_secondCheckpoint = false;
		
		pufferList.Clear();

		if (strartTrackingAgain)
		{
			StartCoroutine("StartTracking");
		}
	}

	public void StopTracking()
	{
		strartTrackingAgain = false;
		foreach (var gesture in _gestureDetectorList)
		{
			// Pause the current gesture with an id
			if (gesture.TrackingId != 0)
			{		
				gesture.IsPaused = true;
			}
		}
		Debug.Log("STOP UPDATE");
		enabled = false;

		Debug.Log("STOP TRACKING");

	}    
    
	IEnumerator StartTracking()
	{
		yield return new WaitForSeconds(1f);

		foreach (var gesture in _gestureDetectorList)
		{
			// Start the current gesture with an id --> not all because the should be paused like in update function
			if (gesture.TrackingId != 0)
			{		
				gesture.IsPaused = false;
			}
		}
		Debug.Log("START UPDATE");
		enabled = true;
		Debug.Log("START TRACKING");
	}

	private void DisposeGestures()
	{
		
		// Dispose gesture
		Debug.Log("Dispose gesturedetector");
		foreach (var gesture in _gestureDetectorList)
		{
			gesture.Dispose();
		}
//		_gestureDetectorList = null;
	}

	private void LoadSummaryScene()
	{
		DisposeGestures();
//		DisposeBodyManager();
		SceneManager.LoadScene("ExerciseSummary");
	}
	
	// TODO CHECK Adjust it, call it from an extra json saving class
	public void SaveCurrentExerciseData()
	{        
		string currentUsersFilePath = PlayerPrefs.GetString("CurrentUserFilePath");
		
// TODO	check if needed	because of reference
//		UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")] = _currentExerciseData;
		
		string currentExerciseDataAsJson = JsonUtility.ToJson(UserDataObject.currentUser);
		File.WriteAllText(currentUsersFilePath, currentExerciseDataAsJson);
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
}
