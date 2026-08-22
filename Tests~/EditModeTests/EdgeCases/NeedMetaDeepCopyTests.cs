// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Model;

namespace Animo.Tests.EditMode.EdgeCaseTests {
    /// <summary>
    /// Decision-table test for Q-S134 (v0.1.5): NeedMeta must declare
    /// DeepCopy() as an explicit contract so v0.2 / v0.3 field additions
    /// are caught at compile time.
    ///
    /// Pre-Q-S134: Persona.DeepCopy() (Phase 3) would have written
    /// `new NeedMeta { tier = m.tier }` inline — silently omitting any
    /// future NeedMeta field. Q-S134 adds NeedMeta.DeepCopy() as a
    /// declared method; the build fails if the method body misses a
    /// field added in a future version (compiler forces exhaustive
    /// object-initializer update).
    ///
    /// Phase 2: verifies the method is declared and returns a separate
    /// object with equal fields. v0.1.5 only carries `tier` (value type).
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedMetaDeepCopyTests {

        [Test] public void Case01_DeepCopy_ReturnsSeparateInstance() {
            var original = new NeedMeta { tier = 3 };
            var copy = original.DeepCopy();
            Assert.That(copy, Is.Not.SameAs(original),
                "Q-S134: NeedMeta.DeepCopy() must return a new instance, not the same reference.");
        }

        [Test] public void Case02_DeepCopy_PreservesTier() {
            var original = new NeedMeta { tier = 5 };
            var copy = original.DeepCopy();
            Assert.That(copy.tier, Is.EqualTo(5),
                "Q-S134: NeedMeta.DeepCopy() must copy the tier field correctly.");
        }

        [Test] public void Case03_DeepCopy_MutatingCopyDoesNotAffectOriginal() {
            var original = new NeedMeta { tier = 2 };
            var copy = original.DeepCopy();
            copy.tier = 4;
            Assert.That(original.tier, Is.EqualTo(2),
                "Q-S134: mutating the copy must not affect the original (value-type isolation).");
        }

        [Test] public void Case04_SpecDocumentsDeepCopyContract() {
            // File-string verification: Scripts/Data.cs must contain
            // the NeedMeta.DeepCopy declaration with its v.0.2 extension note.
            var root = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location)!,
                    "..", "..", "..", "..", "..", "..", "Scripts", "Model", "Data.cs"));
            if (!System.IO.File.Exists(root)) root =
                System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(),
                    "Scripts", "Model", "Data.cs");
            // Repo-relative fallback: walk up until Scripts/Model/Data.cs is found.
            string? dir = System.IO.Directory.GetCurrentDirectory();
            while (dir != null && !System.IO.File.Exists(System.IO.Path.Combine(dir, "Scripts", "Model", "Data.cs")))
                dir = System.IO.Directory.GetParent(dir)?.FullName;
            if (dir != null) root = System.IO.Path.Combine(dir, "Scripts", "Model", "Data.cs");

            Assert.That(System.IO.File.Exists(root), Is.True, $"Scripts/Model/Data.cs not found at {root}");
            var text = System.IO.File.ReadAllText(root);
            Assert.That(text, Does.Contain("NeedMeta DeepCopy()"),
                "Q-S134: Scripts/Data.cs must declare NeedMeta.DeepCopy().");
            Assert.That(text, Does.Contain("v0.2 / v0.3 NeedMeta field additions"),
                "Q-S134: NeedMeta.DeepCopy() docstring must warn about future field additions.");
        }
    }
}
