using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserSelectionManager : MonoBehaviour
{

    public GameObject userSelectionButton,
                      KinectManager;
    
    public Transform spacer;
    
    public static UserDataObject currentUserDataObject;

    private KinectManager _kinectManager;
    private string allUsersFilePath = "/StreamingAssets/JSONData/Users/";

    void Start()
    {
        KinectManager = GameObject.Find("KinectManager");
        _kinectManager = KinectManager.GetComponent<KinectManager>();
        if (!_kinectManager.displayUserMapSmall) _kinectManager.displayUserMapSmall = true;
            
        currentUserDataObject = new UserDataObject();
        LoadAllUsers();
    }
    
    private void LoadAllUsers()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + allUsersFilePath);
        FileInfo[] files = directoryInfo.GetFiles();

        foreach (var file in files)
        {
            if (file.Extension.Contains("json"))
            {
                string filePath = Application.dataPath + allUsersFilePath + file.Name;
                Debug.Log(filePath);
                string alluserAsJson = File.ReadAllText(filePath);
    
                UserData tempUserData = JsonUtility.FromJson<UserData>(alluserAsJson);

                GameObject gameObjectButton = Instantiate(userSelectionButton) as GameObject;
                UserSelectionButton button = gameObjectButton.GetComponent<UserSelectionButton>();
    
                button.buttonText.text = tempUserData.name;
                button.GetComponent<Button>().onClick.AddListener(() => ClickOnUser(tempUserData, filePath));
                
                gameObjectButton.transform.SetParent(spacer, false);
            }
        }
    }

    private void ClickOnUser(UserData user, string filePath)
    {
        PlayerPrefs.SetString("CurrentUser", user.name);
        PlayerPrefs.SetString("CurrentUserFilePath", filePath);
        PlayerPrefs.SetInt("CurrentTierId", 0);

        currentUserDataObject.SetCurrentUser(user);
        Debug.Log(currentUserDataObject.GetCurrentUser());        
        Debug.Log(currentUserDataObject.GetCurrentUser().name);        
        
        SceneManager.LoadScene("TierMenu");
    }

    // TODO Just for test purposes -> Delete in production
    public static void TestSetCurrentUser()
    {        
        string allUsersFilePath = "/StreamingAssets/JSONData/Users/";

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + allUsersFilePath);
        FileInfo[] files = directoryInfo.GetFiles();
        currentUserDataObject = new UserDataObject();

        foreach (var file in files)
        {
            if (file.Extension.Contains("json"))
            {
                string filePath = Application.dataPath + allUsersFilePath + file.Name;
                string alluserAsJson = File.ReadAllText(filePath);
    
                UserData tempUserData = JsonUtility.FromJson<UserData>(alluserAsJson);

                if (tempUserData.isCurrentUser == true)
                {
                    Debug.Log("TEST CURRENT USER: " + tempUserData.name);
                    currentUserDataObject.SetCurrentUser(tempUserData);
                    PlayerPrefs.SetString("CurrentUser", tempUserData.name);
                    PlayerPrefs.SetString("CurrentUserFilePath",  filePath);
                    PlayerPrefs.SetInt("CurrentTierId", 0);					

                }
            }
        }
    }
    
}


