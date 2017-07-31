using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
	public MovieTexture movieTexture;
	
	// Use this for initialization
	void Start ()
	{
		// TODO Just for test purposes -> Delete in production
//		UserSelectionManager.TestSetCurrentUser(); 
//		PlayerPrefs.SetInt("CurrentTierId", 1);
//		PlayerPrefs.SetInt("CurrentExerciseId", 3);
//		PlayerPrefs.SetInt("CurrentSideId", 0);
		
//		RawImage rim = GetComponent<RawImage>();
////		movieTexture = (MovieTexture)rim.mainTexture;
		movieTexture = Resources.Load("Videos/" + UserDataObject.GetCurrentTierFileName() + "/"
		                              + UserDataObject.GetCurrentExerciseFileName() + "_"
		                              + UserDataObject.GetCurrentSide().direction) as MovieTexture;
//		movieTexture.Play();

		Debug.Log("stin: " + "Videos/" + UserDataObject.GetCurrentTierFileName() + "/"
		          + UserDataObject.GetCurrentExerciseFileName() + "_"
		          + UserDataObject.GetCurrentSide().direction);
		GetComponent<Renderer>().material.mainTexture = movieTexture;
		movieTexture.Play();
		movieTexture.loop = true;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
