using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text;

public class Main : MonoBehaviour {
    Script script;

    List<Text> choiceTexts = new List<Text>();
    List<Button> choiceButtons = new List<Button>();
    Text textField;
    Text debugField;

    DynValue currentChoiceCallback;

    int debugLinesTop = -1;
    string[] debugLines = new string[20];

    public void OnChoice(int choice)
    {
        script.Call(currentChoiceCallback, choice + 1);
    }

    void LuaReadAllFiles(string folder)
    {
        foreach (var file in Directory.GetFiles(folder, "*.lua", SearchOption.AllDirectories))
        {
            try
            {
                string code = File.ReadAllText(file, System.Text.Encoding.UTF8);
                script.DoString(code, null, file);
            }
            catch (SyntaxErrorException e)
            {
                Debug.LogError("Lua Syntax Error: " + e.DecoratedMessage);
            }
            catch (ScriptRuntimeException e)
            {
                Debug.LogError("Lua Runtime Error: " + e.DecoratedMessage);
            }
        }
    }

    // Use this for initialization
    void Start () {
        textField = transform.Find("Text").GetComponent<Text>();
        debugField = transform.Find("Console").GetComponent<Text>();


        for (int i = 1; i <= 4; i++)
        {
            choiceTexts.Add(transform.Find("Choice" + i).Find("Text").GetComponent<Text>());
            choiceButtons.Add(transform.Find("Choice" + i).GetComponent<Button>());
        }
        




        Script.DefaultOptions.DebugPrint = s => Debug.LogError(s);
        script = new Script();
        script.Globals["SetNewChoice"] = (Action<string, DynValue, Table>)SetNewChoice;
        script.Globals["Log"] = (Action<string>)Log;
        script.Globals["data"] = new Table(script);

        try
        {
            string code = File.ReadAllText("scripts/main.lua", System.Text.Encoding.UTF8);
            script.DoString(code, null, "scripts/main.lua");
        }
        catch (SyntaxErrorException e)
        {
            Debug.LogError("Lua Syntax Error: " + e.DecoratedMessage);
        }
        catch (ScriptRuntimeException e)
        {
            Debug.LogError("Lua Runtime Error: " + e.DecoratedMessage);
        }

        //LuaReadAllFiles("scripts");
    }

    public void SetNewChoice(string text, DynValue callback, Table choices)
    {
        currentChoiceCallback = callback;
        textField.text = text;
        int i = 1;
        for (; i <= 4; i++)
        {
            choiceButtons[i - 1].gameObject.SetActive(true);
            string buttonText = choices[i] as string;
            if (buttonText == null) break;
            choiceTexts[i - 1].text = buttonText;
        }
        for (; i <= 4; i++)
        {
            choiceButtons[i - 1].gameObject.SetActive(false);
        }
    }

    public void Log(string line)
    {
        debugLinesTop = (debugLinesTop + 1) % debugLines.Length;

        debugLines[debugLinesTop] = line;

        UpdateDebugView();
    }

    private void UpdateDebugView()
    {
        StringBuilder sb = new StringBuilder();
        int current = (debugLinesTop + 1) % debugLines.Length;
        for (int i = 0; i < debugLines.Length; i++)
        {
            string line = debugLines[current];
            if (line != null && line != "")
            {
                sb.AppendLine(line);
            }
            current = (current + 1) % debugLines.Length;
        }
        debugField.text = sb.ToString();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
