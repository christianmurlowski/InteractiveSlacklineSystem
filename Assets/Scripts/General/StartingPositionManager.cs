using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingPositionManager : MonoBehaviour
{

	public GameObject KinectManager;
	public Button buttonUserSelectionStart;
	public AudioSource audioSuccess,
					   audioFail;

	private KinectManager _kinectManager;
	private KinectInterop.JointType _jointFootLeft,
									_jointFootRight;
	private float tolerance = 0.1f;
	
	void Start () {
		
		KinectManager = GameObject.Find("KinectManager");

		if (KinectManager == null)
		{
			return;
		}

		_kinectManager = KinectManager.GetComponent<KinectManager>();

		_jointFootRight = KinectInterop.JointType.FootRight;
		_jointFootLeft = KinectInterop.JointType.FootLeft;

		buttonUserSelectionStart.GetComponent<Button>().interactable = false;
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene("UserSelection");
	}
	
	// Update is called once per frame
	void Update () {

		if (_kinectManager && _kinectManager.IsInitialized() && _kinectManager.IsUserDetected())
		{
			long userId = _kinectManager.GetPrimaryUserID();

			if (_kinectManager.IsJointTracked(userId, (int) _jointFootRight) &&
			    _kinectManager.IsJointTracked(userId, (int) _jointFootLeft))
			{
				float jointPosFootLeftDepth = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootLeft).z;
				float jointPosFootRightDepth = _kinectManager.GetJointKinectPosition(userId, (int) _jointFootRight).z;
				
				if (((jointPosFootLeftDepth > jointPosFootRightDepth - tolerance) && (jointPosFootLeftDepth < jointPosFootRightDepth + tolerance) &&
				     (jointPosFootRightDepth > jointPosFootLeftDepth - tolerance) && (jointPosFootRightDepth < jointPosFootLeftDepth + tolerance)))
				{
					if (!buttonUserSelectionStart.GetComponent<Button>().interactable)
					{
						if (audioFail.isPlaying) audioFail.Stop();
						audioSuccess.Play();
						
						buttonUserSelectionStart.GetComponent<Button>().interactable = true;
						buttonUserSelectionStart.GetComponentInChildren<Text>().text = "Click me!";						
					}
				}
				else
				{
					if (buttonUserSelectionStart.GetComponent<Button>().interactable)
					{
						if (audioSuccess.isPlaying) audioSuccess.Stop();
						audioFail.Play();
						
						buttonUserSelectionStart.GetComponent<Button>().interactable = false;
						buttonUserSelectionStart.GetComponentInChildren<Text>().text = "Please go into starting position";
					}
				}
				
			}
		}
	}
}
