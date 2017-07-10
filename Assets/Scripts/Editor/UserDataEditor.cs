﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UserDataEditor : EditorWindow
{
    public UserData currentUser;
    private string allUserDataProjectFilePath = "/StreamingAssets/JSONData/Users/";
    private string currentUserDataFilePath = "/StreamingAssets/JSONData/Users/";
    
    
    private string _exerciseDataProductionProjectFilePath = "/StreamingAssets/JSONData/exerciseDataProduction.json";
    private string _exerciseDataTestProjectFilePath = "/StreamingAssets/JSONData/exerciseDataTest.json";

    private string userName;

    private ExerciseDataEditor _exerciseDataEditor;

    private Vector2 scrollPos;

    [MenuItem("Window/User Data Editor")]
    private static void init()
    {        
        UserDataEditor window = (UserDataEditor) EditorWindow.GetWindow(typeof(UserDataEditor));
        window.Show();
    }

    void OnGUI()
    {        
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + allUserDataProjectFilePath);

        if (currentUser != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            // FindProperty --> name of currentUser variable
            SerializedProperty serializedProperty = serializedObject.FindProperty("currentUser");
            
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (currentUser != null)
            {
                if (GUILayout.Button("Save Data"))
                {
                    SaveUserData(currentUser);
                }                
            }

            if (GUILayout.Button("Load Production Exercises Data"))
            {
                LoadExercisesIntoUserData(_exerciseDataProductionProjectFilePath);
            }            
            
            if (GUILayout.Button("Load Test Exercises Data"))
            {
                LoadExercisesIntoUserData(_exerciseDataTestProjectFilePath);
            }
            
            if (GUILayout.Button("Delete User"))
            {
                DeleteUser(directoryInfo);
            }
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
        }
        
        EditorGUILayout.LabelField("ALL AVAILABLE USERS");
        LoadAllUsers(directoryInfo);

        EditorGUILayout.Space();
        EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("CREATE A NEW USER");
        userName = EditorGUILayout.TextField("Username:", userName);
        if (GUILayout.Button("Create User"))
        {
            CreateNewUser(userName);
        }
//        Debug.Log(GetNumberOfUsers(directoryInfo));

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void CreateNewUser(string username)
    {
        if (username != null)
        {
            currentUser = new UserData();
            currentUser.name = username;
            currentUserDataFilePath = allUserDataProjectFilePath;
            currentUserDataFilePath += currentUser.name.Replace(" ", "") + ".json";
        }
    }

    private void LoadAllUsers(DirectoryInfo directoryInfo)
    {
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (var file in files)
        {
            if (file.Extension.Contains("json"))
            {    
                if (GUILayout.Button("Load " + file.Name))
                {
                    currentUserDataFilePath = allUserDataProjectFilePath;

                    Debug.Log(currentUserDataFilePath);
                    currentUserDataFilePath += file.Name;
                    Debug.Log(currentUserDataFilePath);
                    
                    LoadUserData(currentUserDataFilePath);
                }

            }
        }
    }

    
    private void LoadUserData(string dataFilePath)
    {
        string filePath = Application.dataPath + dataFilePath;

        if (File.Exists(filePath))
        {
            string currentUserDataAsJson = File.ReadAllText(filePath);
            currentUser = JsonUtility.FromJson<UserData>(currentUserDataAsJson);
        }
        else
        {
            currentUser = new UserData();
        }
    }

    private void SaveUserData(UserData currentUserData)
    {
        string currentUserDataAsJson = JsonUtility.ToJson(currentUserData);
        string filePath = Application.dataPath + currentUserDataFilePath;
        
        File.WriteAllText(filePath, currentUserDataAsJson);
    }

    private void LoadExercisesIntoUserData(string filePath)
    {
        _exerciseDataEditor = ScriptableObject.CreateInstance<ExerciseDataEditor>();
        _exerciseDataEditor.LoadExerciseData(filePath);
        
        currentUser.tierData = _exerciseDataEditor.tierDataObject.tierDataList;
    }

    private void DeleteUser(DirectoryInfo directoryInfo)
    {
        string filePath = Application.dataPath + currentUserDataFilePath;
        File.Delete(filePath);
        currentUser = null;
        
    }


}
