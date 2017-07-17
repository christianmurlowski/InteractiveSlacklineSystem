using System;
using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodyManager : MonoBehaviour 
{
	public static BodyManager BM;
	
//	private KinectInterop.SensorData _Sensor;
	private KinectSensor _Sensor;
	private BodyFrameReader _Reader;
	private Body[] _Data = null;

	public Body[] GetData()
	{
		return _Data;
	}
	public Body[] GetBodies()
	{
		return _Data;
	}
	
	public ulong GetBodiesarray()
	{
		foreach (var body in _Data)
		{
			return body.TrackingId;
		}
		return 0;
	}

	public void DisposeBodyManager()
	{
		if (_Reader != null)
		{
			_Reader.Dispose();
			_Reader = null;
			_Sensor = null;
			GC.SuppressFinalize(this);
		}
		        
		if (_Sensor != null)
		{
			if (_Sensor.IsOpen)
			{
				_Sensor.Close();
			}
            
			_Sensor = null;
		}
		
		_Sensor = KinectSensor.GetDefault();

		if (_Sensor != null)
		{
			_Reader = _Sensor.BodyFrameSource.OpenReader();
			
			if (_Data == null)
			{
				Debug.Log("_bodies: " + GetData());

				_Data = new Body[_Sensor.BodyFrameSource.BodyCount]; // evtl hier in "new Body[1]" einsetzen
//				_Data = new Body[1]; // evtl hier in "new Body[1]" einsetzen
				Debug.Log("_bodies new: " + GetData());
				Debug.Log("_bodies new lengt: " + GetData().Length);
			}
			
			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		}   
	}

	public KinectSensor GetSensor()
	{
		return _Sensor;
	}

//	private void Awake()
//	{
//		if (BM == null)
//		{
//			DontDestroyOnLoad(gameObject);
//			BM = this;
//		}
//		else
//		{
//			if (BM != null)
//			{
//				Destroy(gameObject);
//			}
//		}
//	}
	KinectInterop.SensorData sensorData;
	void Start ()
	{
		_Sensor = KinectSensor.GetDefault();

		if (_Sensor != null)
		{
			_Reader = _Sensor.BodyFrameSource.OpenReader();
			
			if (_Data == null)
			{
				Debug.Log("_bodies: " + GetData());

				_Data = new Body[_Sensor.BodyFrameSource.BodyCount]; // evtl hier in "new Body[1]" einsetzen
//				_Data = new Body[1]; // evtl hier in "new Body[1]" einsetzen
				Debug.Log("_bodies new: " + GetData());
				Debug.Log("_bodies new lengt: " + GetData().Length);
			}
			
			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		}   
	}
    
	void Update () 
	{
		if (_Reader != null)
		{
			var frame = _Reader.AcquireLatestFrame();
			if (frame != null)
			{
				if (_Data == null)
				{
					Debug.Log("_bodies: " + GetData());

					_Data = new Body[_Sensor.BodyFrameSource.BodyCount]; // evtl hier in "new Body[1]" einsetzen
//					_Data = new Body[1]; // evtl hier in "new Body[1]" einsetzen
					Debug.Log("_bodies new: " + GetData());
					Debug.Log("_bodies new length: " + GetData().Length);
				}
				//Debug.Log("BM: " + _Sensor.BodyFrameSource.BodyCount);

				frame.GetAndRefreshBodyData(_Data);
                
				frame.Dispose();
				frame = null;
			}
		}    
	}
    
	void OnApplicationQuit()
	{
		if (_Reader != null)
		{
			_Reader.Dispose();
			_Reader = null;
		}
        
		if (_Sensor != null)
		{
			if (_Sensor.IsOpen)
			{
				_Sensor.Close();
			}
            
			_Sensor = null;
		}
	}
}
