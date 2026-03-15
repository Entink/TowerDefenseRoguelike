using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;
using TMPro;

public class DebugConsoleUI : MonoBehaviour
{
    private static DebugConsoleUI instance;

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private DebugConsole debugConsole;

    [Header("Settings")]
    [SerializeField] private int maxLogLines = 20;
    [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;

    private readonly List<string> logLines = new();
    private bool isOpen;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
        if(instance == this)
        {
            instance = null;
        }

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

        string result = debugConsole != null ? debugConsole.ExecuteCommand(input) : "DebugConsole reference is missing.";

        if(result == "__CLEAR__")
        {
            ClearLog();
        }
        else if(!string.IsNullOrWhiteSpace(result))
        {
            AppendLine(result);
        }

        commandInput.text = string.Empty;
        commandInput.ActivateInputField();
        commandInput.Select();
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