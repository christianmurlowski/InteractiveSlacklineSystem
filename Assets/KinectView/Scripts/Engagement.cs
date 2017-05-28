using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kinect = Windows.Kinect;

public class Engagement : MonoBehaviour
{

	public BodyManager BodyManager;
	private BodyManager _bodyManager;

	private Kinect.BodyFrame _bodyFrame;
	
	private bool levelLoad = false;	

	// Use this for initialization
	void Start ()
	{
		
		
	} // Start
	
	// Update is called once per frame
	void Update ()
	{

		if (BodyManager == null)
		{
			return;
		}

		_bodyManager = BodyManager.GetComponent<BodyManager>();
		if (_bodyManager == null)
		{
			return;
		}

		Kinect.Body[] data = _bodyManager.GetData();
		if (data == null)
		{
			return;
		}
		
		foreach (var body in data)
		{
			if (body == null)
			{
				continue;
			}

			if (body.IsTracked)
			{			
				var rightHandPos = body.Joints[Kinect.JointType.HandRight].Position;
				var headPos = body.Joints[Kinect.JointType.Head].Position;

				if (rightHandPos.Y > headPos.Y)
				{
					SceneManager.LoadScene("MainMenu");
				}
				//Debug.Log("Handpos: " + rightHandPos.Y + " || " + headPos.Y + " :Head");
			}
		}
	} // Update
}
