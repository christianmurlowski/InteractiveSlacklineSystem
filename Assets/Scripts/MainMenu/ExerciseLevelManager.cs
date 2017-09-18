using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseLevelManager : MonoBehaviour
{
	public GameObject exerciseLevelButton,
					  KinectManager,
					  HandCursor;
	public Transform spacerHorizontal;
	public Image progressImage;
	public ScrollRect scrollView;
	public Scrollbar scrollBar;
	public Text textHandLeft,
				textHandRight,
				textHandLeftX,
				textHandRightX;
	
	private KinectManager _kinectManager;
	private InteractionManager _interactionManager;
	private KinectInterop.JointType _jointHandRight,
									_jointHandLeft;
	
	private TierData _currTier;
	private List<TierData> _allTierData;
	private ExerciseData[] _allTierExercises;

	private Image imageHandCursor;
	
	// Use this for initialization
	void Start ()
	{
		
		KinectManager = GameObject.Find("KinectManager");
		_kinectManager = KinectManager.GetComponent<KinectManager>();
		_interactionManager = KinectManager.GetComponent<InteractionManager>();

		HandCursor = GameObject.Find("ImageHandCursor");
		imageHandCursor = HandCursor.GetComponent<Image>();
		
		if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;
		
		_jointHandRight = KinectInterop.JointType.HandRight;
		_jointHandLeft = KinectInterop.JointType.HandLeft;
		
		_currTier = UserDataObject.GetCurrentTier();
			
		FillMenu();
//		ScrollToCurrentTier();
	}

	private void Update()
	{
//		if (imageHandCursor.transform.localPosition.x > 850) scrollBar.value +=  Mathf.Lerp(0, 1, 0.01f);
//		else if (imageHandCursor.transform.localPosition.x < -850) scrollBar.value -= Mathf.Lerp(0, 1, 0.01f);

		if (_interactionManager.GetCursorPosition().x > 0.9) scrollBar.value +=  Mathf.Lerp(0, 1, 0.01f);
		else if (_interactionManager.GetCursorPosition().x < 0.1) scrollBar.value -= Mathf.Lerp(0, 1, 0.01f);
	}

	public void LoadPreviousScene()
	{
		SceneManager.LoadScene("TierMenu");
	}

	void FillMenu()
	{
		// For current tier create tier basics, exercises and summary button

		//Text for the tier
//		_tierText.text = tier.tierName;

		// Instantiate a button gameobject from a prefab
		GameObject tierBasicGameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
		// Get the script for the button to set its values
		ExerciseLevelButton tierBasicButton = tierBasicGameObjectButton.GetComponent<ExerciseLevelButton>();

		tierBasicButton.buttonText.text = "Introduction";
		
		// Set image for basic information button
		tierBasicGameObjectButton.GetComponent<Image>().sprite =
			Resources.Load<Sprite>("Images/Information/" + _currTier.fileName);
		tierBasicButton.bgImage.sprite = Resources.Load<Sprite>("Images/Information/InformationIcon");
		tierBasicButton.bgImage.color = new Color32(255, 255, 255, 20);


		// If tier unlocked --> unlock the basic information for this tier
		if (_currTier.isInteractable)
		{
			tierBasicButton.unlocked = 1;
			tierBasicButton.GetComponent<Button>().interactable = true;
		}
		else
		{
			tierBasicButton.unlocked = 0;
			tierBasicButton.GetComponent<Button>().interactable = false;
			tierBasicGameObjectButton.GetComponent<Image>().color = new Color32(150,150,150, 255);
		}

		// Add listener for the button to load the appropriate scene
		tierBasicButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			SceneManager.LoadScene("TierInfo");
		});

		// Append the basic information button to the exercise spacer
		tierBasicGameObjectButton.transform.SetParent(spacerHorizontal, false);

		// Manage exercise data in the current tier
		foreach (var exercise in _currTier.exercises)
		{
			// Instantiate a button gameobject from a prefab
			GameObject gameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			// Get the script for the button to set its values
			ExerciseLevelButton button = gameObjectButton.GetComponent<ExerciseLevelButton>();

			// Set the values regarding the current exercise
			button.buttonText.text = exercise.exerciseName;
			button.unlocked = exercise.unlocked;
			button.GetComponent<Button>().interactable = exercise.isInteractable;

			// Set image for exercise button
//				Debug.Log("Images/" + tier.fileName + "/" + exercise.fileName);
			button.bgImage.sprite = Resources.Load<Sprite>("Images/" + _currTier.fileName + "/" + exercise.fileName);
			
			// If exercise locked
			if (!exercise.isInteractable)
			{
				button.bgImage.color = MainColors.WhiteTransparent(3);
				gameObjectButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/LockExercise2");
				gameObjectButton.GetComponent<Image>().color = MainColors.Grey();
			}
			
			// Fill progress in button
			foreach(var side in exercise.sides){
				if(side.accomplished)
				{
					button.progressImage.fillAmount += 0.5f;
				}
			}

			// Add listener for the button to set the PP and load the appropriate scene
			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt("CurrentExerciseId", _currTier.exercises.IndexOf(exercise));
				SceneManager.LoadScene("ExerciseSideSelection");
			});

			// Highlight current exercise
			if (PlayerPrefs.GetInt("CurrentExerciseId") == _currTier.exercises.IndexOf(exercise))
			{
				Debug.Log("Current exercise to highlight: " + exercise.exerciseName);
			}
				
			// Append the exercise button to the exercise spacer
			gameObjectButton.transform.SetParent(spacerHorizontal, false);
		}

		// Instantiate a button gameobject from a prefab
		GameObject tierSumGameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
		// Get the script for the button to set its values
		ExerciseLevelButton tierSumButton = tierSumGameObjectButton.GetComponent<ExerciseLevelButton>();

		// Set the values regarding the summary
		tierSumButton.buttonText.text = "Summary";

		// Set image for summary button
		tierSumGameObjectButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Summary/SummaryIcon");
		tierSumButton.bgImage.sprite = Resources.Load<Sprite>("Images/Summary/SummaryTierBG");
		tierSumButton.bgImage.color = new Color32(255, 255, 255, 15);
		
		// If last exercise accomplished --> unlock the summary for this tier
		if (_currTier.exercises.Last().accomplished)
		{
			tierSumButton.unlocked = 1;
			tierSumButton.GetComponent<Button>().interactable = true;
		}
		else
		{
			tierSumButton.unlocked = 0;
			tierSumButton.GetComponent<Button>().interactable = false;
			tierSumButton.GetComponent<Image>().color = new Color32(150,150,150, 255);
		}

		// Add listener for the button to load the appropriate scene
		tierSumButton.GetComponent<Button>().onClick.AddListener(() =>
		{
			SceneManager.LoadScene("TierSummary");
		});

		// Append the summary button to the exercise spacer
		tierSumGameObjectButton.transform.SetParent(spacerHorizontal, false);
		
//		SaveData();
	}

	private void ScrollToCurrentTier()
	{

		switch (PlayerPrefs.GetInt("CurrentTierId"))
		{
			case 0:
				scrollView.verticalNormalizedPosition = 1f;
				break;
			case 1:
				scrollView.verticalNormalizedPosition = 0.52f;
				break;
			case 2:
				scrollView.verticalNormalizedPosition = 0.17f;
				break;
			case 3:
				scrollView.verticalNormalizedPosition = 0f;
				break;
			default:
				scrollView.verticalNormalizedPosition = 1f;
				break;
		}
	}
	
	// Initial PlayerPrefs creation for each Level --> set unlock value
	void SaveData()
	{
		if (PlayerPrefs.HasKey("Exercise0Unlocked"))
		{
			Debug.Log("Exercise0Unlocked exists");
			return;		
		}
		else
		{
			Debug.Log("Exercise0Unlocked exists NOT");
			GameObject[] allButtons = GameObject.FindGameObjectsWithTag("ExerciseButton");
			foreach (GameObject buttons in allButtons)
			{
				ExerciseLevelButton button = buttons.GetComponent<ExerciseLevelButton>();
				PlayerPrefs.SetInt("Exercise" + button.buttonText.text + "Unlocked", button.unlocked);
			}
			
		}
	}
	
}
