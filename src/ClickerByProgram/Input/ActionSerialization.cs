using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ClickerByProgram.Input;

internal static class ActionSerialization
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize(IEnumerable<IInputAction> actions)
    {
        var payload = actions.Select(CreateSerializableAction).ToArray();
        return JsonSerializer.Serialize(payload, Options);
    }

    public static IReadOnlyList<IInputAction> Deserialize(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Array.Empty<IInputAction>();
        }

        SerializableAction[]? definitions;
        try
        {
            definitions = JsonSerializer.Deserialize<SerializableAction[]>(content, Options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("The action file contains invalid JSON.", ex);
        }

        if (definitions == null || definitions.Length == 0)
        {
            return Array.Empty<IInputAction>();
        }

        var actions = new List<IInputAction>(definitions.Length);

        foreach (var definition in definitions)
        {
            if (definition == null)
            {
                continue;
            }

            actions.Add(CreateAction(definition));
        }

        return actions;
    }

    private static SerializableAction CreateSerializableAction(IInputAction action)
    {
        return action switch
        {
            KeyboardChordAction keyboard => new SerializableAction
            {
                Type = "KeyboardChord",
                DelayMilliseconds = keyboard.DelayBeforeExecution.TotalMilliseconds,
                HoldMilliseconds = keyboard.HoldDuration.TotalMilliseconds,
                Keys = keyboard.Keys.Select(k => k.ToString()).ToArray()
            },
            MouseButtonAction mouseButton => new SerializableAction
            {
                Type = "MouseButton",
                DelayMilliseconds = mouseButton.DelayBeforeExecution.TotalMilliseconds,
                HoldMilliseconds = mouseButton.HoldDuration.TotalMilliseconds,
                Button = mouseButton.Button.ToString(),
                X = mouseButton.ScreenPosition.X,
                Y = mouseButton.ScreenPosition.Y
            },
            MouseMoveAction mouseMove => new SerializableAction
            {
                Type = "MouseMove",
                DelayMilliseconds = mouseMove.DelayBeforeExecution.TotalMilliseconds,
                X = mouseMove.ScreenPosition.X,
                Y = mouseMove.ScreenPosition.Y
            },
            _ => throw new InvalidOperationException($"Unsupported action type: {action.GetType().Name}")
        };
    }

    private static IInputAction CreateAction(SerializableAction definition)
    {
        if (string.IsNullOrWhiteSpace(definition.Type))
        {
            throw new InvalidOperationException("Each action must specify a type.");
        }

        var delay = TimeSpan.FromMilliseconds(Math.Max(0, definition.DelayMilliseconds ?? 0));

        if (definition.Type.Equals("KeyboardChord", StringComparison.OrdinalIgnoreCase))
        {
            return CreateKeyboardChordAction(definition, delay);
        }

        if (definition.Type.Equals("MouseButton", StringComparison.OrdinalIgnoreCase))
        {
            return CreateMouseButtonAction(definition, delay);
        }

        if (definition.Type.Equals("MouseMove", StringComparison.OrdinalIgnoreCase))
        {
            return CreateMouseMoveAction(definition, delay);
        }

        throw new InvalidOperationException($"Unsupported action type '{definition.Type}'.");
    }

    private static IInputAction CreateKeyboardChordAction(SerializableAction definition, TimeSpan delay)
    {
        if (definition.Keys == null || definition.Keys.Length == 0)
        {
            throw new InvalidOperationException("Keyboard actions must include at least one key.");
        }

        var keys = new List<Keys>(definition.Keys.Length);
        foreach (var keyName in definition.Keys)
        {
            if (Enum.TryParse(keyName, out Keys key))
            {
                keys.Add(key);
            }
            else
            {
                throw new InvalidOperationException($"Unknown key value '{keyName}'.");
            }
        }

        var hold = TimeSpan.FromMilliseconds(Math.Max(0, definition.HoldMilliseconds ?? 0));
        return new KeyboardChordAction(keys.ToArray(), hold, delay);
    }

    private static IInputAction CreateMouseButtonAction(SerializableAction definition, TimeSpan delay)
    {
        if (string.IsNullOrWhiteSpace(definition.Button))
        {
            throw new InvalidOperationException("Mouse button actions must specify a button.");
        }

        if (!Enum.TryParse(definition.Button, out MouseButton button))
        {
            throw new InvalidOperationException($"Unknown mouse button '{definition.Button}'.");
        }

        if (definition.X is null || definition.Y is null)
        {
            throw new InvalidOperationException("Mouse button actions must include an X and Y coordinate.");
        }

        var position = new Point(definition.X.Value, definition.Y.Value);
        var hold = TimeSpan.FromMilliseconds(Math.Max(0, definition.HoldMilliseconds ?? 0));
        return new MouseButtonAction(button, position, hold, delay);
    }

    private static IInputAction CreateMouseMoveAction(SerializableAction definition, TimeSpan delay)
    {
        if (definition.X is null || definition.Y is null)
        {
            throw new InvalidOperationException("Mouse move actions must include an X and Y coordinate.");
        }

        var position = new Point(definition.X.Value, definition.Y.Value);
        return new MouseMoveAction(position, delay);
    }

    private sealed class SerializableAction
    {
        public string? Type { get; set; }

        public double? DelayMilliseconds { get; set; }

        public double? HoldMilliseconds { get; set; }

        public int? X { get; set; }

        public int? Y { get; set; }

        public string[]? Keys { get; set; }

        public string? Button { get; set; }
    }
}
