// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using Animo.Core;
using Animo.Model;

namespace Animo {
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>
    /// (v0.1.5, Q-S29 + Q-S38 + Q-S79) Per-template Flyweight cache for
    /// composed Personas. Holds the result of `Composer.Compose` for each
    /// `template_id` so repeated `Agent.Awake` calls don't redundantly
    /// re-validate and re-compose the same JSON shape — N-fold cost
    /// collapses to 1×Validate + N×(Compose, N = unique templates) +
    /// M×DeepCopy for M Agent spawns.
    ///
    /// Q-S38 fail-loud: stage-2 errors throw `InvalidOperationException`
    /// instead of returning a broken Persona — `Agent.Awake` catches and
    /// disables the Agent without taking down the scene.
    ///
    /// Pre-Q-S79 §11.6.1 contained the implementation as spec text but
    /// no `Scripts/PersonaCache.cs` file existed in the repository —
    /// `Agent.Awake`'s `Animo.PersonaCache.GetComposed(...)` call would
    /// fail to compile because the type didn't exist. Q-S79 materializes
    /// the file with method declarations matching §11.6.1; Phase 3
    /// implements the bodies.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PersonaCache {
        // (v0.1.5, Q-S79) Phase 3 implementation will read/write these
        // fields in Initialize/GetComposed bodies; v0.1.5 stub doesn't
        // yet, hence the CS0414 suppression. ClearForTesting (below)
        // already writes them to null/clear so the suppression is
        // narrow to the field declarations only.
        #pragma warning disable CS0414

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Fields

        static Root? _root;
        static ValidationResult? _validation;
        #pragma warning restore CS0414
        static readonly Dictionary<string, Persona> CACHE = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Set the Root once at app startup. Runs Validator on Root
        /// (stage 1 + stage-2-on-demand-via-GetComposed). Idempotent:
        /// subsequent Initialize calls overwrite Root and clear cache.
        /// </summary>
        public static void Initialize(Root root) {
            _root       = root;
            _validation = Validator.Validate(root);
            CACHE.Clear();
        }

        /// <summary>
        /// Compose-once accessor. The first call per template runs
        /// `Composer.Compose` + `Validator.ValidateStage2`; subsequent
        /// calls return the cached composed Persona. Caller MUST
        /// `DeepCopy()` the returned Persona before mutation (Q-S64).
        ///
        /// (v0.1.5, Q-S111) Throws `PersonaCacheNotInitializedException`
        /// if `Initialize(root)` has not been called (architectural
        /// startup error — Bootstrapper missing or wrong execution
        /// order). Throws `PersonaTemplateRejectedException` for
        /// per-template authoring errors: unknown `template_id`
        /// (Q-S103) or stage-2 validation failure (Q-S38 fail-loud).
        /// `Agent.Awake` distinguishes the two so logs can name the
        /// real cause instead of disguising one as the other.
        /// (Q-S144) PersonaCache throws only — logging is Agent.Awake's job.
        /// </summary>
        public static Persona GetComposed(string template_id) {
            if (_root == null)
                throw new PersonaCacheNotInitializedException(
                    "PersonaCache.GetComposed: Initialize(root) has not been called. " +
                    "Ensure AnimoBootstrapper runs before any Agent.Awake.");

            if (CACHE.TryGetValue(template_id, out var cached)) return cached;

            Persona? raw = null;
            foreach (var persona in _root.personas)
                if (persona.agent_id == template_id) { raw = persona; break; }
            if (raw == null)
                throw new PersonaTemplateRejectedException(
                    $"PersonaCache.GetComposed: template_id '{template_id}' not found in Root.");

            var composed = Composer.Compose(persona: raw, root: _root);
            var stage2   = Validator.ValidateStage2(composed);

            if (stage2.has_errors) {
                var messages = string.Join("; ",
                    System.Linq.Enumerable.Select(stage2.errors, e => $"{e.rule_id}: {e.message}"));
                throw new PersonaTemplateRejectedException(
                    $"PersonaCache.GetComposed: template_id '{template_id}' failed stage-2: {messages}");
            }

            if (_validation != null) _validation.Merge(stage2);
            CACHE[template_id] = composed;
            return composed;
        }

        /// <summary>
        /// Test-only cleanup. Called by `AnimoBootstrapper.OnDestroy`
        /// (Q-S58) and headless test teardown to reset cache + root +
        /// validation state. Idempotent.
        /// </summary>
        public static void ClearForTesting() {
            _root = null;
            _validation = null;
            CACHE.Clear();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>
    /// (v0.1.5, Q-S111) Thrown by `PersonaCache.GetComposed` when
    /// `Initialize(root)` has not been called yet. Distinct exception
    /// type so `Agent.Awake` can handle it differently than
    /// `PersonaTemplateRejectedException` — an architectural startup
    /// bug (Bootstrapper missing or in wrong execution order) is
    /// fundamentally different from a per-Agent JSON authoring bug,
    /// and a single shared `InvalidOperationException` catch could
    /// disguise the former as the latter, hiding the root cause
    /// behind a "stage-2 fail-loud" log entry that talks about a
    /// completely unrelated problem.
    ///
    /// Pre-Q-S111 both errors threw bare `InvalidOperationException`,
    /// `Agent.Awake` caught the union, and the log message claimed
    /// "Q-S38 stage-2 fail-loud" even when Bootstrapper had never
    /// run. Diagnosis from logs alone was impossible; engineers had
    /// to attach a debugger to discover the real cause. Q-S111
    /// splits the exception types so the catch can distinguish them
    /// and produce honest diagnostics.
    ///
    /// (v0.1.5, Q-S143) [Serializable] added per C# custom exception
    /// best practice. Required for correct behavior if the exception
    /// crosses AppDomain boundaries (e.g. Unity Editor assembly reload,
    /// Editor test runner host↔sandbox communication). Without it,
    /// serialization of the exception loses state silently on some
    /// .NET runtimes. The base class `InvalidOperationException` is
    /// [Serializable] itself, but derived classes must declare the
    /// attribute independently to be correctly serialized.
    /// </summary>
    [Serializable]
    public sealed class PersonaCacheNotInitializedException : InvalidOperationException {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PersonaCacheNotInitializedException(string message) : base(message) { }
    }

    /// <summary>
    /// (v0.1.5, Q-S111) Thrown by `PersonaCache.GetComposed` when a
    /// per-template error blocks composition: stage-2 validation
    /// failure (Q-S38), or the requested `template_id` does not
    /// match any Persona in the loaded Root (Q-S103). Both are
    /// authoring errors in the JSON, surfaced per Agent so the
    /// rest of the scene can keep running.
    ///
    /// Distinct from `PersonaCacheNotInitializedException`
    /// (architectural startup error) so `Agent.Awake` can produce
    /// honest, actionable diagnostics for each case.
    ///
    /// (v0.1.5, Q-S143) [Serializable] added — see
    /// PersonaCacheNotInitializedException docstring for rationale.
    /// </summary>
    [Serializable]
    public sealed class PersonaTemplateRejectedException : InvalidOperationException {
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PersonaTemplateRejectedException(string message) : base(message) { }
    }
}
