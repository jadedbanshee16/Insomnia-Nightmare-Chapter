using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class GenerateInitialState : EditorWindow
{
    [MenuItem("Window/State Generation")]
    public static void ShowWindow()
    {
        GetWindow<GenerateInitialState>("State Generation");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate initial state"))
        {
            generateId();
            GenerateState();
        }

        if(GUILayout.Button("Safe default controls"))
        {
            generateControls();
        }
    }

    private void GenerateState()
    {        
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<WorldStateManager>().generateInitialState();
    }

    private void generateId()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().setInteractables();
    }

    private void generateControls()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().saveControls(true);
    }
}
