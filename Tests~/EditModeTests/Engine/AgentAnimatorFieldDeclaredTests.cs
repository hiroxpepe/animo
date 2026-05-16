// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using NUnit.Framework;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>
    /// Decision-table test for Q-S75 (v0.1.5): `Agent._animator` field is
    /// declared as `[SerializeField] Animator? _animator = null`.
    /// Pre-Q-S75 §11.4.1 Awake step (6) called `_animator?.Play(...)`
    /// but the Agent class declaration had no _animator field —
    /// confirmed missing-field compile error.
    ///
    /// Phase 3 contract: Animo.Agent class has `_animator` field of type
    /// `UnityEngine.Animator` (nullable). Verified via reflection once
    /// Phase 3 ships the Agent class.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class AgentAnimatorFieldDeclaredTests {
        [Test] public void Case01_AgentClass_DeclaresAnimatorField() {
            Assert.Fail(message: "Phase 3 implementation pending: " +
                "typeof(Animo.Agent) must declare a private/serialized field `_animator` " +
                "of type Animator (nullable). Q-S75 fix: [SerializeField] Animator? " +
                "_animator = null. Verified at runtime once Agent class ships in Phase 3.");
        }
    }
}
