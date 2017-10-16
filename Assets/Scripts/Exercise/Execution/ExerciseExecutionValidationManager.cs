using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseExecutionValidationManager : MonoBehaviour {

	public GameObject KinectManager,
					  ExerciseExecutionManager;

	private ExerciseExecutionManager _exerciseExecutionManager;
	private KinectManager _kinectManager;
	private KinectInterop.JointType _jointFootRight,
									_jointFootLeft,
									_jointHandRight,
									_jointHandLeft,
									_jointHipRight,
									_jointHipLeft,
									_jointKneeRight,
									_jointKneeLeft,
									_jointShoulderRight,
									_jointShoulderLeft,
									_jointSpineShoulder,
									_jointSpineBase;
	private Vector3 _positionFootRight,
				  	_positionFootLeft,
					_positionHandRight,
					_positionHandLeft,
					_positionHipRight,
					_positionHipLeft,
					_positionKneeRight,
					_positionKneeLeft,
					_positionShoulderRight,
					_positionShoulderLeft,
					_positionSpineShoulder,
					_positionSpineBase;
	private long userId;
	// Use this for initialization
	void Start () {

		_exerciseExecutionManager = ExerciseExecutionManager.GetComponent<ExerciseExecutionManager>();
		// -----------------------------------------
		// ---------------- KINECT -----------------
		// -----------------------------------------
		KinectManager  = GameObject.Find("KinectManager");
		_kinectManager = KinectManager.GetComponent<KinectManager>();

		_jointFootRight 	= KinectInterop.JointType.FootRight;
		_jointFootLeft 		= KinectInterop.JointType.FootLeft;

		_jointHandRight 	= KinectInterop.JointType.HandRight;
		_jointHandLeft 		= KinectInterop.JointType.HandLeft;

		_jointHipRight 		= KinectInterop.JointType.HipRight;
		_jointHipLeft 		= KinectInterop.JointType.HipLeft;

		_jointKneeRight 	= KinectInterop.JointType.KneeRight;
		_jointKneeLeft 		= KinectInterop.JointType.KneeLeft;

		_jointShoulderRight 	= KinectInterop.JointType.ShoulderRight;
		_jointShoulderLeft 		= KinectInterop.JointType.ShoulderLeft;

		_jointSpineShoulder = KinectInterop.JointType.SpineShoulder;

		_jointSpineBase 	= KinectInterop.JointType.SpineBase;
	}

	void Update(){
		userId    = _kinectManager.GetPrimaryUserID();
		
		_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
		_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);

		_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight); 
		_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft); 

		_positionHipRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHipRight); 
		_positionHipLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHipLeft); 

		_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight); 
		_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft); 

		_positionShoulderRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointShoulderRight);
		_positionShoulderLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointShoulderLeft);

		_positionSpineShoulder = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineShoulder); 

		_positionSpineBase = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineBase);
	}

	public bool BothArmsUp() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandRight) && 
			_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointShoulderRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointShoulderLeft))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight); 
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionShoulderRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointShoulderRight); 
			_positionShoulderLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointShoulderLeft); 
			
			if (_positionHandRight.y > _positionShoulderRight.y && 
			    _positionHandLeft.y > _positionShoulderLeft.y)
				return true;
		}
		return false;
	}
	public bool BothArmsLoose() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandRight) && 
			_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointSpineShoulder))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight); 
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionSpineShoulder = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineShoulder); 
			
			if (_positionHandRight.y < _positionSpineShoulder.y && _positionHandLeft.y < _positionSpineShoulder.y)
				return true;
		}
		return false;
	}
	public bool RightFootStretchedToSide() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointShoulderRight))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionShoulderRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHipRight);

			if (_positionFootRight.x > _positionShoulderRight.x &&
				_positionFootRight.y > _positionFootLeft.y + 0.1) 
				return true;
		}
		return false;
	}
	public bool LeftFootStretchedToSide() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointShoulderLeft))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionShoulderLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHipLeft);
			
			if (_positionFootLeft.x < _positionShoulderLeft.x &&
				_positionFootLeft.y > _positionFootRight.y + 0.1)
				return true;
		}
		return false;
	}
	public bool ArmsCrossed() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointHandLeft))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);

			
			if (_positionHandRight.x < _positionHandLeft.x) // Maybe + 0.1
				return true;
		}
		return false;
	}
	public bool LeftFootOnKnee() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeRight))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight);
			
			if (_positionFootLeft.y > (_positionKneeRight.y - 0.25) &&
				_positionFootLeft.y < (_positionKneeRight.y + 0.25))
				return true;
		}
		return false;
	}
	public bool RightFootOnKnee() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft);
			
			if (_positionFootRight.y > (_positionKneeLeft.y - 0.25) &&
				_positionFootRight.y < (_positionKneeLeft.y + 0.25))
				return true;
		}
		return false;
	}
	public bool HandsOnRightKneeFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeRight))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight);
			
			if (_positionHandRight.z < (_positionKneeRight.z + 0.25) &&
				_positionHandLeft.z < (_positionKneeRight.z + 0.25) &&
				_positionHandRight.y < (_positionKneeRight.y + 0.25) &&
				_positionHandRight.y > (_positionKneeRight.y - 0.25) &&
				_positionHandLeft.y < (_positionKneeRight.y + 0.25) &&
				_positionHandLeft.y > (_positionKneeRight.y - 0.25))
				return true;
		}
		return false;
	}
	public bool HandsOnLeftKneeFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeLeft))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft);
			
			if (_positionHandRight.z < (_positionKneeLeft.z + 0.25) &&
				_positionHandLeft.z < (_positionKneeLeft.z + 0.25) &&
				_positionHandRight.y < (_positionKneeLeft.y + 0.25) &&
				_positionHandRight.y > (_positionKneeLeft.y - 0.25) &&
				_positionHandLeft.y < (_positionKneeLeft.y + 0.25) &&
				_positionHandLeft.y > (_positionKneeLeft.y - 0.25))
				return true;
		}
		return false;
	}

	public bool HandsOnRightKneeSide() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeRight))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight);
			
			if (_positionHandRight.x > (_positionKneeRight.x - 0.2) &&
				_positionHandLeft.x > (_positionKneeRight.x - 0.2) &&
				_positionHandRight.y < (_positionKneeRight.y + 0.2) &&
				_positionHandRight.y > (_positionKneeRight.y - 0.2) &&
				_positionHandLeft.y < (_positionKneeRight.y + 0.2) &&
				_positionHandLeft.y > (_positionKneeRight.y - 0.2))
				return true;
		}
		return false;
	}
	public bool HandsOnLeftKneeSide() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointHandLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointHandRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeLeft))
		{
			_positionHandRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandRight);
			_positionHandLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointHandLeft);
			_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft);
			
			if (_positionHandRight.x > (_positionKneeLeft.x - 0.2) &&
				_positionHandLeft.x > (_positionKneeLeft.x - 0.2) &&
				_positionHandRight.y < (_positionKneeLeft.y + 0.2) &&
				_positionHandRight.y > (_positionKneeLeft.y - 0.2) &&
				_positionHandLeft.y < (_positionKneeLeft.y + 0.2) &&
				_positionHandLeft.y > (_positionKneeLeft.y - 0.2))
				return true;
		}
		return false;
	}
	public bool LeftFootOnLine() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);

			if (_positionFootLeft.y > _exerciseExecutionManager.GetLeftFootStartingHeight())
				return true;
		}
		return false;
	}
	public bool RightFootOnLine() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);

			if (_positionFootRight.y > _exerciseExecutionManager.GetRightFootStartingHeight())
				return true;
		}
		return false;
	}
	public bool RightFootInFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);

			if (_positionFootRight.z < _positionFootLeft.z - 0.2 &&
				_positionFootRight.y > -0.9)
				return true;
		}
		return false;
	}
	public bool LeftFootInFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);

			if (_positionFootLeft.z < _positionFootRight.z - 0.2 &&
				_positionFootLeft.y > -0.9)
				return true;
		}
		return false;
	}
	public bool RightFootBehind() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			
			if (_positionFootRight.z > (_positionFootLeft.z + 0.2) &&
				_positionFootRight.y > -0.95)

				return true;
		}
		return false;
	}
	public bool LeftFootBehind() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			
			if (_positionFootLeft.z > (_positionFootRight.z + 0.2) &&
				_positionFootLeft.y > -0.95)
				return true;
		}
		return false;
	}
	public bool SitsOnLine() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointSpineBase))
		{
			_positionSpineBase = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineBase);
			
			if (_positionSpineBase.y < -0.45)
				return true;
		}
		return false;
	}
	public bool RightFootBent() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeRight)&&
		    _kinectManager.IsJointTracked(userId, (int) _jointSpineBase))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight);
			_positionSpineBase = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineBase);

			if (_positionFootRight.z > (_positionKneeRight.z - 0.25) &&
				_positionFootRight.z < (_positionKneeRight.z + 0.25) &&
			    _positionSpineBase.y < -0.5)
				return true;
		}
		return false;
	}
	public bool LeftFootBent() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeLeft) &&
		    _kinectManager.IsJointTracked(userId, (int) _jointSpineBase))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft);
			_positionSpineBase = _kinectManager.GetJointKinectPosition(userId, (int) _jointSpineBase);

			if (_positionFootLeft.z > (_positionKneeLeft.z - 0.25) &&
				_positionFootLeft.z < (_positionKneeLeft.z + 0.25) &&
			    _positionSpineBase.y < -0.5)
				return true;
		}
		return false;
	}
	public bool RightFootStretchedFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeRight))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionKneeRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeRight);
			
			if (_positionFootRight.z < (_positionKneeRight.z - 0.2))
				return true;
		}
		return false;
	}
	public bool LeftFootStretchedFront() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointKneeLeft))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionKneeLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointKneeLeft);
			
			if (_positionFootLeft.z < (_positionKneeLeft.z - 0.2))
				return true;
		}
		return false;
	}	
	public bool LeftFootUp() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
		    _kinectManager.IsJointTracked(userId, (int) _jointFootRight))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			
			if (_positionFootLeft.y > _positionFootRight.y + 0.1)
				return true;
		}
		return false;
	}	
	public bool RightFootUp() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootLeft) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootRight))
		{
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			
			if (_positionFootRight.y > _positionFootLeft.y + 0.1)
				return true;
		}
		return false;
	}
	public bool FootCrossed() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			
			if (_positionFootRight.x < _positionFootLeft.x || RightFootOnLine() || LeftFootOnLine())
				return true;
		}
		return false;
	}
	public bool RightFootInFrontOfLeft() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			
			if (_positionFootRight.z < _positionFootLeft.z &&
				_positionFootRight.x < (_positionFootLeft.x + 0.2) &&
				_positionFootRight.x > (_positionFootLeft.x - 0.2))
				return true;
		}
		return false;
	}
	public bool LeftFootInFrontOfRight() {
		if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			_kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
		{
			_positionFootRight = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight);
			_positionFootLeft = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft);
			
			if (_positionFootLeft.z < _positionFootRight.z &&
				_positionFootLeft.x < (_positionFootRight.x + 0.2) &&
				_positionFootLeft.x > (_positionFootRight.x - 0.2))
				return true;
		}
		return false;
	}

	public bool GetMethodToCheck(ChecksData check) {
		// Debug.Log(check.methodName);
		switch(check.methodName) {
			case "ArmsCrossed":
				return ArmsCrossed();

			case "BothArmsLoose":
				return BothArmsLoose();

			case "BothArmsUp":
				return BothArmsUp();

			case "FootCrossed":
				return FootCrossed();

			case "HandsOnLeftKneeFront":
				return HandsOnLeftKneeFront();

			case "HandsOnRightKneeFront":
				return HandsOnRightKneeFront();

			case "HandsOnLeftKneeSide":
				return HandsOnLeftKneeSide();

			case "HandsOnRightKneeSide":
				return HandsOnRightKneeSide();

			case "LeftFootBehind":
				return LeftFootBehind();

			case "LeftFootBent":
				return LeftFootBent();

			case "LeftFootInFront":
				return LeftFootInFront();

			case "LeftFootInFrontOfRight":
				return LeftFootInFrontOfRight();

			case "LeftFootOnKnee":
				return LeftFootOnKnee();
				
			case "LeftFootOnLine":
				return LeftFootOnLine();
				
			case "LeftFootStretchedFront":
				return LeftFootStretchedFront();

			case "LeftFootStretchedToSide":
				return LeftFootStretchedToSide();

			case "LeftFootUp":
				return LeftFootUp();

			case "RightFootBehind":
				return RightFootBehind();

			case "RightFootBent":
				return RightFootBent();

			case "RightFootInFront":
				return RightFootInFront();

			case "RightFootInFrontOfLeft":
				return RightFootInFrontOfLeft();

			case "RightFootOnKnee":
				return RightFootOnKnee();

			case "RightFootOnLine":
				return RightFootOnLine();

			case "RightFootStretchedFront":
				return RightFootStretchedFront();

			case "RightFootStretchedToSide":
				return RightFootStretchedToSide();
			
			case "RightFootUp":
				return RightFootUp();
			
			case "SitsOnLine":
				return SitsOnLine();

			default:
				return false;
		}
	}
}
