using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;


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

        try
        {
            return command.Invoke(args);

        }
        catch(Exception ex)
        {
            return $"Command '{commandName}' failed: {ex.GetType().Name} - {ex.Message}";
        }
    }

    private void RegisterCommands()
    {
        commands.Clear();

        commands["help"] = HelpCommand;
        commands["clear"] = ClearCommand;
        commands["echo"] = EchoCommand;
        commands["timescale"] = TimeScaleCommand;

        commands["save"] = SaveCommand;
        commands["deletesave"] = DeleteSaveCommand;
        commands["showrun"] = ShowRunCommand;

        commands["gold"] = GoldCommand;
        commands["killallenemies"] = KillAllEnemiesCommand;
        commands["damageplayerbase"] = DamagePlayerBaseCommand;
        commands["damageenemybase"] = DamageEnemyBaseCommand;
        commands["showcombat"] = ShowCombatCommand;
    }

    private string HelpCommand(string[] args)
    {
        return "Available commands: help, clear, echo, timescale, save, deletesave, showrun, gold, killallenemies, damageplayerbase, damageenemybase, showcombat";
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

    private string SaveCommand(string[] args)
    {
        if (MapRunData.currentMap == null)
            return "Cannot save: no active run loaded yet.";
        RunSaveManager.Save();
        return "Run saved.";
    }

    private string DeleteSaveCommand(string[] args)
    {
        if (!RunSaveManager.Exists())
            return "No save file";

        RunSaveManager.Delete();
        return "Save deleted";
    }

    private string ShowRunCommand(string[] args)
    {
        string currentNodeText = MapRunData.currentNode != null ? MapRunData.currentNode.id.ToString() : "null";
        string pendingNodeText = MapRunData.pendingNodeId.ToString();

        int fightsWon = RunStatsCollector.S.fightsWon;
        int fightsLost = RunStatsCollector.S.fightsLost;
        float timeInFights = RunStatsCollector.S.timeInFights;

        return
            $"Seed = {MapRunData.currentSeed} | " +
            $"Act = {MapRunData.currentAct} | " +
            $"Current node = {currentNodeText} | " +
            $"Pending node = {pendingNodeText} | " +
            $"Cash = {RunResources.GetCash()} | " +
            $"Won fights = {fightsWon} | " +
            $"Lost fights = {fightsLost} | " +
            $"Time in fights = {timeInFights.ToString("0.00", CultureInfo.InvariantCulture)} | " +
            $"Save exists = {RunSaveManager.Exists()}";
    }

    private string GoldCommand(string[] args)
    {
        if (args.Length < 1)
            return "Usage: gold <value>";

        if (!int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            return "Invalid value";

        CurrencyManager currencyManager = FindFirstObjectByType<CurrencyManager>();
        if (currencyManager == null)
            return "CurrencyManager not found in current scene";

        bool success = TrySetGold(currencyManager, value);

        if (!success)
            return "Could not change gold. No supported gold field/property/method found in CurrencyManager";

        return $"Gold set to {value}";
    }

    private string KillAllEnemiesCommand(string[] args)
    {
        UnitController[] units = FindObjectsByType<UnitController>(FindObjectsSortMode.None);

        int killed = 0;
        int skipped = 0;

        foreach(UnitController unit in units)
        {
            if (unit == null)
                continue;

            bool? isEnemy = TryDetectEnemyUnit(unit);

            if(isEnemy != true)
            {
                skipped++;
                continue;
            }

            if (TryKillUnit(unit))
                killed++;
            else
                skipped++;
        }

        return $"killallenemies: killed = {killed} | skipped = {skipped}";
    }

    private string DamagePlayerBaseCommand(string[] args)
    {
        return DamageBase(args, true);
    }

    private string DamageEnemyBaseCommand(string[] args)
    {
        return DamageBase(args, false);
    }

    private string ShowCombatCommand(string[] args)
    {
        BaseController[] bases = FindObjectsByType<BaseController>(FindObjectsSortMode.None);
        UnitController[] units = FindObjectsByType<UnitController>(FindObjectsSortMode.None);

        BaseController playerBase = null;
        BaseController enemyBase = null;

        foreach (BaseController baseController in bases)
        {
            if (baseController == null)
                continue;

            if (baseController.isPlayerBase)
                playerBase = baseController;
            else
                enemyBase = baseController;
        }

        int playerUnits = 0;
        int enemyUnits = 0;
        int unknownUnits = 0;

        foreach (UnitController unit in units)
        {
            bool? isEnemy = TryDetectEnemyUnit(unit);

            if (isEnemy == true)
                enemyUnits++;
            else if (isEnemy == false)
                playerUnits++;
            else
                unknownUnits++;
        }

        string playerBaseHp = playerBase != null
            ? $"{playerBase.currentHP.ToString("0.##", CultureInfo.InvariantCulture)}/{playerBase.maxHP.ToString("0.##", CultureInfo.InvariantCulture)}"
            : "missing";

        string enemyBaseHp = enemyBase != null
            ? $"{enemyBase.currentHP.ToString("0.##", CultureInfo.InvariantCulture)}/{enemyBase.maxHP.ToString("0.##", CultureInfo.InvariantCulture)}"
            : "missing";

        string goldText = TryGetCurrentGold(out int gold) ? gold.ToString() : "unknown";

        return
            $"Player Base HP = {playerBaseHp} | " +
            $"Enemy Base HP = {enemyBaseHp} | " +
            $"Player Units = {playerUnits} | " +
            $"Enemy Units = {enemyUnits} | " +
            $"Unknown Units = {unknownUnits} | " +
            $"Gold = {goldText} | " +
            $"Time Scale = {Time.timeScale.ToString("0.##", CultureInfo.InvariantCulture)}";
    }

    private string DamageBase(string[] args, bool damagePlayerBase)
    {
        if (args.Length < 1)
            return damagePlayerBase ? "Usage: damageplayerbase <value>" : "Usage: damageenemybase <value>";

        if (!float.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            return "Invalid value. Use dot for decimal values";

        if (value < 0f)
            return "Damage cannot be negative";

        BaseController[] bases = FindObjectsByType<BaseController>(FindObjectsSortMode.None);

        foreach (BaseController baseController in bases)
        {
            if (baseController == null)
                continue;

            if (baseController.isPlayerBase != damagePlayerBase)
                continue;

            baseController.TakeDamage(value);

            return damagePlayerBase
                ? $"Player base damaged for {value.ToString("0.##", CultureInfo.InvariantCulture)}"
                : $"Enemy base damaged for {value.ToString("0.##", CultureInfo.InvariantCulture)}";
        }

        return damagePlayerBase ? "Player base not found" : "Enemy base not found";
    }

    private bool TryGetCurrentGold(out int value)
    {
        value = 0;

        CurrencyManager currencyManager = FindFirstObjectByType<CurrencyManager>();
        if (currencyManager == null)
            return false;

        object target = currencyManager;
        Type type = target.GetType();

        string[] memberNames =
        {
            "currentGold",
            "gold",
            "CurrentGold",
            "Gold",
            "currentAmount",
            "CurrentAmount"
        };

        foreach(string name in memberNames)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if(field != null)
            {
                object raw = field.GetValue(target);
                if (TryConvertToInt(raw, out value))
                    return true;   
            }

            PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanRead)
            {
                object raw = property.GetValue(target);
                if (TryConvertToInt(raw, out value))
                    return true;
            }
        }

        return false;
    }

    private bool TrySetGold(CurrencyManager currencyManager, int value)
    {
        object target = currencyManager;
        Type type = target.GetType();

        if (TryInvokeNumericMethod(target, type, "SetGold", value))
            return true;

        if (TryInvokeNumericMethod(target, type, "SetCurrentGold", value))
            return true;

        if (TryInvokeNumericMethod(target, type, "SetAmount", value))
            return true;

        if (TrySetNumericMember(target, type, "currentGold", value))
            return true;

        if (TrySetNumericMember(target, type, "gold", value))
            return true;

        if (TrySetNumericMember(target, type, "CurrentGold", value))
            return true;

        if (TrySetNumericMember(target, type, "Gold", value))
            return true;

        if (TrySetNumericMember(target, type, "currentAmount", value))
            return true;

        if (TrySetNumericMember(target, type, "CurrentAmount", value))
            return true;

        if (TryGetCurrentGold(out int currentGold))
        {
            int delta = value - currentGold;

            if (TryInvokeNumericMethod(target, type, "AddGold", delta))
                return true;

            if (TryInvokeNumericMethod(target, type, "GainGold", delta))
                return true;
        }

        return false;
    }

    private bool? TryDetectEnemyUnit(UnitController unit)
    {
        if (unit == null)
            return null;

        if (unit.CompareTag("Enemy"))
            return true;

        if (unit.CompareTag("Player"))
            return false;

        object target = unit;
        Type type = target.GetType();

        if (TryReadBoolMember(target, type, "isEnemy", out bool isEnemy))
            return isEnemy;

        if (TryReadBoolMember(target, type, "IsEnemy", out isEnemy))
            return isEnemy;

        if (TryReadBoolMember(target, type, "enemyUnit", out isEnemy))
            return isEnemy;

        if (TryReadBoolMember(target, type, "isPlayerUnit", out bool isPlayerUnit))
            return !isPlayerUnit;

        if (TryReadBoolMember(target, type, "IsPlayerUnit", out isPlayerUnit))
            return !isPlayerUnit;

        if (TryReadBoolMember(target, type, "isPlayer", out bool isPlayer))
            return !isPlayer;

        return null;
    }

    private bool TryKillUnit(UnitController unit)
    {
        if (unit == null)
            return false;

        object target = unit;
        Type type = target.GetType();

        MethodInfo dieMethod = type.GetMethod("Die", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (dieMethod != null && dieMethod.GetParameters().Length == 0)
        {
            dieMethod.Invoke(target, null);
            return true;
        }

        MethodInfo takeDamageMethod = type.GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (takeDamageMethod != null)
        {
            ParameterInfo[] parameters = takeDamageMethod.GetParameters();
            object[] invokeArgs = BuildHeavyDamageArgs(parameters);

            if (invokeArgs != null)
            {
                takeDamageMethod.Invoke(target, invokeArgs);
                return true;
            }
        }

        return false;
    }

    private object[] BuildHeavyDamageArgs(ParameterInfo[] parameters)
    {
        if (parameters == null)
            return null;

        object[] args = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;

            if (parameterType == typeof(int))
                args[i] = 999999;
            else if (parameterType == typeof(float))
                args[i] = 999999f;
            else if (parameterType == typeof(double))
                args[i] = 999999d;
            else if (parameterType == typeof(bool))
                args[i] = false;
            else if (parameterType == typeof(Vector2))
                args[i] = Vector2.zero;
            else if (parameterType == typeof(Vector3))
                args[i] = Vector3.zero;
            else if (parameterType.IsEnum)
                args[i] = Activator.CreateInstance(parameterType);
            else if (!parameterType.IsValueType)
                args[i] = null;
            else
                return null;
        }

        return args;
    }

    private bool TryInvokeNumericMethod(object target, Type type, string methodName, int value)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
            return false;

        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != 1)
            return false;

        Type paramType = parameters[0].ParameterType;

        if (paramType == typeof(int))
        {
            method.Invoke(target, new object[] { value });
            return true;
        }

        if (paramType == typeof(float))
        {
            method.Invoke(target, new object[] { (float)value });
            return true;
        }

        return false;
    }

    private bool TrySetNumericMember(object target, Type type, string name, int value)
    {
        FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            if (field.FieldType == typeof(int))
            {
                field.SetValue(target, value);
                return true;
            }

            if (field.FieldType == typeof(float))
            {
                field.SetValue(target, (float)value);
                return true;
            }
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null && property.CanWrite)
        {
            if (property.PropertyType == typeof(int))
            {
                property.SetValue(target, value);
                return true;
            }

            if (property.PropertyType == typeof(float))
            {
                property.SetValue(target, (float)value);
                return true;
            }
        }

        return false;
    }

    private bool TryReadBoolMember(object target, Type type, string name, out bool value)
    {
        value = false;

        FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && field.FieldType == typeof(bool))
        {
            value = (bool)field.GetValue(target);
            return true;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null && property.CanRead && property.PropertyType == typeof(bool))
        {
            value = (bool)property.GetValue(target);
            return true;
        }

        return false;
    }

    private bool TryConvertToInt(object raw, out int value)
    {
        value = 0;

        if (raw == null)
            return false;

        switch (raw)
        {
            case int intValue:
                value = intValue;
                return true;
            case float floatValue:
                value = Mathf.RoundToInt(floatValue);
                return true;
            case double doubleValue:
                value = Mathf.RoundToInt((float)doubleValue);
                return true;
            default:
                return false;
        }
    }
}