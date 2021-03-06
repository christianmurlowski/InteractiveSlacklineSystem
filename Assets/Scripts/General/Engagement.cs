﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kinect = Windows.Kinect;

public class Engagement : MonoBehaviour
{
	public AudioSource audioSuccess;
	
	public KinectManager KinectManager;
	private KinectManager _kinectManager;

	private KinectInterop.JointType _jointHandRight,
									_jointHandLeft,
									_jointHead;
	
	// Use this for initialization
	void Start ()
	{
		PlayerPrefs.DeleteAll();
		_jointHandRight = KinectInterop.JointType.HandRight;
		_jointHandLeft = KinectInterop.JointType.HandLeft;
		_jointHead = KinectInterop.JointType.Head;
		
		Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));
		Debug.Log("CurrentExerciseId: " + PlayerPrefs.GetInt("CurrentExerciseId"));
		Debug.Log("CurrentSide: " + PlayerPrefs.GetString("CurrentSide"));
	} // Start

	// Update is called once per frame
	void Update ()
	{
		// If right arrow pressed --> start tutorial
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			SceneManager.LoadScene("TutHandPush");
		}
		
		if (KinectManager == null)
		{
			return;
		}

		_kinectManager = KinectManager.GetComponent<KinectManager>();

//		_kinectManager = KinectManager.Instance;
		
		if (_kinectManager && _kinectManager.IsInitialized())
		{
			if (_kinectManager.IsUserDetected())
			{
				long userId = _kinectManager.GetPrimaryUserID();

				if ((_kinectManager.IsJointTracked(userId, (int) _jointHandRight) || _kinectManager.IsJointTracked(userId, (int) _jointHandLeft)) &&
				    _kinectManager.IsJointTracked(userId, (int) _jointHead))
				{
					Vector3 jointPosHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
					Vector3 jointPosHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
					Vector3 jointPosHead = _kinectManager.GetJointKinectPosition(userId, (int) _jointHead);			

					if (!audioSuccess.isPlaying && ((jointPosHandRight.y > jointPosHead.y) || (jointPosHandLeft.y > jointPosHead.y)))
					{	
						SceneManager.LoadScene("TutHandPush");
					}
				}
			}
		}
	} // Update

}
