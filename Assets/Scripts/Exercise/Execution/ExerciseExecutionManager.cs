using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseExecutionManager : MonoBehaviour
{
	private ExerciseData _currentExerciseData;

	public Text titleText;

	public GameObject bodyManager;
	private BodyManager _bodyManager;
	private KinectSensor _kinectSensor;
	private Body[] _bodies = null;

	public DurationManager durationManager;

	private string _gestureName;

	private List<GestureDetector> _gestureDetectorList = null;


	public Text testText;
	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		PlayerPrefs.SetInt("CurrentExerciseId", 3);// TODO Just for test purposes -> Delete in production
		
		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		Debug.Log(_currentExerciseData.exerciseName);

		titleText.text = _currentExerciseData.exerciseName.ToUpper();
		
		
		durationManager = GetComponent<DurationManager>();


		_bodyManager = bodyManager.GetComponent<BodyManager>();
//		_bodyManager = BodyManager.BM;
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
			testText.text = "DISCRETE: " +  e.IsGestureDetected.ToString() + " " + e.DetectionConfidence;		
		}
		else if (e.GestureType == GestureType.Continuous)
		{
			testText.text = "CONTINUOUS: " + e.Progress;
		}
	}	
}
