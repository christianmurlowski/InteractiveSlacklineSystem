using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class ExerciseDataEditor : EditorWindow
{

    public ExerciseObject exerciseDataObject;
    private string exerciseDataProjectFilePath = "/StreamingAssets/exerciseData.json";

    [MenuItem ("Window/Game Data Editor")]
    private static void Init()
    {
        ExerciseDataEditor window = (ExerciseDataEditor)EditorWindow.GetWindow(typeof(ExerciseDataEditor));
        window.Show();
    }

    void OnGui()
    {
        
    }
    
    private void LoadExerciseData()
    {
        string filePath = Application.dataPath + exerciseDataProjectFilePath;
        
        if (File.Exists(filePath))
        {
            string exerciseDataAsJson = File.ReadAllText(filePath);
            exerciseDataObject = JsonUtility.FromJson<ExerciseObject>(exerciseDataAsJson);
        }
        else
        {
            exerciseDataObject = new ExerciseObject();
        }
    }

    private void SaveExerciseData()
    {
        
        string exerciseDataAsJson = JsonUtility.ToJson(exerciseDataObject);
        string filePath = Application.dataPath + exerciseDataProjectFilePath;
        
        File.WriteAllText(filePath, exerciseDataAsJson);
    }
    
}
