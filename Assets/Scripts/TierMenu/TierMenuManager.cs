using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TierMenuManager : MonoBehaviour
{

	public GameObject tierMenuButton,
					  KinectManager;

	public Transform spacerHorizontal;

	private KinectManager _kinectManager;
	private List<TierData> _allTiers;
	
	// Use this for initialization
	void Start () {
		
		KinectManager = GameObject.Find("KinectManager");

		_kinectManager = KinectManager.GetComponent<KinectManager>();

		if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;

		_allTiers = UserDataObject.GetAllTiers();
		
		FillTierMenu();	
	}

	private void Update()
	{
		// If left arrow pressed --> side selection
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			LoadPreviousScene();
		}
	}

	private void FillTierMenu()
	{
		foreach (var tier in _allTiers)
		{
			GameObject goTierMenuButton = Instantiate(tierMenuButton) as GameObject;
			
			TierMenuButton tierButton = goTierMenuButton.GetComponent<TierMenuButton>();

			tierButton.buttonText.text = tier.tierName;
			tierButton.buttonNumber.text = (_allTiers.IndexOf(tier) + 1).ToString();
			tierButton.GetComponent<Button>().interactable = tier.isInteractable;
			
			// Set the image regarding interactability
			goTierMenuButton.GetComponent<Image>().sprite = 
				Resources.Load<Sprite>("Images/TierMenu/" + tier.fileName + "1");

			if (!tier.isInteractable)
			{
				tierButton.buttonBgImage.color = MainColors.WhiteTransparent(110);
				tierButton.buttonBgImage.sprite = 
					Resources.Load<Sprite>("Images/LockTier3");
			}
			
			tierButton.GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt("CurrentTierId", _allTiers.IndexOf(tier));
				PlayerPrefs.SetString("CurrentTierFileName", tier.fileName);
				
				SceneManager.LoadScene("MainMenu");
			});
			
			goTierMenuButton.transform.SetParent(spacerHorizontal, false);
		}
	}
	
	public void LoadPreviousScene()
	{
		SceneManager.LoadScene("UserSelection");
	}
}
