// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License.
#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Animo.Tests.EditMode.Convention;

/// <summary>
/// The rules themselves. Pure: takes a source string, returns violation strings.
/// No filesystem, no paths, no compilation. That is what makes them testable with
/// mock code snippets.
///
/// Naming rules
///   1. private / mutable-static fields  -> _snake_case
///   2. const / static-readonly fields   -> UPPER_SNAKE
///   3. locals, foreach vars, parameters -> snake_case
///   4. exposed methods / properties     -> PascalCase
///   5. private methods / properties     -> camelCase
///   6. enum members                     -> PascalCase
///   7. spelling: expand abbreviations, upper-case true acronyms
///
/// Order rule
///   StyleCop element kind, then accessibility, then static-before-instance.
///
/// PORTING: only EXPAND / UPPER are repo specific.
/// </summary>
static class ConventionRules
{
    static readonly Regex SNAKE = new(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);
    static readonly Regex SNAKE_FIELD = new(@"^_[a-z][a-z0-9_]*$", RegexOptions.Compiled);
    static readonly Regex UPPER_SNAKE = new(@"^[A-Z][A-Z0-9_]*$", RegexOptions.Compiled);
    static readonly Regex PASCAL = new(@"^[A-Z][A-Za-z0-9]*$", RegexOptions.Compiled);
    static readonly Regex CAMEL = new(@"^[a-z][A-Za-z0-9]*$", RegexOptions.Compiled);

    // RULE 7a: abbreviation -> full word.
    internal static readonly Dictionary<string, string> EXPAND = new() {
        ["Msg"] = "Message",
        ["Btn"] = "Button",
        ["Cfg"] = "Config",
        ["Idx"] = "Index",
        ["Param"] = "Parameter",
        ["Init"] = "Initialize",
        ["Calc"] = "Calculate",
    };

    // RULE 7b: true acronym -> ALL-CAPS.
    internal static readonly Dictionary<string, string> UPPER = new() {
        ["Id"] = "ID", ["Io"] = "IO", ["Ui"] = "UI", ["Db"] = "DB",
        ["Api"] = "API", ["Url"] = "URL", ["Json"] = "JSON", ["Csv"] = "CSV",
        ["Http"] = "HTTP", ["Html"] = "HTML", ["Css"] = "CSS", ["Dom"] = "DOM",
        ["Cpu"] = "CPU", ["Gpu"] = "GPU", ["Gc"] = "GC", ["Cli"] = "CLI",
    };

    // ---- naming ----------------------------------------------------------

    internal static List<string> find_naming_violations(string code, string label)
    {
        var found = new List<string>();
        var root = CSharpSyntaxTree.ParseText(code).GetRoot();

        foreach (var field in root.DescendantNodes().OfType<FieldDeclarationSyntax>()) {
            bool is_const = has(field.Modifiers, "const");
            bool is_static_readonly = has(field.Modifiers, "static") && has(field.Modifiers, "readonly");
            foreach (var variable in field.Declaration.Variables) {
                var id = variable.Identifier.ValueText;
                if (is_const || is_static_readonly) {
                    if (!UPPER_SNAKE.IsMatch(id))
                        found.Add($"{label}:{line(variable)}: const '{id}' must be UPPER_SNAKE");
                } else if (exposed(field.Modifiers)) {
                    // An exposed mutable field on a [Serializable] type is a
                    // JSON-mapping field: snake_case is its external key.
                    // Anywhere else an exposed field is PascalCase.
                    if (in_serializable_type(variable)) {
                        if (!PASCAL.IsMatch(id) && !SNAKE.IsMatch(id))
                            found.Add($"{label}:{line(variable)}: json field '{id}' must be snake_case or PascalCase");
                    } else if (!PASCAL.IsMatch(id)) {
                        found.Add($"{label}:{line(variable)}: field '{id}' must be PascalCase");
                    }
                } else {
                    if (!SNAKE_FIELD.IsMatch(id))
                        found.Add($"{label}:{line(variable)}: field '{id}' must be _snake_case");
                }
            }
        }

        foreach (var declarator in root.DescendantNodes().OfType<VariableDeclaratorSyntax>()) {
            if (declarator.Parent?.Parent is FieldDeclarationSyntax) continue;
            // Event field declarations are members, not locals; they are checked
            // by the dedicated event loop below with the exposed/PascalCase rule.
            if (declarator.Parent?.Parent is EventFieldDeclarationSyntax) continue;
            var id = declarator.Identifier.ValueText;
            if (!SNAKE.IsMatch(id))
                found.Add($"{label}:{line(declarator)}: local '{id}' must be snake_case");
        }

        foreach (var ev in root.DescendantNodes().OfType<EventFieldDeclarationSyntax>()) {
            if (has(ev.Modifiers, "override")) continue;
            foreach (var variable in ev.Declaration.Variables)
                check_casing(found, label, variable, variable.Identifier.ValueText, ev.Modifiers, "event");
        }

        foreach (var ev in root.DescendantNodes().OfType<EventDeclarationSyntax>()) {
            if (has(ev.Modifiers, "override")) continue;
            if (ev.ExplicitInterfaceSpecifier != null) continue;
            check_casing(found, label, ev, ev.Identifier.ValueText, ev.Modifiers, "event");
        }

        foreach (var each in root.DescendantNodes().OfType<ForEachStatementSyntax>()) {
            var id = each.Identifier.ValueText;
            if (!SNAKE.IsMatch(id))
                found.Add($"{label}:{line(each)}: foreach var '{id}' must be snake_case");
        }

        foreach (var parameter in root.DescendantNodes().OfType<ParameterSyntax>()) {
            var id = parameter.Identifier.ValueText;
            if (id.Length == 0) continue;
            if (in_overriding_member(parameter)) continue;
            if (!SNAKE.IsMatch(id))
                found.Add($"{label}:{line(parameter)}: parameter '{id}' must be snake_case");
        }

        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>()) {
            if (has(method.Modifiers, "override")) continue;
            if (has(method.Modifiers, "extern")) continue;
            if (method.ExplicitInterfaceSpecifier != null) continue;
            if (method.Parent is InterfaceDeclarationSyntax) continue;
            check_casing(found, label, method, method.Identifier.ValueText, method.Modifiers, "method");
        }

        foreach (var property in root.DescendantNodes().OfType<PropertyDeclarationSyntax>()) {
            if (has(property.Modifiers, "override")) continue;
            if (property.ExplicitInterfaceSpecifier != null) continue;
            if (property.Parent is InterfaceDeclarationSyntax) continue;
            // A public property on a [Serializable] type is a JSON-mapping
            // property: its name is the external JSON key, so snake_case is
            // correct and PascalCase is not required. See tech_terms JSON entry.
            if (exposed(property.Modifiers) && in_serializable_type(property)) {
                var pid = property.Identifier.ValueText;
                if (!PASCAL.IsMatch(pid) && !SNAKE.IsMatch(pid))
                    found.Add($"{label}:{line(property)}: json property '{pid}' must be snake_case or PascalCase");
                continue;
            }
            check_casing(found, label, property, property.Identifier.ValueText, property.Modifiers, "property");
        }

        foreach (var member in root.DescendantNodes().OfType<EnumMemberDeclarationSyntax>()) {
            var id = member.Identifier.ValueText;
            if (!PASCAL.IsMatch(id))
                found.Add($"{label}:{line(member)}: enum member '{id}' must be PascalCase");
        }

        // Type names (class, struct, interface, enum, record) are always
        // PascalCase. The print rule holds here too: a letter word in a type
        // name is all caps (JSON, not Json), enforced by the spelling pass below.
        foreach (var type in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()) {
            var id = type.Identifier.ValueText;
            if (!PASCAL.IsMatch(id))
                found.Add($"{label}:{line(type)}: type '{id}' must be PascalCase");
        }

        // Namespace names are PascalCase in every dotted segment (Animo.Core,
        // not animo.core). The spelling pass below also holds for each segment.
        foreach (var ns in root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>()) {
            foreach (var seg in ns.Name.ToString().Split('.')) {
                if (!PASCAL.IsMatch(seg))
                    found.Add($"{label}:{line(ns)}: namespace segment '{seg}' must be PascalCase");
            }
        }

        // Spelling applies only to names WE declare. Names that come from outside
        // (platform and SDK members) are not ours to rename, so call sites and
        // member accesses are not scanned.
        foreach (var (id, at) in declared_names(root)) {
            foreach (var pair in EXPAND)
                if (is_hump(id, pair.Key))
                    found.Add($"{label}:{at}: '{id}' uses '{pair.Key}', expand to '{pair.Value}'");
            foreach (var pair in UPPER)
                if (is_hump(id, pair.Key))
                    found.Add($"{label}:{at}: '{id}' uses '{pair.Key}', use '{pair.Value}'");
        }

        found.Sort(StringComparer.Ordinal);
        return found;
    }

    // Identifiers introduced by this file: types, members, locals, parameters.
    // Overrides and explicit interface implementations are excluded because their
    // names are fixed by the external type they come from.
    static IEnumerable<(string id, int at)> declared_names(SyntaxNode root)
    {
        foreach (var type in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            yield return (type.Identifier.ValueText, line(type));

        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>()) {
            if (has(method.Modifiers, "override") || has(method.Modifiers, "extern")) continue;
            if (method.ExplicitInterfaceSpecifier != null) continue;
            yield return (method.Identifier.ValueText, line(method));
        }

        foreach (var property in root.DescendantNodes().OfType<PropertyDeclarationSyntax>()) {
            if (has(property.Modifiers, "override") || property.ExplicitInterfaceSpecifier != null) continue;
            yield return (property.Identifier.ValueText, line(property));
        }

        foreach (var declarator in root.DescendantNodes().OfType<VariableDeclaratorSyntax>())
            yield return (declarator.Identifier.ValueText, line(declarator));

        foreach (var ns in root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>())
            foreach (var seg in ns.Name.ToString().Split('.'))
                yield return (seg, line(ns));

        foreach (var each in root.DescendantNodes().OfType<ForEachStatementSyntax>())
            yield return (each.Identifier.ValueText, line(each));

        foreach (var parameter in root.DescendantNodes().OfType<ParameterSyntax>()) {
            if (parameter.Identifier.ValueText.Length == 0) continue;
            if (in_overriding_member(parameter)) continue;
            yield return (parameter.Identifier.ValueText, line(parameter));
        }

        foreach (var member in root.DescendantNodes().OfType<EnumMemberDeclarationSyntax>())
            yield return (member.Identifier.ValueText, line(member));
    }

    static void check_casing(List<string> found, string label, SyntaxNode node,
        string id, SyntaxTokenList modifiers, string kind)
    {
        bool want_pascal = exposed(modifiers);
        bool ok = want_pascal ? PASCAL.IsMatch(id) : CAMEL.IsMatch(id);
        if (!ok)
            found.Add($"{label}:{line(node)}: {kind} '{id}' must be {(want_pascal ? "PascalCase" : "camelCase")}");
    }

    // ---- file name ------------------------------------------------------

    // The file name (without .cs) must follow the same print rule as a type
    // name: no short forms, letter words in all caps. A file holding type JSON
    // is JSON.cs, not Json.cs.
    internal static List<string> find_filename_violations(string file_name)
    {
        var found = new List<string>();
        var stem = file_name.EndsWith(".cs") ? file_name.Substring(0, file_name.Length - 3) : file_name;
        foreach (var pair in EXPAND)
            if (is_hump(stem, pair.Key))
                found.Add($"{file_name}: file name uses '{pair.Key}', expand to '{pair.Value}'");
        foreach (var pair in UPPER)
            if (is_hump(stem, pair.Key))
                found.Add($"{file_name}: file name uses '{pair.Key}', use '{pair.Value}'");
        return found;
    }

    // ---- order -----------------------------------------------------------

    internal static List<string> find_order_violations(string code, string label)
    {
        var found = new List<string>();
        var tree = CSharpSyntaxTree.ParseText(code);
        var unit = tree.GetCompilationUnitRoot();

        foreach (var type in unit.DescendantNodes().OfType<TypeDeclarationSyntax>()) {
            if (type is InterfaceDeclarationSyntax) continue;
            var members = type.Members;
            if (members.Count < 2) continue;
            (int, int, int, int) high = (-1, -1, -1, -1);
            foreach (var member in members) {
                var key = key_of(member);
                if (key.CompareTo(high) < 0) {
                    var at = tree.GetLineSpan(member.Span).StartLinePosition.Line + 1;
                    found.Add($"{label}:{at}: '{type.Identifier.Text}.{name_of(member)}' is out of StyleCop order");
                }
                if (key.CompareTo(high) > 0) high = key;
            }
        }
        found.Sort(StringComparer.Ordinal);
        return found;
    }

    static (int kind, int sub, int acc, int stat) key_of(MemberDeclarationSyntax member)
    {
        int kind = kind_rank(member);
        int sub = member is FieldDeclarationSyntax f ? field_sub(f) : 0;
        var modifiers = modifiers_of(member);
        int stat = has(modifiers, "static") ? 0 : 1;
        int acc = accessibility_rank(modifiers);
        return (kind, sub, acc, stat);
    }

    static int kind_rank(MemberDeclarationSyntax member) => member switch {
        FieldDeclarationSyntax => 0,
        ConstructorDeclarationSyntax => 2,
        DestructorDeclarationSyntax => 3,
        DelegateDeclarationSyntax => 4,
        EventDeclarationSyntax => 5,
        EventFieldDeclarationSyntax => 5,
        EnumDeclarationSyntax => 6,
        InterfaceDeclarationSyntax => 7,
        PropertyDeclarationSyntax => 8,
        IndexerDeclarationSyntax => 9,
        MethodDeclarationSyntax => 10,
        OperatorDeclarationSyntax => 10,
        ConversionOperatorDeclarationSyntax => 10,
        StructDeclarationSyntax => 11,
        ClassDeclarationSyntax => 12,
        RecordDeclarationSyntax => 12,
        _ => 10
    };

    static int field_sub(FieldDeclarationSyntax field)
    {
        if (has(field.Modifiers, "const")) return 0;
        if (has(field.Modifiers, "static")) return 1;
        return 2;
    }

    static int accessibility_rank(SyntaxTokenList modifiers)
    {
        bool is_public = has(modifiers, "public");
        bool is_internal = has(modifiers, "internal");
        bool is_protected = has(modifiers, "protected");
        bool is_private = has(modifiers, "private");
        if (is_public) return 0;
        if (is_protected && is_internal) return 1;
        if (is_internal) return 2;
        if (is_protected && is_private) return 3;
        if (is_protected) return 4;
        return 5;
    }

    static SyntaxTokenList modifiers_of(MemberDeclarationSyntax member) => member switch {
        BaseFieldDeclarationSyntax f => f.Modifiers,
        BaseMethodDeclarationSyntax m => m.Modifiers,
        BasePropertyDeclarationSyntax p => p.Modifiers,
        BaseTypeDeclarationSyntax t => t.Modifiers,
        DelegateDeclarationSyntax d => d.Modifiers,
        _ => default
    };

    static string name_of(MemberDeclarationSyntax member) => member switch {
        MethodDeclarationSyntax m => m.Identifier.Text + "()",
        PropertyDeclarationSyntax p => p.Identifier.Text,
        FieldDeclarationSyntax f => string.Join(",", f.Declaration.Variables.Select(v => v.Identifier.Text)),
        ConstructorDeclarationSyntax => "<ctor>",
        _ => member.Kind().ToString()
    };

    // ---- shared ----------------------------------------------------------

    static bool has(SyntaxTokenList modifiers, string text) => modifiers.Any(m => m.Text == text);

    static bool exposed(SyntaxTokenList modifiers) =>
        has(modifiers, "public") || has(modifiers, "internal") || has(modifiers, "protected");

    // True when the node sits inside a type marked [Serializable]. Such a type
    // is a JSON-mapping DTO, so its public property names are external JSON keys
    // and are allowed to stay snake_case.
    static bool in_serializable_type(SyntaxNode node)
    {
        for (var current = node.Parent; current != null; current = current.Parent) {
            if (current is TypeDeclarationSyntax type) {
                foreach (var list in type.AttributeLists)
                    foreach (var attr in list.Attributes) {
                        var name = attr.Name.ToString();
                        if (name == "Serializable" || name == "System.Serializable")
                            return true;
                    }
            }
        }
        return false;
    }

    static bool in_overriding_member(SyntaxNode node)
    {
        for (var current = node.Parent; current != null; current = current.Parent) {
            if (current is MethodDeclarationSyntax m)
                return has(m.Modifiers, "override") || m.ExplicitInterfaceSpecifier != null;
            if (current is BasePropertyDeclarationSyntax p)
                return has(p.Modifiers, "override");
        }
        return false;
    }

    // Matches only as a camelCase hump, so 'Io' hits ReadIoPort but not Region.
    static bool is_hump(string identifier, string token)
    {
        for (int i = 0; i + token.Length <= identifier.Length; i++) {
            if (string.CompareOrdinal(identifier, i, token, 0, token.Length) != 0) continue;
            bool left_ok = i == 0 || char.IsLower(identifier[i - 1]) || char.IsDigit(identifier[i - 1]) || identifier[i - 1] == '_';
            int after = i + token.Length;
            bool right_ok = after == identifier.Length || char.IsUpper(identifier[after]) || char.IsDigit(identifier[after]) || identifier[after] == '_';
            if (left_ok && right_ok) return true;
        }
        return false;
    }

    static int line(SyntaxNode node) => node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

    static int line_of_token(SyntaxToken token) => token.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
}
