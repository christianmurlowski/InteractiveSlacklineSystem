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
            // write into textfile Todo can be deleted for production
//            WriteIntoText();
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

    // Write into textfile Todo can be deleted for production
    private void WriteIntoText()
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + _allExerciseDataProjectFilePath + "/test.txt", true);
        
        foreach (var tier in currentTier.tierDataList)
        {
            Debug.Log("");
            Debug.Log("");
            Debug.Log("-----");
            Debug.Log("-----");
            Debug.Log("TIER");
            Debug.Log(tier.tierName + " || " +tier.fileName);
            writer.WriteLine(" ");
            writer.WriteLine(" ");
            writer.WriteLine("-----");
            writer.WriteLine(tier.tierName + " || " +tier.fileName);
            
            Debug.Log("----------");
            Debug.Log("Goals: ");
            writer.WriteLine("----------");
            writer.WriteLine("Goals");
            foreach (var goal in tier.goals)
            {
                Debug.Log(goal.description);
                writer.WriteLine(goal.description);
            }
            
            Debug.Log("");
            Debug.Log("----------");
            Debug.Log("Tips:");
            writer.WriteLine("");
            writer.WriteLine("----------");
            writer.WriteLine("Tips");
            foreach (var tip in tier.tips)
            {
                writer.WriteLine(tip.description);
                Debug.Log(tip.description);
            }
            
            Debug.Log("");
            Debug.Log("--------------------");
            Debug.Log("Exercises:");
            writer.WriteLine("");
            writer.WriteLine("--------------------");
            writer.WriteLine("Exercises");
            foreach (var exercise in tier.exercises)
            {
                Debug.Log("");
                Debug.Log("----------------------------------------");
                Debug.Log(exercise.fileName + " || " + exercise.exerciseName);                    
                writer.WriteLine("");
                writer.WriteLine("----------------------------------------");
                writer.WriteLine(exercise.fileName + " || " + exercise.exerciseName);
               
                foreach (var side in exercise.sides)
                {                
                    Debug.Log("--------------------------------------------------------------------------------");
                    Debug.Log(exercise.fileName + " || " + exercise.exerciseName);
                    Debug.Log("Side Tip: ");
                    Debug.Log(side.direction);             
                    writer.WriteLine("--------------------------------------------------------------------------------");
                    writer.WriteLine(exercise.fileName + " || " + exercise.exerciseName);
                    writer.WriteLine("Side Tip: ");
                    writer.WriteLine(side.direction);
                    foreach (var tip in side.tips)
                    {
                        writer.WriteLine(tip.description);
                        Debug.Log(tip.description);
                    }
                }
            }
        }
        
        writer.Close();
    }
}
