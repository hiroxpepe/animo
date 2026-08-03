// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License.
#nullable enable
using NUnit.Framework;
using System.Linq;

namespace Animo.Tests.EditMode.Convention;

/// <summary>
/// Verifies the rules themselves against mock code. Without this, a green
/// convention run only proves the scan ran, not that it can catch anything.
/// Every rule gets a dirty case (must be caught) and a clean case (must pass).
/// </summary>
[TestFixture]
[Category("Convention")]
public class ConventionRulesTests
{
    static bool caught(string code, string needle) =>
        ConventionRules.find_naming_violations(code, "mock.cs").Any(v => v.Contains(needle));

    static int naming_count(string code) =>
        ConventionRules.find_naming_violations(code, "mock.cs").Count;

    static int order_count(string code) =>
        ConventionRules.find_order_violations(code, "mock.cs").Count;

    // ---- naming: dirty cases must be caught ------------------------------

    [Test]
    public void Catches_PrivateFieldNotSnakeCase()
    {
        Assert.That(caught("class M { int badField; }", "must be _snake_case"), Is.True);
    }

    [Test]
    public void Catches_ConstNotUpperSnake()
    {
        Assert.That(caught("class M { const int maxSize = 1; }", "must be UPPER_SNAKE"), Is.True);
    }

    [Test]
    public void Catches_LocalNotSnakeCase()
    {
        Assert.That(caught("class M { void run() { var itemCount = 1; } }", "local 'itemCount'"), Is.True);
    }

    [Test]
    public void Catches_ForEachVarNotSnakeCase()
    {
        Assert.That(caught("class M { void run() { foreach (var eachItem in x) {} } }", "foreach var 'eachItem'"), Is.True);
    }

    [Test]
    public void Catches_ParameterNotSnakeCase()
    {
        Assert.That(caught("class M { void run(int tabId) {} }", "parameter 'tabId'"), Is.True);
    }

    [Test]
    public void Catches_PublicMethodNotPascalCase()
    {
        Assert.That(caught("class M { public void doWork() {} }", "must be PascalCase"), Is.True);
    }

    [Test]
    public void Catches_PrivateMethodNotCamelCase()
    {
        Assert.That(caught("class M { private void DoWork() {} }", "must be camelCase"), Is.True);
    }

    [Test]
    public void Catches_EnumMemberNotPascalCase()
    {
        Assert.That(caught("enum M { first_value }", "must be PascalCase"), Is.True);
    }

    [Test]
    public void Catches_AbbreviationNotExpanded()
    {
        Assert.That(caught("class M { public void CalcNow() {} }", "expand to 'Calculate'"), Is.True);
    }

    [Test]
    public void Catches_AcronymNotUpperCased()
    {
        Assert.That(caught("class M { public void ReadApiState() {} }", "use 'API'"), Is.True);
    }

    // ---- naming: clean cases must pass -----------------------------------

    [Test]
    public void Passes_CleanNaming()
    {
        var code = @"
enum TabState { Idle, Stopped }

class Watcher
{
    const int MAX_TABS = 8;
    static readonly string DEFAULT_URL = ""x"";
    int _tab_count;

    public void Start(int tab_index)
    {
        var next_state = TabState.Idle;
        foreach (var each_tab in tabs) { }
    }

    void reset() { }
}";
        Assert.That(naming_count(code), Is.Zero,
            string.Join("\n  ", ConventionRules.find_naming_violations(code, "mock.cs")));
    }

    [Test]
    public void Skips_OverrideMemberParameters()
    {
        // An override signature comes from outside, so its parameter names are exempt.
        Assert.That(naming_count("class M { public override void OnCreate(int savedState) {} }"), Is.Zero);
    }

    [Test]
    public void Allows_AcronymAlreadyUpperCased()
    {
        Assert.That(naming_count("class M { public void ReadDOMTree() {} }"), Is.Zero);
    }

    [Test]
    public void Allows_WordThatMerelyContainsAcronymLetters()
    {
        // 'Region' contains 'io' but not as a hump, so it must not be flagged.
        Assert.That(naming_count("class M { public void FindRegion() {} }"), Is.Zero);
    }


    [Test]
    public void Ignores_ExternalApiNamesWhenSpelling()
    {
        // Calling an SDK member named LoadUrl is not ours to rename.
        Assert.That(naming_count("class M { void run() { view.LoadUrl(site); } }"), Is.Zero);
    }

    [Test]
    public void Ignores_ExternalPropertyNamesWhenSpelling()
    {
        Assert.That(naming_count("class M { void run() { settings.DomStorageEnabled = true; } }"), Is.Zero);
    }

    [Test]
    public void Ignores_ExternDeclarations()
    {
        // The name of an imported function is fixed by the platform. It cannot be
        // renamed, so holding it to our casing would only force it to be silenced.
        var code = "class M { static extern int DwmSetWindowAttribute(int window); }";
        Assert.That(naming_count(code), Is.Zero);
    }

    [Test]
    public void Catches_AbbreviationInDeclaredTypeName()
    {
        Assert.That(caught("class CfgBox { }", "expand to 'Config'"), Is.True);
    }

    // ---- order -----------------------------------------------------------

    [Test]
    public void Catches_MethodBeforeField()
    {
        var code = "class M { public void Run() {} int _count; }";
        Assert.That(order_count(code), Is.GreaterThan(0));
    }

    [Test]
    public void Catches_PublicMethodAfterPrivateMethod()
    {
        var code = "class M { void helper() {} public void Run() {} }";
        Assert.That(order_count(code), Is.GreaterThan(0));
    }

    [Test]
    public void Catches_InstanceFieldBeforeConst()
    {
        var code = "class M { int _count; const int MAX = 1; }";
        Assert.That(order_count(code), Is.GreaterThan(0));
    }

    [Test]
    public void Passes_CleanOrder()
    {
        var code = @"
class Watcher
{
    const int MAX_TABS = 8;
    static int _shared_count;
    int _tab_count;

    public Watcher() { }

    public int TabCount { get; }

    public void Start() { }

    void reset() { }
}";
        Assert.That(order_count(code), Is.Zero,
            string.Join("\n  ", ConventionRules.find_order_violations(code, "mock.cs")));
    }

    [Test]
    public void Ignores_InterfaceDeclarations()
    {
        Assert.That(order_count("interface I { void Run(); int Count { get; } }"), Is.Zero);
    }

    // ---- type names ------------------------------------------------------

    [Test]
    public void Catches_TypeNameNotPascalCase()
    {
        Assert.That(caught("class json_data { }", "type 'json_data' must be PascalCase"), Is.True);
    }

    [Test]
    public void Passes_PascalCaseTypeName()
    {
        Assert.That(naming_count("class NeedTable { }"), Is.Zero);
    }

    // ---- file names ------------------------------------------------------

    [Test]
    public void Catches_FileNameWithShortForm()
    {
        Assert.That(
            ConventionRules.find_filename_violations("Cfg.cs").Any(v => v.Contains("expand to 'Config'")),
            Is.True);
    }

    [Test]
    public void Catches_FileNameWithLowerCaseAcronym()
    {
        Assert.That(
            ConventionRules.find_filename_violations("Json.cs").Any(v => v.Contains("use 'JSON'")),
            Is.True);
    }

    [Test]
    public void Passes_CleanFileName()
    {
        Assert.That(ConventionRules.find_filename_violations("JSON.cs"), Is.Empty);
        Assert.That(ConventionRules.find_filename_violations("Composer.cs"), Is.Empty);
    }

    // ---- exposed fields --------------------------------------------------

    [Test]
    public void Catches_ExposedFieldNotPascalCase()
    {
        Assert.That(caught("class M { public int tab_count; }", "field 'tab_count' must be PascalCase"), Is.True);
    }

    [Test]
    public void Passes_ExposedFieldPascalCase()
    {
        Assert.That(naming_count("class M { public int TabCount; }"), Is.Zero);
    }

    // ---- namespaces ------------------------------------------------------

    [Test]
    public void Catches_NamespaceSegmentNotPascalCase()
    {
        Assert.That(caught("namespace animo.core { class M { } }",
            "namespace segment 'animo' must be PascalCase"), Is.True);
    }

    [Test]
    public void Passes_PascalCaseNamespace()
    {
        Assert.That(naming_count("namespace Animo.Core { class M { } }"), Is.Zero);
    }

    // ---- unit marks are not touched --------------------------------------

    [Test]
    public void Passes_UnitMarkInName()
    {
        // Hz is a unit mark, not a letter word: it keeps its print form and is
        // not in the all-caps list, so a name that holds it is clean.
        Assert.That(naming_count("class M { public float ToHz() => 0f; }"), Is.Zero);
        Assert.That(ConventionRules.find_filename_violations("FrequencyHz.cs"), Is.Empty);
    }
}
