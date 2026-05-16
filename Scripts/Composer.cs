// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Runtime.CompilerServices;
using Animo.Model;

[assembly: InternalsVisibleTo("Animo.Tests.EditMode")]

namespace Animo.Core {
    /// <summary>
    /// Composes a final Persona by deep-copying the kind chain and applying
    /// persona-level overrides. See spec §10.
    ///
    /// (v0.1.5, Q-S85) When merging `binding.thresholds[]` lists, use
    /// **first-occurrence-wins** semantics: iterate the merged-so-far
    /// list IN ORDER, and the FIRST matching entry per `ThresholdsMatch`
    /// (need-name + EPSILON-tolerant trigger comparison, §8.3.1) is the
    /// one Persona overrides. Second matches are left untouched. This
    /// makes merge output order-deterministic despite the non-transitive
    /// nature of the EPSILON comparison (A=80.000, B=80.006, C=80.012
    /// has A≈B and B≈C but A≉C).
    ///
    /// (v0.1.5, Q-S11 + Q-S86 contract) Compose MUST fill every
    /// Threshold's `reset_threshold` with a numeric value before
    /// returning — either the author's explicit value, or
    /// `Math.Max(0f, trigger_threshold - 5f)` if author omitted it.
    /// Engine.Step3 (post-Q-S86) reads `reset_threshold!.Value`
    /// directly without null-coalescing in the hot path; a Composer
    /// that returns a null `reset_threshold` would crash with NRE on
    /// the first frame.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Composer {
        /// <summary>
        /// Build a fully-composed, deep-copied Persona by merging every Kind
        /// referenced in <c>persona.kind_ids</c> in order, then applying
        /// persona's own overrides last. Missing Need keys are filled with 0.0.
        /// </summary>
        internal static Persona Compose(Persona persona, Root root) {
            throw new NotImplementedException();
        }
    }
}
