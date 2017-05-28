using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class BodyView : MonoBehaviour {
	public BodyManager BodyManager;
	
	private Dictionary<ulong, GameObject> _bodies = new Dictionary<ulong, GameObject>();

	private BodyManager _bodyManager;

	private ulong _currentTrackedBody;

	private Kinect.KinectSensor sensor;

	private Kinect.MultiSourceFrameReader reader;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (BodyManager == null)
		{
			return;
		}
		
		_bodyManager = BodyManager.GetComponent<BodyManager>(); // Store gameobject for usage
		if (_bodyManager == null)
		{
			return;
		}
		
		Kinect.Body[] data = _bodyManager.GetData(); // Get bodies of bodymanager
		if (data == null)
		{
			return;
		}
        
		
		
		List<ulong> trackedIds = new List<ulong>(); // List to store tracked body ids
		
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
                
			if(body.IsTracked)
			{	
				KinectInputModule.instance.TrackBody(body);
				trackedIds.Add (body.TrackingId);

				if (_currentTrackedBody == 0)
				{
					_currentTrackedBody = body.TrackingId;
				}
				else
				{
					
				}
				
				//Debug.Log(body.TrackingId + " | " + _currentTrackedBody + " | " + trackedIds.Count);
			}
		}
		
		List<ulong> knownIds = new List<ulong>(_bodies.Keys);
        
		// First delete untracked bodies
		foreach(ulong trackingId in knownIds)
		{
			if(!trackedIds.Contains(trackingId))
			{
				Destroy(_bodies[trackingId]);
				_bodies.Remove(trackingId);
			}
		}
	} // Update
}
