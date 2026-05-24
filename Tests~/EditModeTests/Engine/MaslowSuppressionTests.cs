// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.EngineTests {
    /// <summary>Step 4: Maslow dynamic suppression — numerical verification (spec §9.3.4).</summary>
    [TestFixture]
    public class MaslowSuppressionTests {

        // spec §9.4: score = Pow(intensity,exp)*100 * (1 - suppression_factor[tier] * max_lower_tier_intensity)

        [Test] public void Case01_Tier1HighNeed_SupressesTier5Action() {
            // hunger(tier1)=80, idle(tier5)=50
            // suppression.tier5=0.9 (max for tier5)
            // max_lower_tier_intensity = max(hunger/100=0.8) = 0.8
            // idle_score_unsuppressed = (50/100)^1 * 100 = 50
            // suppression_amount = 0.9 * 0.8 = 0.72
            // idle_score = 50 * (1 - 0.72) = 14.0
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("hunger",80f),("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Eat","hunger",1,1.0f), ActionOf("Idle","idle",5,1.0f) },
                suppression = new Suppression { tier5 = 0.9f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float idle_score  = e.GetActionScore("Idle");
            float unsuppressed = 50f;
            float expected    = unsuppressed * (1f - 0.9f * 0.8f);  // 14.0
            Assert.That(idle_score, Is.EqualTo(expected).Within(0.1f),
                $"Step 4 §9.3.4: Tier5 score must be suppressed. Expected {expected:F1}, got {idle_score:F1}.");
        }

        [Test] public void Case02_Tier5HighNeed_DoesNotSuppressTier1() {
            // idle(tier5) high — must NOT suppress hunger(tier1)
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("hunger",50f),("idle",80f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Eat","hunger",1,1.0f), ActionOf("Idle","idle",5,1.0f) },
                suppression = new Suppression { tier5 = 0.9f, tier2 = 0.7f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float eat_score   = e.GetActionScore("Eat");
            float expected    = 50f;  // tier1: no lower tiers → not suppressed
            Assert.That(eat_score, Is.EqualTo(expected).Within(0.1f),
                "Step 4: Tier1 action must NOT be suppressed by higher-tier Needs.");
        }

        [Test] public void Case03_SuppressorFactorZero_NoEffect() {
            // suppression.tier5=0 → score unchanged
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("hunger",80f),("idle",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Eat","hunger",1,1.0f), ActionOf("Idle","idle",5,1.0f) },
                suppression = new Suppression { tier5 = 0f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            Assert.That(e.GetActionScore("Idle"), Is.EqualTo(50f).Within(0.1f),
                "Step 4: suppression_factor=0 must produce no suppression effect.");
        }

        [Test] public void Case04_SuppressionFormula_MatchesSpec() {
            // spec §9.4: suppression_factor[tier] × max_lower_tier_intensity
            // = suppression.tier3 × max(tier1_needs, tier2_needs)
            // fear(tier2)=60, idle(tier3_action): need=confidence(tier3_need)=50
            // tier3_action_need has tiers 1+2 below it
            // hunger(tier1)=40, fear(tier2)=60 → max_lower = max(40,60)/100 = 0.6
            // suppression_amount = tier3_factor * 0.6
            // score_unsuppressed = (50/100)^1.0 * 100 = 50
            // score_suppressed   = 50 * (1 - tier3_factor * 0.6)
            float tier3_factor = 0.5f;
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("hunger",40f),("fear",60f),("confidence",50f)),
                actions = new List<Animo.Model.Action>{
                    ActionOf("Socialize","confidence",3,1.0f) },
                suppression = new Suppression { tier2=0f, tier3=tier3_factor, tier4=0f, tier5=0f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float max_lower   = 0.6f;  // fear=60
            float expected    = 50f * (1f - tier3_factor * max_lower);  // 50*(1-0.3)=35
            Assert.That(e.GetActionScore("Socialize"), Is.EqualTo(expected).Within(0.1f),
                $"Step 4 §9.4 formula: expected {expected:F1}.");
        }

        [Test] public void Case05_MissingLowerTier_UsesCorrectTierFactor() {
            // CRITICAL: this test catches the sf/supp_factor overwrite bug (Gemini 致命傷1)
            // act.tier=4 action. Tier 3 Needs are absent.
            // Loop: t2=1(hunger→max_lower=0.8), t2=2(fear→max_lower=0.8), t2=3(no tier3→continue)
            // BUG: supp_factor stays at tier3_factor*max_lower if t2=3 is skipped
            // CORRECT: suppression_factor is suppression.tier4 (factor for act.tier=4)
            //   max_lower = max(tier1+tier2 needs) = max(hunger=80, fear=60)/100 = 0.8
            //   suppression_amount = tier4_factor * 0.8
            float tier4_factor = 0.6f;
            float tier3_factor = 0.9f;  // wrong value — should NOT be used
            var p = new Persona { agent_id = "a",
                needs   = NeedsOf(("hunger",80f),("fear",60f),("confidence",50f)),
                // NO tier3 Needs in _need_tier_indices for this Persona
                actions = new List<Animo.Model.Action>{
                    ActionOf("Relax","confidence",4,1.0f) },  // tier 4 action
                suppression = new Suppression {
                    tier2 = 0f, tier3 = tier3_factor, tier4 = tier4_factor, tier5 = 0f }
            };
            var e = new Engine(p);
            e.Live(dt: 0.016f);
            float max_lower  = 0.8f;  // max(hunger=80,fear=60)/100
            float expected   = 50f * (1f - tier4_factor * max_lower);  // 50*(1-0.48)=26
            float wrong      = 50f * (1f - tier3_factor * max_lower);  // 50*(1-0.72)=14 ← bug value
            Assert.That(e.GetActionScore("Relax"), Is.Not.EqualTo(wrong).Within(0.1f),
                "Gemini 致命傷1: must NOT use tier3_factor when tier3 Needs are absent.");
            Assert.That(e.GetActionScore("Relax"), Is.EqualTo(expected).Within(0.1f),
                $"Step 4 §9.3.4: suppression_factor must be tier4 factor. Expected {expected:F1}.");
        }
    }
}
