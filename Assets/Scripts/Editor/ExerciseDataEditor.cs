using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class ExerciseDataEditor : EditorWindow
{
    public TierDataObject currentTier;
    private string _allExerciseDataProjectFilePath = "/StreamingAssets/JSONData/Exercises/";
    private string _currentExerciseDataFilePath  = "/StreamingAssets/JSONData/Exercises/";
    
    private string _exerciseDataProductionProjectFilePath = "/StreamingAssets/JSONData/exerciseDataProduction.json";
    private string _exerciseDataTestProjectFilePath = "/StreamingAssets/JSONData/exerciseDataTest.json";
    
    private Vector2 scrollPos;
    
    [MenuItem ("Window/Exercise Data Editor")]
    private static void Init()
    {
        ExerciseDataEditor window = (ExerciseDataEditor)EditorWindow.GetWindow(typeof(ExerciseDataEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + _allExerciseDataProjectFilePath);
        
        if (currentTier != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            // property --> name of tierDataObject variable
            SerializedProperty serializedProperty = serializedObject.FindProperty("currentTier");
            
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data"))
            {
                SaveExerciseData(currentTier);
            }
        }

        EditorGUILayout.LabelField("ALL AVAILABLE EXERCISES");
        LoadAllExercises(directoryInfo);

//        if (GUILayout.Button("Load Production Data"))
//        {
//            LoadExerciseData(_exerciseDataProductionProjectFilePath);
//        }
//        if (GUILayout.Button("Load Test Data"))
//        {
//            LoadExerciseData(_exerciseDataTestProjectFilePath);
//        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    // Load all available exercise json files and create a button for each
    public void LoadAllExercises(DirectoryInfo directoryInfo)
    {
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (var file in files)
        {
            if (file.Extension.Contains(".json"))
            {
                if (GUILayout.Button("Load " + file.Name))
                {
                    _currentExerciseDataFilePath = _allExerciseDataProjectFilePath;
                    Debug.Log(_currentExerciseDataFilePath);
                    _currentExerciseDataFilePath += file.Name;
                    Debug.Log(_currentExerciseDataFilePath);
                    
                    LoadExerciseData(_currentExerciseDataFilePath);
                }
            }
        }
    }
    
    // Load clicked exercise
    public void LoadExerciseData(string filepath)
    {

        string filePath = Application.dataPath + filepath;
        
        if (File.Exists(filePath))
        {
            string exerciseDataAsJson = File.ReadAllText(filePath);
            currentTier = JsonUtility.FromJson<TierDataObject>(exerciseDataAsJson);
        }
        else
        {
            currentTier= new TierDataObject();
        }
    }

    private void SaveExerciseData(TierDataObject tierDataObject)
    {
        string exerciseDataAsJson = JsonUtility.ToJson(tierDataObject);
        string filePath = Application.dataPath + _currentExerciseDataFilePath;
        
        File.WriteAllText(filePath, exerciseDataAsJson);
    }
}
