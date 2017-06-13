﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kinect = Windows.Kinect;

public class Engagement : MonoBehaviour
{
	public KinectManager KinectManager;
	private KinectManager _kinectManager;

	private KinectInterop.JointType _jointHandRight;
	private KinectInterop.JointType _jointHead;
	
	// Use this for initialization
	void Start ()
	{
		_jointHandRight = KinectInterop.JointType.HandRight;
		_jointHead = KinectInterop.JointType.Head;
		
	} // Start
	
	// Update is called once per frame
	void Update ()
	{
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

				if (_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
				    _kinectManager.IsJointTracked(userId, (int) _jointHead))
				{
					Vector3 jointPosHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
					Vector3 jointPosHead = _kinectManager.GetJointKinectPosition(userId, (int) _jointHead);			

					if (jointPosHandRight.y > jointPosHead.y)
					{
						SceneManager.LoadScene("UserSelection");
					}					

				}
			}
		}
	} // Update
}
