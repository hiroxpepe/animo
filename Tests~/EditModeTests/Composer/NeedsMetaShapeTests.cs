// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;
using Animo.Model;

namespace Animo.Tests.EditMode.ComposerTests {
    /// <summary>
    /// Decision-table test for Q-S36 (v0.1.5): Persona/Kind have
    /// optional `needs_meta` Dictionary, and NeedMeta type exists.
    /// Compile-time verification that Q-S30 spec has runtime backing.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class NeedsMetaShapeTests {
        [Test] public void Case01_PersonaHasNeedsMetaProperty_NeedMetaExists() {
            // Compile-time verification: if Persona.needs_meta or
            // NeedMeta were missing, the assignments below would not
            // compile. The test passes immediately when the properties
            // ship in this phase.
            var p = new Persona { agent_id = "x" };
            p.needs_meta = new System.Collections.Generic.Dictionary<string, NeedMeta> {
                { "oxygen", new NeedMeta { tier = 1 } }
            };
            Assert.That(p.needs_meta!["oxygen"].tier, Is.EqualTo(expected: 1),
                "Q-S36: Persona.needs_meta must be a Dictionary<string, NeedMeta> " +
                "with NeedMeta.tier int field. Without these types, Q-S30 spec is " +
                "unimplementable and Engine ctor's _persona.needs_meta reference is " +
                "a compile error.");

            var k = new Kind { kind_id = "y" };
            k.needs_meta = new System.Collections.Generic.Dictionary<string, NeedMeta> {
                { "thirst", new NeedMeta { tier = 1 } }
            };
            Assert.That(k.needs_meta!["thirst"].tier, Is.EqualTo(expected: 1),
                "Q-S36: Kind.needs_meta must also exist (mergeable from Kind side per §8.3)");
        }
    }
}
