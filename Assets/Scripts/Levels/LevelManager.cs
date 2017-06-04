using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	[System.Serializable]
	// Information for the button
	public class Level
	{
		public string LevelText;
		public int UnLocked; // --> evtl boolwert?
		public bool IsInteractable;
	}

	public GameObject LevelButton;
	public Transform Spacer;
	public List<Level> LevelList;

	// Use this for initialization
	void Start () 
	{
		PlayerPrefs.SetInt("LevelAmount", LevelList.Count);
//	 	PlayerPrefs.DeleteAll(); // Deletes playerprefs
		
		FillList();
	}

	public int GetLevelListAmount()
	{
		return LevelList.Count;
	}

	void FillList()
	{
		foreach (var level in LevelList)
		{
			GameObject newButton = Instantiate(LevelButton) as GameObject;
			LevelButton button = newButton.GetComponent<LevelButton>();
			button.ButtonText.text = level.LevelText;

			if (PlayerPrefs.GetInt("Level" + button.ButtonText.text) == 1) // index statt text auf button
			{
				level.UnLocked = 1;
				level.IsInteractable = true;
			}

			button.Unlocked = level.UnLocked;
			button.GetComponent<Button>().interactable = level.IsInteractable;
			button.GetComponent<Button>().onClick.AddListener(() => LoadLevels("Level" + button.ButtonText.text));
			
			newButton.transform.SetParent(Spacer, false);
		}
		SaveAll();
	}

	void SaveAll()
	{
		// Initial PlayerPrefs creation for each Level if it is unlocked
		if (PlayerPrefs.HasKey("Level1"))
		{
			return;		
		}
		else
		{
			GameObject[] allButtons = GameObject.FindGameObjectsWithTag("LevelButton");
			foreach (GameObject buttons in allButtons)
			{
				LevelButton button = buttons.GetComponent<LevelButton>();
				PlayerPrefs.SetInt("Level" + button.ButtonText.text, button.Unlocked);
			}
			
		}
	}

	void LoadLevels(string value)
	{
		SceneManager.LoadScene(value);
	}
	
}
