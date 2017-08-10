using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text;

public class Main : MonoBehaviour {
    public Button buttonPrefab;

    Script script;

    Text textField;
    Text debugField;

    Transform choicesNPanel;

    int debugLinesTop = -1;
    string[] debugLines = new string[20];

    public void CallCallback(DynValue v)
    {
        try
        {
            script.Call(v);
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

    // Use this for initialization
    void Start () {
        textField = transform.Find("Text").GetComponent<Text>();
        debugField = transform.Find("ConsolePanel/Console").GetComponent<Text>();
        choicesNPanel = transform.Find("NChoices");

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
                foreach (var stl in e.CallStack)
                {
                    Log("  -> " + script.GetSourceCode(stl.Location.SourceIdx).Name + ": " + stl.Location.FromLine + ": " + stl.Location.FromChar);
                }
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
    }

    public void SetChoices(Table data)
    {
        
        textField.text = data.Get("text").String;

        RectTransform content = choicesNPanel.Find("Scroll View/Viewport/Content") as RectTransform;
        Scrollbar scrollbar = choicesNPanel.Find("Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();
        scrollbar.value = 1.0f;
        Table choices = data.Get("choices").Table;
        Transform panel = choicesNPanel;
        float yStep = 90;
        int count = choices.Length;
        int columns = 1;
        if (count > 4) columns = 2;
        int rows = count / columns + ((count % columns == 0) ? 0 : 1);
        float height = 90.0f * rows + 10.0f;
        float currentY = -50.0f;
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        float buttonSizeX = (content.sizeDelta.x - 10.0f - 10.0f * columns) / (float)columns;
        float buttonStartX = 10.0f + buttonSizeX / 2.0f;

        for (int i = 0; i < content.childCount; i++)
        {
            GameObject.Destroy(content.GetChild(i).gameObject);
        }

        int c = 0;
        for (int row = 0; row < rows; row++)
        {
            float currentX = buttonStartX;
            for (int column = 0; column < columns; column++)
            {
                Button button = GameObject.Instantiate<Button>(buttonPrefab);
                button.transform.SetParent(content.transform);
                RectTransform buttonTransf = button.GetComponent<RectTransform>();
                buttonTransf.localPosition = new Vector3(currentX, currentY, 0.0f);
                buttonTransf.sizeDelta = new Vector2(buttonSizeX, 80.0f);
                currentX += buttonSizeX + 10.0f;

                Table choice = choices.Get(c + 1).Table;
                Text text = button.transform.Find("Text").GetComponent<Text>();
                text.text = choice.Get(1).String;
                button.onClick.AddListener(() => CallCallback(choice.Get(2)));

                c++;
                if (c >= count) return;
            }
            currentY -= yStep;
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
