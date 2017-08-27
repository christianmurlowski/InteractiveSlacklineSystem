using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseInfoManager : MonoBehaviour
{

	public GameObject exerciseTipPanel,
					  KinectManager;
	public Transform buttonExerciseStart,
					 spacer;
	public Text exerciseName,
				exerciseRepetitions,
				exerciseMinTime;
	public AudioSource audioSuccess,
					   audioFail;
	
	private KinectManager _kinectManager;
	private KinectInterop.JointType _jointFootRight,
									_jointFootLeft;
	private float tolerance = 0.1f;
	
	// DEV TESTING
	public Text footLeftText,
				footRightText,
				inRange;
	
	// Use this for initialization
	void Start ()
	{
		exerciseName.text = UserDataObject.GetCurrentExerciseAndSideName();
		
		KinectManager = GameObject.Find("KinectManager");
		if (KinectManager == null)
		{
			return;
		}

		_kinectManager = KinectManager.GetComponent<KinectManager>();
		
		_jointFootRight = KinectInterop.JointType.FootRight;
		_jointFootLeft = KinectInterop.JointType.FootLeft;
		
		// Start button is false
		buttonExerciseStart.GetComponent<Button>().interactable = false;

		FillTipList();
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene("ExerciseExecution");
	}
	
	public void LoadPreviousScene()
	{
		SceneManager.LoadScene("ExerciseSideSelection");
	}
	
	void FillTipList()
	{
		foreach (var tip in UserDataObject.GetCurrentTipsArray())
		{
			GameObject gameObjectTipPanel = Instantiate(exerciseTipPanel) as GameObject;
			TipPanel tipPanel = gameObjectTipPanel.GetComponent<TipPanel>();

			tipPanel.tipDescription.text = tip.description;
			tipPanel.tipNumber.text = (Array.IndexOf(UserDataObject.GetCurrentTipsArray(), tip)+1).ToString();
			
			gameObjectTipPanel.transform.SetParent(spacer, false);
		}
		
		exerciseRepetitions.text = UserDataObject.GetCurrentRepetitionsArrayLength().ToString();
		exerciseMinTime.text = UserDataObject.GetCurrentRepetitionMinTime() + " sec";
	}

	private void Update()
	{
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
				
				footLeftText.text = jointPosFootLeftDepth.ToString();
				footRightText.text = jointPosFootRightDepth.ToString();
				
				if (((jointPosFootLeftDepth > jointPosFootRightDepth - tolerance) && (jointPosFootLeftDepth < jointPosFootRightDepth + tolerance) &&
				    (jointPosFootRightDepth > jointPosFootLeftDepth - tolerance) && (jointPosFootRightDepth < jointPosFootLeftDepth + tolerance)) &&
				    ((jointPosFootLeftHeight > jointPosFootRightHeight - tolerance) && (jointPosFootLeftHeight < jointPosFootRightHeight + tolerance) &&
				     (jointPosFootRightHeight > jointPosFootLeftHeight - tolerance) && (jointPosFootRightHeight < jointPosFootLeftHeight + tolerance)))
				{
					if (!buttonExerciseStart.GetComponent<Button>().interactable)
					{
						if (audioFail.isPlaying) audioFail.Stop();
						audioSuccess.Play();
							
						buttonExerciseStart.GetComponent<Button>().interactable = true;
						buttonExerciseStart.GetComponentInChildren<Text>().text = "Click to start exercise";
						inRange.text = true.ToString();						
					}
				}
				else
				{
					if (buttonExerciseStart.GetComponent<Button>().interactable)
					{
						if (audioSuccess.isPlaying) audioSuccess.Stop();
						audioFail.Play();

						inRange.text = false.ToString();
						buttonExerciseStart.GetComponent<Button>().interactable = false;
						buttonExerciseStart.GetComponentInChildren<Text>().text = "Please go into starting position";
					}
				}
					
			}

		}
	}
}
