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

    List<DynValue> currentCallbacks = new List<DynValue>();
    List<Text> choiceTexts = new List<Text>();
    List<Button> choiceButtons = new List<Button>();
    Text textField;
    Text debugField;

    

    int debugLinesTop = -1;
    string[] debugLines = new string[20];

    public void OnChoice(int choice)
    {
        try
        {
            script.Call(currentCallbacks[choice]);
        }
        catch (SyntaxErrorException e)
        {
            Log("Lua Syntax Error: " + e.DecoratedMessage);
        }
        catch (ScriptRuntimeException e)
        {
            Log("Lua Runtime Error: " + e.DecoratedMessage);
        }
        catch (Exception e)
        {
            Log(e.Message);
        }
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
                Log("Lua Syntax Error: " + e.DecoratedMessage);
            }
            catch (ScriptRuntimeException e)
            {
                Log("Lua Runtime Error: " + e.DecoratedMessage);
            }
            catch (Exception e)
            {
                Log(e.Message);
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
            currentCallbacks.Add(null);
        }

        try
        {
            Script.DefaultOptions.DebugPrint = s => Debug.LogError(s);
            MoonSharp.Interpreter.Loaders.FileSystemScriptLoader scriptLoader = new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();
            scriptLoader.ModulePaths = new string[] { "scripts/?.lua" };
            scriptLoader.IgnoreLuaPathGlobal = true;

            script = new Script();
            script.Options.ScriptLoader = scriptLoader;
            script.Globals["SetChoices"] = (Action<Table>)SetChoices;
            script.Globals["Log"] = (Action<string>)Log;
            script.Globals["data"] = new Table(script);

            try
            {
                string code = File.ReadAllText("scripts/main.lua", System.Text.Encoding.UTF8);
                script.DoString(code, null, "scripts/main.lua");
            }
            catch (SyntaxErrorException e)
            {
                Log("Lua Syntax Error: " + e.DecoratedMessage);
            }
            catch (ScriptRuntimeException e)
            {
                Log("Lua Runtime Error: " + e.DecoratedMessage);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }
        catch (Exception e)
        {
            Log(e.Message);
        }

        //LuaReadAllFiles("scripts");
    }

    public void SetChoices(Table data)
    {
        textField.text = data.Get("text").String;

        Table choices = data.Get("choices").Table;
        int i = 1;
        for (; i <= 4; i++)
        {
            Table choice = choices.Get(i).Table;
            if (choice == null) break;
            choiceTexts[i - 1].text = choice.Get(1).String;
            choiceButtons[i - 1].gameObject.SetActive(true);
            currentCallbacks[i - 1] = choice.Get(2);
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
