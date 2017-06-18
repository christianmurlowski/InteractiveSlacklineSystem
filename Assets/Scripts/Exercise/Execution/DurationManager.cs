using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DurationManager : MonoBehaviour
{

	public Image durationImage;
	public Text counterText;
	
	private float tempTimer;

	private Color32 red, darkOrange, lightOrange, green;

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
	}

	public void StartTimer()
	{
		durationImage.fillAmount += Time.deltaTime / 5;

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
		
		StartCounter();
	}

	private void StartCounter()
	{
		tempTimer += Time.deltaTime;
		counterText.text = Mathf.Round(tempTimer).ToString();
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
