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
					  KinectManager;
	
	public Transform tierSpacer,
	                 exerciseSpacer,
	                 verticalSpacer;
	
	public Image progressImage;

	public ScrollRect scrollView;

	private KinectManager _kinectManager;
	
	private List<TierData> _allTierData;
	private ExerciseData[] _allTierExercises;
	
	// Use this for initialization
	void Start ()
	{
		_allTierData = UserDataObject.GetAllTiers();
		
		KinectManager = GameObject.Find("KinectManager");
		_kinectManager = KinectManager.GetComponent<KinectManager>();

		if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;
		
		FillMenu();
		ScrollToCurrentTier();
	}

	public void LoadPreviousScene()
	{
		SceneManager.LoadScene("UserSelection");
	}

	void FillMenu()
	{
		// For each tier create tier basics, exercises and tier summary button
		foreach (var tier in _allTierData)
		{
			// Instantiate the tier and exercise spacer
			Transform _tierSpacer = Instantiate(tierSpacer);
			Transform _exerciseSpacer = Instantiate(exerciseSpacer);

			//Text for the tier
			Text _tierText = _tierSpacer.GetComponentInChildren<Text>();
			_tierText.text = tier.tierName;

			// Instantiate a button gameobject from a prefab
			GameObject tierBasicGameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			// Get the script for the button to set its values
			ExerciseLevelButton tierBasicButton = tierBasicGameObjectButton.GetComponent<ExerciseLevelButton>();

			tierBasicButton.buttonText.text = "Introduction";
			
			// Set image for basic information button
			tierBasicGameObjectButton.GetComponent<Image>().sprite =
				Resources.Load<Sprite>("Images/Information/" + tier.fileName);
			tierBasicButton.bgImage.sprite = Resources.Load<Sprite>("Images/Information/Information");
			tierBasicButton.bgImage.color = new Color32(255, 255, 255, 20);


			// If tier unlocked --> unlock the basic information for this tier
			if (tier.isInteractable)
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

			// Add listener for the button to set tier pp and load the appropriate scene
			tierBasicButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt("CurrentTierId", _allTierData.IndexOf(tier));
				Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));

				SceneManager.LoadScene("TierInfo");
			});


			// Append the basic information button to the exercise spacer
			tierBasicGameObjectButton.transform.SetParent(_exerciseSpacer, false);

			// Manage exercise data in the current tier
			foreach (var exercise in tier.exercises)
			{
				// Instantiate a button gameobject from a prefab
				GameObject gameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
				// Get the script for the button to set its values
				ExerciseLevelButton button = gameObjectButton.GetComponent<ExerciseLevelButton>();

				// Set the values regarding the current exercise
				button.buttonText.text = exercise.exerciseName;

				//			Debug.Log("index of " + exercise.exerciseName + ": " + Array.IndexOf(_allExercises, exercise));
				//
				//			if (exercise.isInteractable)
				//			{
				//				button.unlocked = exercise.unlocked;
				//				button.GetComponent<Button>().interactable = exercise.isInteractable;
				//			}
				// If PP key from current exercise is unlocked --> unlock and make current exercise interactive
				//			if (PlayerPrefs.GetInt("Exercise" + Array.IndexOf(_allExercises, exercise) + "Unlocked")  == 1)
				//			{
				//				exercise.isInteractable = true;
				//				exercise.unlocked = 1;
				//			}

				button.unlocked = exercise.unlocked;
				button.GetComponent<Button>().interactable = exercise.isInteractable;

				// Set image for exercise button
//				Debug.Log("Images/" + tier.fileName + "/" + exercise.fileName);
				button.bgImage.sprite = Resources.Load<Sprite>("Images/" + tier.fileName + "/" + exercise.fileName);
				
				// If exercise locked
				if (!exercise.isInteractable)
				{
					button.bgImage.color = new Color32(255, 255, 255, 0);
					gameObjectButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Lock");
					gameObjectButton.GetComponent<Image>().color = new Color32(255,255,255, 255);
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
					PlayerPrefs.SetInt("CurrentTierId", _allTierData.IndexOf(tier));
					PlayerPrefs.SetInt("CurrentExerciseId", tier.exercises.IndexOf(exercise));
					PlayerPrefs.SetString("CurrentTierFileName", tier.fileName);

					SceneManager.LoadScene("ExerciseSideSelection");
				});

				if (PlayerPrefs.GetInt("CurrentTierId") == _allTierData.IndexOf(tier) && 
				    PlayerPrefs.GetInt("CurrentExerciseId") == tier.exercises.IndexOf(exercise))
				{
					Debug.Log("Current exervcise to highlight: " + exercise.exerciseName);
				}
					

				// Append the exercise button to the exercise spacer
				gameObjectButton.transform.SetParent(_exerciseSpacer, false);
			}

			// Instantiate a button gameobject from a prefab
			GameObject tierSumGameObjectButton = Instantiate(exerciseLevelButton) as GameObject;
			// Get the script for the button to set its values
			ExerciseLevelButton tierSumButton = tierSumGameObjectButton.GetComponent<ExerciseLevelButton>();

			// Set the values regarding the summary
			tierSumButton.buttonText.text = "Summary";

			// Set image for summary button
			tierSumGameObjectButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Summary/Summary");
			tierSumButton.bgImage.sprite = Resources.Load<Sprite>("Images/Summary/SummaryBackground");
			tierSumButton.bgImage.color = new Color32(255, 255, 255, 15);
			
			// If last exercise accomplished --> unlock the sum information for this tier
			if (tier.exercises.Last().accomplished)
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

			// Add listener for the button to set tier PP and load the appropriate scene
			tierSumButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt("CurrentTierId", _allTierData.IndexOf(tier));

				Debug.Log("CurrentTierId: " + PlayerPrefs.GetInt("CurrentTierId"));

				SceneManager.LoadScene("TierSummary");
			});

			// Append the summary button to the exercise spacer
			tierSumGameObjectButton.transform.SetParent(_exerciseSpacer, false);

			// Append the exercise spacer to the tier spacer and tier spacer to the root spacer
			_exerciseSpacer.transform.SetParent(_tierSpacer, false);
			_tierSpacer.transform.SetParent(verticalSpacer, false);
		}
		
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
