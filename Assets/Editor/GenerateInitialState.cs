using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class GenerateInitialState : EditorWindow
{
    [MenuItem("Window/StateGeneration")]
    public static void ShowWindow()
    {
        GetWindow<GenerateInitialState>("StateGeneration");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate initial state"))
        {
            GenerateState();
        }
    }

    private void GenerateState()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldStateManager>().generateInitialState();
    }
}
