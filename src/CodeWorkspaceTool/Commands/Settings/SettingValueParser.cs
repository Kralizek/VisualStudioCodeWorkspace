using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CodeWorkspaceTool.Commands.Settings;

public static class SettingValueParser
{
    public static JsonNode Parse(string raw, SettingValueType type) => type switch
    {
        SettingValueType.String => JsonValue.Create(raw),
        SettingValueType.Bool => ParseBool(raw),
        SettingValueType.Int => ParseInt(raw),
        SettingValueType.Number => ParseNumber(raw),
        SettingValueType.Json => ParseJson(raw),
        _ => ParseAuto(raw),
    };

    private static JsonNode ParseAuto(string raw)
    {
        if (bool.TryParse(raw, out var boolValue))
        {
            return JsonValue.Create(boolValue);
        }

        if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
        {
            return JsonValue.Create(intValue);
        }

        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var numberValue))
        {
            return JsonValue.Create(numberValue);
        }

        if (raw.Length > 0 && (raw[0] == '{' || raw[0] == '['))
        {
            try
            {
                return JsonNode.Parse(raw) ?? JsonValue.Create(raw);
            }
            catch (JsonException)
            {
                // Not valid JSON after all; fall through and treat it as a plain string.
            }
        }

        return JsonValue.Create(raw);
    }

    private static JsonNode ParseBool(string raw) =>
        bool.TryParse(raw, out var value)
            ? JsonValue.Create(value)
            : throw new CodeWorkspaceException($"'{raw}' is not a valid bool value.");

    private static JsonNode ParseInt(string raw) =>
        long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? JsonValue.Create(value)
            : throw new CodeWorkspaceException($"'{raw}' is not a valid int value.");

    private static JsonNode ParseNumber(string raw) =>
        double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? JsonValue.Create(value)
            : throw new CodeWorkspaceException($"'{raw}' is not a valid number value.");

    private static JsonNode ParseJson(string raw)
    {
        try
        {
            return JsonNode.Parse(raw) ?? throw new CodeWorkspaceException($"'{raw}' parses to a JSON null, which is not a supported setting value.");
        }
        catch (JsonException ex)
        {
            throw new CodeWorkspaceException($"'{raw}' is not valid JSON: {ex.Message}");
        }
    }
}
