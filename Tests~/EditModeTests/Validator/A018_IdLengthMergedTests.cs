// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>Decision-table tests for Validator rule A018: agent_id/kind_id ≤ 128 (merged into A002/A003).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A018_IdLengthMergedTests {
        [Test] public void Case01_Over128CharsKindId_FailsA018OrA003() {
            Root root = MinimalRoot(); root.kinds.Add(item: KindOf(kind_id: new string('a', 129)));
            ValidationResult r = Validator.Validate(root: root);
            Assert.That(r.HasRule(rule_id: "A018") || r.HasRule(rule_id: "A003"), Is.True);
        }
    }
}
