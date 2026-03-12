using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class DebugConsoleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private TMP_InputField commandInput;

    [Header("Settings")]
    [SerializeField] private int maxLogLines = 20;
    [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;

    private readonly List<string> logLines = new();
    private bool isOpen;

    void Start()
    {
        CloseConsoleImmediate();
        AppendLine("Debug console initialized.");
        AppendLine("Press ~ to open.");

        if(commandInput != null)
        {
            commandInput.onSubmit.AddListener(HandleInputSubmit);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleConsole();

        if (!isOpen)
            return;


        if (Input.GetKeyDown(KeyCode.Escape))
            CloseConsole();
    }

    private void HandleInputSubmit(string _)
    {
        if (!isOpen)
            return;

        SubmitCommand();
    }

    private void OnDestroy()
    {
        if (commandInput != null)
            commandInput.onSubmit.RemoveListener(HandleInputSubmit);
    }

    private void CloseConsoleImmediate()
    {
        isOpen = false;

        if (panel != null)
            panel.SetActive(false);
    }

    private void OpenConsole()
    {
        isOpen = true;

        if (panel != null)
            panel.SetActive(true);

        if(commandInput != null)
        {
            commandInput.text = string.Empty;
            commandInput.ActivateInputField();
            commandInput.Select();
            EventSystem.current?.SetSelectedGameObject(commandInput.gameObject);
        }
    }

    public void CloseConsole()
    {
        isOpen = false;

        if (panel != null)
            panel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void ToggleConsole()
    {
        if (isOpen)
            CloseConsole();
        else
            OpenConsole();
    }

    private void SubmitCommand()
    {
        if (commandInput == null)
            return;

        string input = commandInput.text;


        if(string.IsNullOrWhiteSpace(input))
        {
            commandInput.text = string.Empty;
            commandInput.ActivateInputField();
            commandInput.Select();
            return;
        }

        AppendLine($"> {input}");

        string result = ExecuteLocalCommand(input);

        if (!string.IsNullOrWhiteSpace(result))
            AppendLine(result);

        commandInput.text = string.Empty;
        commandInput.ActivateInputField();
        commandInput.Select();
    }

    private string ExecuteLocalCommand(string input)
    {
        string trimmedInput = input.Trim();
        string[] parts = trimmedInput.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return string.Empty;

        string command = parts[0].ToLower();

        switch (command)
        {
            case "help":
                return "Available commands: help, clear, echo, timescale";

            case "clear":
                ClearLog();
                return string.Empty;

            case "echo":
                if (parts.Length == 1)
                    return string.Empty;

                return trimmedInput.Substring(parts[0].Length).TrimStart();

            case "timescale":
                if (parts.Length < 2)
                    return "Usage: timescale <value>";

                if (!float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                    return "Invalid value. Use dot, e.g. timescale 0.5";

                if (value < 0f)
                    return "TimeScale cannot be negative.";

                Time.timeScale = value;
                return $"TimeScale set to {Time.timeScale.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            default:
                return $"Unknown command: {command}. Use 'help' command";
        }
    }

    private void AppendLine(string line)
    {
        if (string.IsNullOrEmpty(line))
            return;

        logLines.Add(line);

        while (logLines.Count > maxLogLines)
            logLines.RemoveAt(0);

        RefreshLog();
    }

    private void RefreshLog()
    {
        if (logText == null)
            return;

        StringBuilder builder = new();

        for(int i = 0; i < logLines.Count; i++)
        {
            builder.AppendLine(logLines[i]);
        }

        logText.text = builder.ToString();
    }

    private void ClearLog()
    {
        logLines.Clear();
        RefreshLog();
    }
}
