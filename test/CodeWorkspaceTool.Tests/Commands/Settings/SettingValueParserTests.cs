using System.Text.Json;

using CodeWorkspaceTool;
using CodeWorkspaceTool.Commands.Settings;
namespace Tests.Commands.Settings;

[TestFixture]
[TestOf(typeof(SettingValueParser))]
public class SettingValueParserTests
{
    [TestCase("true", JsonValueKind.True)]
    [TestCase("false", JsonValueKind.False)]
    [TestCase("42", JsonValueKind.Number)]
    [TestCase("3.14", JsonValueKind.Number)]
    [TestCase("hello", JsonValueKind.String)]
    public void Auto_infers_the_expected_kind(string raw, JsonValueKind expectedKind)
    {
        var value = SettingValueParser.Parse(raw, SettingValueType.Auto);

        Assert.That(value.GetValueKind(), Is.EqualTo(expectedKind));
    }

    [Test]
    public void Auto_parses_a_json_object()
    {
        var value = SettingValueParser.Parse("""{"a":1}""", SettingValueType.Auto);

        Assert.That(value.ToJsonString(), Is.EqualTo("""{"a":1}"""));
    }

    [Test]
    public void Auto_parses_a_json_array()
    {
        var value = SettingValueParser.Parse("[80,120]", SettingValueType.Auto);

        Assert.That(value.ToJsonString(), Is.EqualTo("[80,120]"));
    }

    [Test]
    public void Auto_falls_back_to_string_when_bracketed_text_is_not_valid_json()
    {
        var value = SettingValueParser.Parse("[not json", SettingValueType.Auto);

        Assert.Multiple(() =>
        {
            Assert.That(value.GetValueKind(), Is.EqualTo(JsonValueKind.String));
            Assert.That(value.GetValue<string>(), Is.EqualTo("[not json"));
        });
    }

    [Test]
    public void String_type_keeps_the_raw_text_even_when_it_looks_like_a_bool()
    {
        var value = SettingValueParser.Parse("true", SettingValueType.String);

        Assert.Multiple(() =>
        {
            Assert.That(value.GetValueKind(), Is.EqualTo(JsonValueKind.String));
            Assert.That(value.GetValue<string>(), Is.EqualTo("true"));
        });
    }

    [Test]
    public void Bool_type_rejects_a_non_bool_value()
    {
        Assert.That(
            () => SettingValueParser.Parse("notabool", SettingValueType.Bool),
            Throws.TypeOf<CodeWorkspaceException>());
    }

    [Test]
    public void Int_type_rejects_a_fractional_value()
    {
        Assert.That(
            () => SettingValueParser.Parse("3.14", SettingValueType.Int),
            Throws.TypeOf<CodeWorkspaceException>());
    }

    [Test]
    public void Number_type_accepts_a_fractional_value()
    {
        var value = SettingValueParser.Parse("3.14", SettingValueType.Number);

        Assert.That(value.GetValue<double>(), Is.EqualTo(3.14));
    }

    [Test]
    public void Json_type_parses_an_arbitrary_document()
    {
        var value = SettingValueParser.Parse("""{"rulers":[80,120]}""", SettingValueType.Json);

        Assert.That(value.ToJsonString(), Is.EqualTo("""{"rulers":[80,120]}"""));
    }

    [Test]
    public void Json_type_rejects_invalid_json()
    {
        Assert.That(
            () => SettingValueParser.Parse("not json", SettingValueType.Json),
            Throws.TypeOf<CodeWorkspaceException>());
    }
}