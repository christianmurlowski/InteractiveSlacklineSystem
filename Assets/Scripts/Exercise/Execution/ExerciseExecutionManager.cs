﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioSource = UnityEngine.AudioSource;

public class ExerciseExecutionManager : MonoBehaviour
{
	public Transform SuccessGroupTransform;
	public AudioSource audioSuccess,
					   audioFail;
	public CanvasGroup successPanel;
	public GameObject CanvasHandCursor,
				  	  KinectManager, 
					  ExerciseExecutionValidationManager, 
	  				  bodyManager, 
					  durationManager,
					  toggle,
					  checkItem;
	public Transform toggleGroup,
					 checkSpacer;
	public Text titleText,
				standingLegText,
				successMainText,
        		successSubText,
				testText, // TODO Just for test purposes -> Delete in production
				rightFootHeightText,
				leftFootHeightText,
				rightFootDepthText,
				leftFootDepthText,
				initialRightFootText,
				initialLeftFootText,
				bothFeetUpText,
				textReps,
				armsUpText;
	
	// Kinect stuff
	private Body[] _bodies = null;
	private BodyManager _bodyManager;
	private DurationManager _durationManager;
	private ExerciseData _currentExerciseData,
						 _lastExercise;
	private KinectManager _kinectManager;
	private InteractionManager _interactionManager;
	private KinectInterop.JointType _jointFootRight,
									_jointFootLeft;
	private KinectSensor _kinectSensor;
	private List<GestureDetector> _gestureDetectorList = null;
	private RepetitionData _currentRepetition;
	private ExerciseExecutionValidationManager _exerciseExecutionValidationManager;

	// Unity
	private Toggle[] _toggleArray;
	private Toggle[] _toggleCheckArray;

	private List<float> pufferList = new List<float>();
	private List<CheckItem> checkItemList = new List<CheckItem>();
	private float progress = 0.0f,
				  _currentRepetitionConfidence = 0.0f,
				  _initialStartingHeightLeft = 0.0f,
				  _initialStartingHeightRight = 0.0f,
				  _startingHeightDifference = 0.0f,
				  _footDepthTolerance = 0.25f,
				  _gestureAccuracy,
				  _progressMinConfidence = 0.7f;
	private int confidenceIterator, 
				attemptsIterator,
				sideAccomplishedCounter, // for calculating average confidence
				heightTestIterator,
				_repsIterator;
	
	private bool _checksPassed = true, 
				_firstCheckpoint,
				_secondCheckpoint,
				_thirdCheckpoint,
				startTrackingAgain,
				_bothFeetUp,
				_inStartingPosition,
				_minTimeAlreadyReached,
				_sideNotAccomplished;

	private bool[] _methodCheckedArray;
	private float time = 0.0f;
	private float interpolationPeriod = 0.5f;
	
	// Use this for initialization
	void Start ()
	{
		// -----------------------------------------
		// ------------ INITIALIZATIONS ------------
		// -----------------------------------------
		
		CanvasHandCursor = GameObject.Find("CanvasHandCursor");	
		
		// Disable handcursor in execution mode TODO all disable in testmode, enable in production
		bodyManager = GameObject.Find("BodyManager");
		
		KinectManager = GameObject.Find("KinectManager");
		if (KinectManager == null)
		{
			return;
		}

		_kinectManager = KinectManager.GetComponent<KinectManager>();
		_interactionManager = _kinectManager.GetComponent<InteractionManager>();

		if (CanvasHandCursor.activeSelf)
		{
			_interactionManager.enabled = false;
			CanvasHandCursor.gameObject.SetActive(false);
		}
		
		Debug.Log("IsUserDetected: " + _kinectManager.IsUserDetected());
		
		
		_exerciseExecutionValidationManager = ExerciseExecutionValidationManager.GetComponent<ExerciseExecutionValidationManager>();
		
		// Reference to exercise data of current user
//		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		_currentExerciseData = UserDataObject.GetCurrentExercise();
		_lastExercise = UserDataObject.GetLastTierExercise();
		
		// Duration manager
		_durationManager = durationManager.GetComponent<DurationManager>();
		
		_minTimeAlreadyReached = false;
		_repsIterator = 0;
		
		_methodCheckedArray = new bool[UserDataObject.GetCurrentChecksArray().Length];
		
		// -----------------------------------------
		// ------------- UI COMPONENTS -------------
		// -----------------------------------------
		
		// Exercise name
		titleText.text = UserDataObject.GetCurrentExerciseName().ToUpper();
		standingLegText.text = UserDataObject.GetCurrentSide().direction;
		successPanel = successPanel.GetComponent<CanvasGroup>(); // TODO implement success animation for rep
		
		foreach (var check in UserDataObject.GetCurrentChecksArray())
		{
			GameObject gameObjectCheckItem = Instantiate(checkItem);
			CheckItem currentCheckItem = gameObjectCheckItem.GetComponent<CheckItem>();

			currentCheckItem.methodName = check.methodName;
			currentCheckItem.description.text = check.description;
			checkItemList.Add(currentCheckItem);

			gameObjectCheckItem.transform.SetParent(checkSpacer, false);
		}

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
					_repsIterator += 1;
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
					repetition.attempts = 0;
					repetition.confidence = 0.0f;
					repetition.userTime = 0.0f;
					
					_currentRepetition = repetition;
				}
			}
			// Append GO to group
			gameObjectToggle.transform.SetParent(toggleGroup, false);
		}
		textReps.text = "Reps " + _repsIterator + "/" + UserDataObject.GetCurrentRepetitionsArray().Length; 

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
		
		// Initialize gesture detector object
		_gestureDetectorList = new List<GestureDetector>();
		for (int bodyIndex = 0; bodyIndex < _bodies.Length; bodyIndex++)
		{
			_gestureDetectorList.Add(new GestureDetector(_kinectSensor));
		}
		
		// Foot joints for getting positions
		_jointFootRight = KinectInterop.JointType.FootRight;
		_jointFootLeft = KinectInterop.JointType.FootLeft;
		_startingHeightDifference = UserDataObject.GetCurrentExerciseStartingHeightDifference();
		heightTestIterator = 0;
		
		// Initial foot position
		if (_kinectManager.IsUserDetected())
		{
			long userId = _kinectManager.GetPrimaryUserID();

			if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) && 
			    _kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
			{	
				_initialStartingHeightLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).y;
				_initialStartingHeightRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight).y;

				initialLeftFootText.text += _initialStartingHeightLeft.ToString();
				initialRightFootText.text += _initialStartingHeightRight.ToString();
			}
		}
		_bothFeetUp = false;
		_inStartingPosition = false;


		if (UserDataObject.GetCurrentExerciseFileName() == "WalkForward") _progressMinConfidence = 0.8f;
		if (UserDataObject.GetCurrentExerciseFileName() == "BobOne") _progressMinConfidence = 0.4f;
		Debug.Log("_progressMinConfidence: " + _progressMinConfidence);
	}

	// Update is called once per frame
	void Update () {
		// If left arrow pressed --> exercise info scene
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			StartCoroutine(LoadPreviousScene());
		}
		// If right arrow pressed --> skip exercise
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
//			LoadSkipExercise();
			StartCoroutine(loadSkipExerciseScene());
		}

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
		
/*		time += Time.deltaTime;
		if (time >= interpolationPeriod) {
			time = time - interpolationPeriod;
			if (_kinectManager && _kinectManager.IsInitialized() && _kinectManager.IsUserDetected())
			{
				long userId = _kinectManager.GetPrimaryUserID();
	
				if (_kinectManager.IsJointTracked(userId, (int) _jointFooRight) && 
					_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
				{
					Debug.Log("------ Second ----- " + heightTestIterator);
					heightTestIterator++;
				
					float jointPosFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFooRight).y;
					float jointPosFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).y;
					
					rightFootText.text = jointPosFootRight.ToString();
					leftFootText.text = jointPosFootLeft.ToString();						
	
					Debug.Log("Left: " + jointPosFootLeft + " || Right: " + jointPosFootRight );
					Debug.Log("Left: " + _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).normalized + 
							  " || Right: " + _kinectManager.GetJointKinectPosition(userId, (int) _jointFooRight).normalized);
	
				}

			}
		}*/
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
		
		if (_kinectManager && _kinectManager.IsInitialized() && _kinectManager.IsUserDetected())
		{
			long userId = _kinectManager.GetPrimaryUserID();
	
			if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) && 
			    _kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
			{
			
				float jointPosFootLeftHeight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).y;
				float jointPosFootRightHeight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight).y;
				float jointPosFootLeftDepth = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).z;
				float jointPosFootRightDepth = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight).z;

//				if ((jointPosFootLeftDepth > jointPosFootRightDepth - _footDepthTolerance) && (jointPosFootLeftDepth < jointPosFootRightDepth + _footDepthTolerance) &&
//				     (jointPosFootRightDepth > jointPosFootLeftDepth - _footDepthTolerance) && (jointPosFootRightDepth < jointPosFootLeftDepth + _footDepthTolerance))
//				{
//					_inStartingPosition = true;
//				}
//				else if (!_bothFeetUp)
//				{
//					_inStartingPosition = false;
//				}
				_inStartingPosition = true;
				
				rightFootHeightText.text = "R Height: " + jointPosFootRightHeight.ToString();
				leftFootHeightText.text = "L Height: " + jointPosFootLeftHeight.ToString();				
				rightFootDepthText.text = "R Depth: " + jointPosFootRightDepth.ToString();
				leftFootDepthText.text = "L Depth: " + jointPosFootLeftDepth.ToString();
					
				// Check if in starting position and foot currentFootHeight is between initialFootHeight + difference tolerance
				if (_inStartingPosition && 
				    (jointPosFootLeftHeight > _initialStartingHeightLeft + _startingHeightDifference) &&
				    (jointPosFootRightHeight > _initialStartingHeightRight + _startingHeightDifference))
				{
					_bothFeetUp = true;
				}
				else
				{
					_bothFeetUp = false;
				}
			}

		}
		// Check positions of joints of current exercise
		foreach (var check in UserDataObject.GetCurrentChecksArray())
		{
			if (_exerciseExecutionValidationManager.GetMethodToCheck(check))
			{
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkToggle.isOn = true;
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkImage.sprite = Resources.Load<Sprite>("Images/Toggle");
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkImage.color = MainColors.ToggleIsOn();
				_methodCheckedArray[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)] = true;
			}
			else
			{
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkToggle.isOn = false;
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkImage.sprite = Resources.Load<Sprite>("Images/IconNotChecked");
				checkItemList[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)].checkImage.color = MainColors.White();
				_methodCheckedArray[Array.IndexOf(UserDataObject.GetCurrentChecksArray(), check)] = false;
				_checksPassed = false;
			}
				
			// Debug.Log(_exerciseExecutionValidationManager.GetMethodToCheck(check));
			armsUpText.text = _checksPassed.ToString();
		}
		
		bothFeetUpText.text = "DIFF: " + _startingHeightDifference + " |UP: " + _bothFeetUp + "| INPOS: " + _inStartingPosition;

		if (_exerciseExecutionValidationManager.LeftFootOnLine() || _exerciseExecutionValidationManager.RightFootOnLine())
		{
			if (CanvasHandCursor.activeSelf)
			{
				_interactionManager.enabled = false;
				CanvasHandCursor.gameObject.SetActive(false);
			}
		}
		// Discrete Gesture tracking
		if ((e.GestureType == GestureType.Discrete))
		{
			_gestureAccuracy = e.DetectionConfidence;
			
			for (int i = 0; i < _methodCheckedArray.Length; i++)
			{
				if (_methodCheckedArray[i] == false)
				{
					_gestureAccuracy -= 0.1f;
				}
				testText.text = _gestureAccuracy.ToString();
			}

			if (_gestureAccuracy > 1.0f) _gestureAccuracy = 1.0f;
			_durationManager.SetProgress(_gestureAccuracy);

			if (GestureDetected(_gestureAccuracy, 0.4f, 1f) && _bothFeetUp)
			{
				if (CanvasHandCursor.activeSelf)
				{
					_interactionManager.enabled = false;
					CanvasHandCursor.gameObject.SetActive(false);
				}
				
				_durationManager.StartTimer();

				if (MinTimeReached())
				{
					audioSuccess.Play();
					
					if (_currentRepetition == UserDataObject.GetCurrentRepetitionsArray().Last())
					{
						Debug.Log("LASTREP");
						successMainText.text = "Exercise finished!";
						successSubText.text = "Preparing next exercise";
						StartCoroutine("StartLastAnimateSuccess");
					}
					else
					{
						StartCoroutine("StartAnimateSuccess");
					}
				}
				
				confidenceIterator++;
				_currentRepetitionConfidence += _gestureAccuracy * 100;
				
//				testText.text = "if DISCRETE: " +  e.IsGestureDetected.ToString() + " " + _gestureAccuracy;
			}
			else
			{
				if (_durationManager.IsTimerRunning() && _durationManager.GetlatestTimeInSeconds() <= _currentRepetition.minTime)
				{
					audioFail.Play();
					Debug.Log("Activate handcursor");

//					if (!CanvasHandCursor.activeSelf)
//					{
//						CanvasHandCursor.gameObject.SetActive(true);
//						_interactionManager.enabled = true;
//					}
				}
				
				if (_durationManager.IsTimerRunning())
				{
					attemptsIterator++;
				}
				_durationManager.StopTimer();
				
				// if tracked time is greater than given time of the repetition
				if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime)
				{
					_repsIterator += 1;
					ToggleAndCheckRepetition();
				}					
				
//				testText.text = "else DISCRETE: " +  e.IsGestureDetected.ToString() + " " + _gestureAccuracy;
			}
//				testText.text = "DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;
		}
		else if ((e.GestureType == GestureType.Continuous))
		{
			_gestureAccuracy = e.Progress;
			
			for (int i = 0; i < _methodCheckedArray.Length; i++)
			{
				if (_methodCheckedArray[i] == false)
				{
					_gestureAccuracy -= 0.1f;
				}
				testText.text = _gestureAccuracy.ToString();
			}
			
			if (_gestureAccuracy > 1.0f) _gestureAccuracy = 1.0f;
			_durationManager.SetProgress(_gestureAccuracy);

//			if (_thirdCheckpoint)
//			{
//				_durationManager.StartTimer();
//
//				confidenceIterator++;
//				_currentRepetitionConfidence += e.Progress * 100;
//				
//				if (e.Progress <= 0.4f)
//				{
//					Debug.Log(e.Progress);
//					if (_durationManager.IsTimerRunning())
//					{
//						_currentRepetition.attempts++;
//						Debug.Log("CURRENT REPETITION ATTEMPT: " + _currentRepetition.attempts);
//
//					}
//					
//					_durationManager.StopTimer();
//					
//					_thirdCheckpoint = false;
//					
//					// if tracked time is greater than given time of the repetition
//					if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime)
//					{
//						ToggleAndCheckRepetition();
//					}
//				}
//			}
//			else
//			{
//				if (_secondCheckpoint && GestureDetected(e.Progress, 0.7f, 1f))
//				{
//					_thirdCheckpoint = true;
//				}
//				else if (_firstCheckpoint && GestureDetected(e.Progress, 0.4f, 0.7f))
				
//				Debug.Log(_bothFeetUp +  " | " + _gestureAccuracy + " | " + e.Progress + " | "+ GestureDetected(_gestureAccuracy, 0.7f, 1f) + " | " + GestureDetected(e.Progress, 0.7f, 1f));
					
				if (GestureDetected(_gestureAccuracy, _progressMinConfidence, 1f) && _bothFeetUp)
				{		
					if (CanvasHandCursor.activeSelf)
					{
						_interactionManager.enabled = false;
						CanvasHandCursor.gameObject.SetActive(false);
					}
					_durationManager.StartTimer();

					if (MinTimeReached())
					{
						audioSuccess.Play();
					
						if (_currentRepetition == UserDataObject.GetCurrentRepetitionsArray().Last())
						{
							Debug.Log("LASTREP");
							
							successMainText.text = "Exercise finished!";
							successSubText.text = "Preparing next exercise";
							StartCoroutine("StartLastAnimateSuccess");
						}
						else
						{
							StartCoroutine("StartAnimateSuccess");
						}
					}

					confidenceIterator++;
					_currentRepetitionConfidence += _gestureAccuracy * 100;

//					testText.text = "if CONTINUOUS: " + _gestureAccuracy;
				}
				else
				{
					if (_durationManager.IsTimerRunning() && _durationManager.GetlatestTimeInSeconds() <= _currentRepetition.minTime)
					{
						audioFail.Play();
						Debug.Log("Activate handcursor");
//						if (!CanvasHandCursor.activeSelf)
//						{
//							CanvasHandCursor.gameObject.SetActive(true);
//							_interactionManager.enabled = true;
//						}
					}
					
					if (_durationManager.IsTimerRunning())
					{
						attemptsIterator++;
						Debug.Log("CURRENT REPETITION ATTEMPT: " + attemptsIterator);
						_durationManager.StopTimer();
					}		

					// if tracked time is greater than given time of the repetition
					if (_durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime)
					{
						_repsIterator += 1;
						ToggleAndCheckRepetition();
					}
				
//					testText.text = "else CONTINUOUS: " + _gestureAccuracy;
				}
//			}
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

	private bool MinTimeReached()
	{
		if (!_minTimeAlreadyReached && _durationManager.GetlatestTimeInSeconds() >= _currentRepetition.minTime)
		{
			_minTimeAlreadyReached = true;
			return true;
		}
		return false;
	}
	
	private void ToggleAndCheckRepetition()
	{
		StopTracking();

		_minTimeAlreadyReached = false;
		_bothFeetUp = false;
		_inStartingPosition = false;
		
		textReps.text = "Reps " + _repsIterator + "/" + UserDataObject.GetCurrentRepetitionsArray().Length; 

		Debug.Log("ACCOMPLISHED REPETITION");
		Debug.Log("Activate Handcursor");
//		if (!CanvasHandCursor.activeSelf)
//		{
//			CanvasHandCursor.gameObject.SetActive(true);
//			_interactionManager.enabled = true;
//		}
		
		// Save the time and confidence for the current repetition
		_currentRepetition.userTime = _durationManager.GetlatestTimeInSeconds();
		_currentRepetition.confidence = _currentRepetitionConfidence / confidenceIterator;
		_currentRepetition.attempts = attemptsIterator;
		
		Debug.Log("Repetition time: " + _currentRepetition.userTime + " || confidence: " +  _currentRepetition.confidence);
		
		// Toggle current repetition
		_currentRepetition.accomplished = true;
		_toggleArray[Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition)].isOn = true;
		
		// Check if last repetition
		if (_currentRepetition == UserDataObject.GetCurrentRepetitionsArray().Last())
		{
			// Exercise and side accomplished
			_currentExerciseData.accomplished = true;
			UserDataObject.GetCurrentSide().accomplished = true;
			
			// Set values for side attempts, confidence, and time regarding repetitions
			UserDataObject.GetCurrentSide().userTime = 0.0f;
			UserDataObject.GetCurrentSide().confidence = 0.0f;
			UserDataObject.GetCurrentSide().attempts = 0;
			foreach (var repetition in UserDataObject.GetCurrentRepetitionsArray())
			{
				UserDataObject.GetCurrentSide().userTime += repetition.userTime;
				UserDataObject.GetCurrentSide().confidence += repetition.confidence;
				UserDataObject.GetCurrentSide().attempts += repetition.attempts;
			}
			// Set the avg usertime and confidence for the current side
			UserDataObject.GetCurrentSide().userTime /= UserDataObject.GetCurrentRepetitionsArray().Length;
			UserDataObject.GetCurrentSide().confidence /= UserDataObject.GetCurrentRepetitionsArray().Length;
			
			// Set values for exercise attempts, confidence, and time regarding sides
			sideAccomplishedCounter = 0;
			_currentExerciseData.confidence = 0.0f;
			_currentExerciseData.userTime = 0.0f;
			_currentExerciseData.attempts = 0;
			foreach (var side in UserDataObject.GetCurrentExercise().sides)
			{
				if (side.accomplished)
				{
					sideAccomplishedCounter++;
					_currentExerciseData.confidence += side.confidence;
					_currentExerciseData.userTime += side.userTime;
					_currentExerciseData.attempts += side.attempts;
				}
			}
			// Set the avg usertime and confidence for the current exercise
			_currentExerciseData.userTime /= sideAccomplishedCounter;
			_currentExerciseData.confidence /= sideAccomplishedCounter;


			// If all sides accomplished
			bool allSidesAccomplished = true;
			SideData nextSide;
			foreach (var side in _currentExerciseData.sides)
			{
				if (side.accomplished == false)
				{
					allSidesAccomplished = false;
				}
			}

			if (allSidesAccomplished)
			{
				// If last exercise --> accomplish current tier
				Debug.Log("PlayerPrefs.GetInt(CurrentExerciseId)" + PlayerPrefs.GetInt("CurrentExerciseId"));
				Debug.Log("UserDataObject.GetCurrentTierErcisesLength()" + UserDataObject.GetCurrentTierErcisesLength());
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
			}

			//		 Save data to user json file
			SaveCurrentExerciseData();
			
			// Load the summary scene
			StartCoroutine(loadSummaryScene());

		} // If not last repetition --> current repetition is next repetition
		else
		{
			_currentRepetition =
				UserDataObject.GetCurrentRepetitionsArray()[Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition) + 1];
			PlayerPrefs.SetInt("CurrentRepetitionId", Array.IndexOf(UserDataObject.GetCurrentRepetitionsArray(), _currentRepetition));
			startTrackingAgain = true;
			Debug.Log("STARTTRAKCINGAGAIN" + startTrackingAgain);
			//		 Save data to user json file
			SaveCurrentExerciseData();
		}
		
		Debug.Log("-----------------------");
		Debug.Log("TOGGLEANDSAVE");
		Debug.Log("-----------------------");

		// Reset all variables needed for next repetition
		_currentRepetitionConfidence = 0;				
		confidenceIterator = 0;
		attemptsIterator = 0;
		
		_durationManager.ResetlatestTimeInSeconds();
		_durationManager.ResetProgress();
		
		_firstCheckpoint = false;
		_secondCheckpoint = false;
		
		pufferList.Clear();

		if (startTrackingAgain)
		{
			StartCoroutine("StartTracking");
//      		StartCoroutine("RewindAnimateSuccess");
      
		}
	}
	
	public void StopTracking()
	{
		startTrackingAgain = false;
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
//			_gestureDetectorList.RemoveAt(_gestureDetectorList.IndexOf(gesture));
		}
//		_gestureDetectorList = null;
	}
	
	IEnumerator LoadPreviousScene()
	{
		yield return new WaitForSeconds(0.5f);

		CanvasHandCursor.gameObject.SetActive(true);
		_interactionManager.enabled = true;
		
		StopTracking();
		DisposeGestures();
		SceneManager.LoadScene("ExerciseInfo");
	}
	
	IEnumerator loadSkipExerciseScene()
	{
		yield return new WaitForSeconds(0.5f);

		CanvasHandCursor.gameObject.SetActive(true);
		_interactionManager.enabled = true;
		
		_currentExerciseData.sides[PlayerPrefs.GetInt("CurrentSideId")].accomplished = true;
		UserDataObject.GetCurrentSide().accomplished = true;
		
		StopTracking();
		DisposeGestures();
		ManageNextScene();
//		SceneManager.LoadScene("ExerciseSummary");
	}

	IEnumerator loadSummaryScene()
	{
		yield return new WaitForSeconds(2.5f);
		Debug.Log("LOAD SUMMARY SCENE");
		CanvasHandCursor.gameObject.SetActive(true);
		_interactionManager.enabled = true;
		
		_currentExerciseData.sides[PlayerPrefs.GetInt("CurrentSideId")].accomplished = true;
		UserDataObject.GetCurrentSide().accomplished = true;
		
		StopTracking();
		DisposeGestures();
		
		ManageNextScene();
//		SceneManager.LoadScene("ExerciseSummary");
	}

	public void ManageNextScene()
	{
		Debug.Log("GetCurrentExercise: " + UserDataObject.GetCurrentExercise().fileName);
		Debug.Log("GetLastTierExercise: " + UserDataObject.GetLastTierExercise().fileName);
		Debug.Log("CurrentExercise ID: " + PlayerPrefs.GetInt("CurrentExerciseId"));
		
		// Check if a side is not accomplished
		foreach (var side in _currentExerciseData.sides)
		{
			Debug.Log("side direction: " + side.direction);
			Debug.Log("side accomplished: " + side.accomplished);
			if (side.accomplished == false)
			{
				_sideNotAccomplished = true;
			}
		}
		
		// Not last exercise --> load following side
		if (_currentExerciseData != _lastExercise || _sideNotAccomplished)
		{
			Debug.Log("any sideNotAccomplished? : " + _sideNotAccomplished);
			if (_sideNotAccomplished)
			{
				// TODO change subtitle of success GO
				
				if (PlayerPrefs.GetInt("CurrentSideId") == 0)
				{
					Debug.Log("LOAD LEFT");
					PlayerPrefs.SetInt("CurrentSideId", 1);
					PlayerPrefs.SetString("CurrentSide", "Left");
					LoadNextScene("ExerciseInfo");
				}
				else if (PlayerPrefs.GetInt("CurrentSideId") == 1)
				{
					Debug.Log("LOAD RIGHT");
					PlayerPrefs.SetInt("CurrentSideId", 0);
					PlayerPrefs.SetString("CurrentSide", "Right");
					LoadNextScene("ExerciseInfo");
				}
			}
			else // all sides accomplished --> load next exercise
			{
				// TODO change subtitle of success GO
				UserDataObject.GetNextExercise().isInteractable = true;
				UserDataObject.GetNextExercise().unlocked = 1;
				
				PlayerPrefs.SetInt("CurrentExerciseId", PlayerPrefs.GetInt("CurrentExerciseId") + 1);
				LoadNextScene("ExerciseSideSelection");
			}			
		}
		else // all exercises completed --> load TierMenu
		{
			// TODO change subtitle of success GO
			UserDataObject.GetNextTier().isInteractable = true;
			
			if (!UserDataObject.GetCurrentExercise().isInteractable)
			{
				UserDataObject.GetCurrentExercise().isInteractable = true;
				UserDataObject.GetCurrentExercise().unlocked = 1;	
			}
			
			
			LoadNextScene("TierMenu");
		}
		SaveCurrentExerciseData();

	}
	
	public void LoadNextScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);		
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
	

	public float GetLeftFootStartingHeight(){
		return _initialStartingHeightLeft + _startingHeightDifference;
	}
	public float GetRightFootStartingHeight(){
		return _initialStartingHeightRight + _startingHeightDifference;
	}

	IEnumerator StartAnimateSuccess()
	{
		Debug.Log("---------------------");
		Debug.Log("StartAnimateSuccess");
		for (int i = 0; i < 30; i++)
		{	
			yield return new WaitForSeconds(0.01f);
			SuccessGroupTransform.Translate(Vector3.up * 0.02f);
		}
		Debug.Log("---------------------");
		Debug.Log("STOPAnimateSuccess");
		yield return new WaitForSeconds(2f);
		Debug.Log("---------------------");
		Debug.Log("RewindAnimateSuccess");
		for (int i = 0; i < 30; i++)
		{	
			yield return new WaitForSeconds(0.01f);
			SuccessGroupTransform.Translate(Vector3.down * 0.02f);
		}
		Debug.Log("---------------------");
		Debug.Log("STOPRewindAnimateSuccess");
	}	
	
	IEnumerator StartLastAnimateSuccess()
	{
		Debug.Log("---------------------");
		Debug.Log("StartAnimateSuccess");
		for (int i = 0; i < 32; i++)
		{	
			yield return new WaitForSeconds(0.01f);
			SuccessGroupTransform.Translate(Vector3.up * 0.02f);
		}
		Debug.Log("---------------------");
		Debug.Log("STOPAnimateSuccess");
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