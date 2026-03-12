using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;


public class DebugConsole : MonoBehaviour
{
    private readonly Dictionary<string, Func<string[], string>> commands = new();

    private void Awake()
    {
        RegisterCommands();
    }

    public string ExecuteCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string trimmedInput = input.Trim();
        string[] parts = trimmedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return string.Empty;

        string commandName = parts[0].ToLower();
        string[] args = new string[parts.Length - 1];

        for(int i = 1; i < parts.Length; i++)
        {
            args[i - 1] = parts[i];
        }

        if (!commands.TryGetValue(commandName, out Func<string[], string> command))
            return $"Unknown command: {commandName}. Use 'help' command";

        return command.Invoke(args);
    }

    private void RegisterCommands()
    {
        commands.Clear();

        commands["help"] = HelpCommand;
        commands["clear"] = ClearCommand;
        commands["echo"] = EchoCommand;
        commands["timescale"] = TimeScaleCommand;
    }

    private string HelpCommand(string[] args)
    {
        return "Available commands: help, clear, echo, timescale";
    }

    private string ClearCommand(string[] args)
    {
        return "__CLEAR__";
    }

    private string EchoCommand(string[] args)
    {
        if (args.Length == 0)
            return string.Empty;

        return string.Join(" ", args);
    }

    private string TimeScaleCommand(string[] args)
    {
        if (args.Length < 1)
            return "Usage: timescale <value>";

        if (!float.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            return "Invalid value. Use dot, e.g. timescale 0.5";

        if (value < 0f)
            return "TimeScale cannot be negative.";

        Time.timeScale = value;
        return $"TimeScale set to {Time.timeScale.ToString(CultureInfo.InvariantCulture)}";
    }
}
