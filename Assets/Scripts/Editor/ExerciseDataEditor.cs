using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class ExerciseDataEditor : EditorWindow
{
    public TierDataObject tierDataObject;
    private string _exerciseDataProductionProjectFilePath = "/StreamingAssets/JSONData/exerciseDataProduction.json";
    private string _exerciseDataTestProjectFilePath = "/StreamingAssets/JSONData/exerciseDataTest.json";
    private string _currentExerciseDataFilePath;
    
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
        
        if (tierDataObject != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            // property --> name of tierDataObject variable
            SerializedProperty serializedProperty = serializedObject.FindProperty("tierDataObject");
            
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data"))
            {
                SaveExerciseData(_currentExerciseDataFilePath);
            }
        }

        
        if (GUILayout.Button("Load Production Data"))
        {
            LoadExerciseData(_exerciseDataProductionProjectFilePath);
        }
        if (GUILayout.Button("Load Test Data"))
        {
            LoadExerciseData(_exerciseDataTestProjectFilePath);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    
     public void LoadExerciseData(string filepath)
    {
        _currentExerciseDataFilePath = filepath;

        string filePath = Application.dataPath + filepath;
        
        if (File.Exists(filePath))
        {
            string exerciseDataAsJson = File.ReadAllText(filePath);
            tierDataObject = JsonUtility.FromJson<TierDataObject>(exerciseDataAsJson);
        }
        else
        {
            tierDataObject = new TierDataObject();
        }
    }

    private void SaveExerciseData(string filepath)
    {
        string exerciseDataAsJson = JsonUtility.ToJson(tierDataObject);
        string filePath = Application.dataPath + filepath;
        
        File.WriteAllText(filePath, exerciseDataAsJson);
    }
}
