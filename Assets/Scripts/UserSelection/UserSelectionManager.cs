using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserSelectionManager : MonoBehaviour
{

    public GameObject userSelectionButton;
    public Transform spacer;
    
    public static UserDataObject currentUserDataObject;

    private string allUsersFilePath = "/StreamingAssets/JSONData/Users/";

    void Start()
    {        
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
        currentUserDataObject.SetCurrentUser(user);
        Debug.Log(currentUserDataObject.GetCurrentUser());        
        Debug.Log(currentUserDataObject.GetCurrentUser().name);        
        
        SceneManager.LoadScene("MainMenu");
    }
    
}


