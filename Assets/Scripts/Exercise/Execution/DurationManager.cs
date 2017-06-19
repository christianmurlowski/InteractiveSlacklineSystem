using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class DurationManager : MonoBehaviour
{

	public Image durationImage;
	public Text counterText;
	
	private float tempTimer;

	private Color32 red, darkOrange, lightOrange, green;

	private Stopwatch _attemptExecutionTime;
	private Stopwatch _attemptOverallTime;

	// TODO fillring
	// Use this for initialization
	void Start ()
	{
		durationImage.GetComponent<Image>();
		counterText.GetComponent<Text>();
		
		durationImage.type = Image.Type.Filled;
		durationImage.fillMethod = Image.FillMethod.Radial360;
		durationImage.fillAmount = 0f;
		tempTimer = 0f;		
		
		// Colors
		red = new Color32(227, 63, 34, 255);
		darkOrange = new Color32(227, 126, 34, 255);
		lightOrange = new Color32(231, 201, 80, 255);
		green = new Color32(91, 175, 76, 255);
		
		_attemptExecutionTime = new Stopwatch();
		
	}

	public void StartTimer()
	{
		if (!_attemptExecutionTime.IsRunning)
		{
			_attemptExecutionTime.Start();		
		}
		var _attemptExecutionTimeInt = Mathf.FloorToInt(_attemptExecutionTime.ElapsedMilliseconds * 0.001f);
		counterText.text = _attemptExecutionTimeInt.ToString();

		durationImage.fillAmount = _attemptExecutionTime.ElapsedMilliseconds * 0.001f /5;

		if (durationImage.fillAmount >= 1.0)
		{
			changeColor(green);
		}
		else if (durationImage.fillAmount > 0.66)
		{
			changeColor(lightOrange);
		}
		else if (durationImage.fillAmount > 0.33)
		{
			changeColor(darkOrange);
		}
		
	}

	public void StopTimer()
	{
		_attemptExecutionTime.Reset();
		counterText.text = "0";
		durationImage.fillAmount = 0.0f;
	}
	
	private void StartCounter()
	{
//		tempTimer += Time.deltaTime;
//		counterText.text = Mathf.Round(tempTimer).ToString();
//		counterText.text = _stopWatch.Elapsed;
	}
	public void changeColor(Color32 color)
	{
		durationImage.color = color;
		counterText.color = color;
	}
	
	


// Update is called once per frame
	void Update () {
//		StartTimer();		
	}
}
