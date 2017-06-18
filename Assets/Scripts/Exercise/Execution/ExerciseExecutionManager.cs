using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
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

	private List<GestureDetector> _gestureDetectors = null;


	// Use this for initialization
	void Start ()
	{
		UserSelectionManager.TestSetCurrentUser(); // TODO Just for test purposes -> Delete in production
		
		_currentExerciseData = UserDataObject.currentUser.exerciseData[PlayerPrefs.GetInt("CurrentExerciseId")];
		
		_currentExerciseData = UserDataObject.currentUser.exerciseData[0]; // TODO Just for test purposes -> Delete in production
		
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
		
		Debug.Log("EXECUTION:" + _bodyManager + " | " + _bodies);
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
