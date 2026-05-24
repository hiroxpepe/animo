// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using Animo.Core;
using Animo.Model;
using Germio;

namespace Animo {
    /// <summary>
    /// (v0.1.5, Q-S83) Unity MonoBehaviour adapter for `Animo.Core.Engine`.
    /// Pre-Q-S83 the spec described the implementation in §11.4.1 but no
    /// `Scripts/Agent.cs` file existed in the repository — every spec
    /// reference to `Animo.Agent` would have failed to resolve at compile
    /// time. Q-S83 ships the file with method declarations matching
    /// §11.4.1's signature: Awake (Q-S28+Q-S34+Q-S38+Q-S68), Update
    /// (Q-S80), OnDestroy (Q-S22). Phase 3 wires up the bodies.
    ///
    /// The whole file is bracketed in `#if UNITY_5_3_OR_NEWER` so the
    /// headless dotnet test environment (which doesn't have UnityEngine)
    /// still compiles. Unity build pulls UnityEngine + Germio via the
    /// asmdef references (Q-S77).
    ///
    /// (v0.1.5, Q-S96 + Q-S101) The agent_id getter is null-safe and
    /// OnDestroy is guarded against the Awake-failed case. Q-S96
    /// (Phase_2_4_21) added these to the spec narrative; Q-S101
    /// (Phase_2_4_22) backports them to the physical file — the
    /// Phase_2_4_21 N-round consistency review caught EN+JP+code-blocks
    /// integrity but did not extend to Scripts/*.cs files. Without the
    /// null-safe getter, the chain
    ///   OnDestroy → Store.Unregister(this) → agent_id getter
    ///     → _composed_persona.agent_id (NRE if null)
    /// would crash scene unload for any Agent that hit the Q-S38
    /// fail-loud catch in Awake — breaking Q-S38's "keep-scene-alive"
    /// promise. The null-coalesce returns "&lt;uninitialized&gt;"
    /// (snake_case rules forbid angle brackets so the sentinel never
    /// collides with a real id), and the OnDestroy early-return keeps
    /// the unload path silent for the expected Awake-failed case.
    ///
    /// (v0.1.5, Q-S102 + Q-S111 + Q-S112) Phase 3 Awake implementation
    /// must observe additional cross-references from §11.4.1 spec:
    ///   - Q-S102: `_animator?.Play(stateName: _engine.behavior)` —
    ///     pass the RAW behavior id, NOT the GetExpandedActionTrigger
    ///     output. Animator Controllers use static state names.
    ///   - Q-S111: catch `PersonaTemplateRejectedException` (per-Agent
    ///     authoring error, disable+continue) but NOT
    ///     `PersonaCacheNotInitializedException` (architectural startup
    ///     bug, propagate so Bootstrapper missing is loud).
    ///   - Q-S112: emit `AnimoLog.Warning` once at Awake start when
    ///     `_bus == null` per §12.1 contract — `?.Publish` alone is
    ///     not enough; the contracted authoring-aid Warning must fire.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public sealed class Agent : MonoBehaviour, IAnimoAgent {
        [SerializeField] string _persona_template_id = "";
        [SerializeField] Bus? _bus = null;
        [SerializeField] Animator? _animator = null;
        Persona _composed_persona = null!;
        Engine  _engine           = null!;

        // (v0.1.5, Q-S96 + Q-S101) Null-safe getter — see class docstring.
        // Returns "<uninitialized>" sentinel if Q-S38's fail-loud catch
        // ran before _composed_persona was assigned in Awake step (3).
        public string agent_id => _composed_persona?.agent_id ?? "<uninitialized>";

        // (Q-S115) Optional time provider injected by test harness.
        // null → falls back to Unity Time.deltaTime in Update.
        ITimeProvider? _time_provider = null;
        /// <summary>(Q-S115) Inject custom time provider for deterministic tests.</summary>
        public void SetTimeProvider(ITimeProvider provider) => _time_provider = provider;

        void Awake() {
            // (Q-S112) Warn once if Bus is null — authoring aid, not fatal.
            if (_bus == null)
                AnimoLog.Warning($"Agent '{name}': no Bus assigned. OnSignal events will not route.");

            try {
                // Step 1 (Q-S34): Get composed Persona from cache.
                // Throws PersonaCacheNotInitializedException if Bootstrapper missed.
                // Throws PersonaTemplateRejectedException if stage-2 validation failed.
                var template = PersonaCache.GetComposed(_persona_template_id);

                // Step 2 (Q-S64): Deep copy so runtime mutations stay isolated.
                _composed_persona = template.DeepCopy();

                // Step 3 (Q-S28): Override agent_id with runtime-unique value.
                _composed_persona.agent_id = $"{_persona_template_id}_{GetInstanceID()}";

                // Step 4 (Q-S34): Construct Engine with composed Persona.
                _engine = new Engine(_composed_persona);

                // Step 5 (Q-S22): Register in Store.
                Animo.Store.Instance.Register(agent: this);

                // Step 6 (Q-S102): Wire OnSignal to Bus and Animator.
                _engine.OnSignal += OnSignalHandler;

                // Step 7 (Q-S80): Seed first behavior (silent per Q-S31).
                _engine.Live(dt: 0.0f);

                // Step 8 (Q-S75): Sync Animator to initial behavior.
                _animator?.Play(stateName: _engine.behavior);
                _last_played_behavior = _engine.behavior;
            }
            catch (PersonaCacheNotInitializedException) {
                // (Q-S111) Bootstrapper architectural error — PROPAGATE, do not catch.
                // This represents a scene-level startup bug (wrong execution order, missing
                // Bootstrapper). Propagating causes Unity's hard error dialog, making the
                // root cause immediately visible. Swallowing it with AnimoLog.Error + enabled=false
                // hides a fatal architecture error as a silent per-Agent disable.
                throw;
            }
            catch (PersonaTemplateRejectedException ex) {
                // (Q-S38 + Q-S144) Per-Agent authoring error. Keep scene alive.
                AnimoLog.Error($"Agent '{name}' template '{_persona_template_id}' rejected: {ex.Message}");
                enabled = false;
            }
        }

        string _last_played_behavior = "";

        void OnSignalHandler(string signal_id) {
            // (Q-S102) Publish to Bus on every signal (Step 3 threshold + Step 5 behavior).
            _bus?.Publish(signal_id);
            // (#1) Only call Animator.Play when behavior actually changes.
            // Engine.OnSignal fires for BOTH Step 3 threshold events AND Step 5 behavior
            // switches; calling Play on every signal would restart the current animation
            // from frame 0 each time a threshold crosses, causing visible stutter.
            string current = _engine.behavior;
            if (current != _last_played_behavior) {
                _animator?.Play(stateName: current);
                _last_played_behavior = current;
            }
        }

        /// <summary>(Q-S4) Relay external Affect from Store to this Agent's Engine.</summary>
        public void Affect(string need, float delta, bool force_reset = false) =>
            _engine?.Affect(need, delta, force_reset);

        void Update() {
            // (Q-S80) Per-frame engine tick.
            //
            // (v0.1.5, Q-S115) Phase 3 receives an `ITimeProvider`
            // (constructor-injected or SerializeField) so this line
            // becomes `_engine.Live(dt: _time_provider.deltaTime);`,
            // letting `Animo.Tests.MiniUnity.MockTime` drive the
            // simulator under EditMode. Pre-Q-S115 a hardcoded
            // `Time.deltaTime` here meant `MockScene.Tick(dt)` could
            // not advance the Agent's simulated time even though
            // `MockTime.Step(dt)` was correctly updating
            // `MockTime.deltaTime`. The v0.1.5 stub keeps the direct
            // reference because the Phase 3 implementation contract
            // hasn't been written yet, but the Phase 3 contract is
            // recorded here and in spec §11.4.1.
            //
            // (v0.1.5, Q-S147) Guard against _engine being null when
            // Update is called before Awake completes or after Awake's
            // Q-S38 fail-loud catch disables this Agent.
            //
            // (#5) MockScene/MockMonoBehaviour parity was the original justification
            // but it does not apply: MockGameObject.AddComponent<T> has the constraint
            // `where T : MockMonoBehaviour`, and Agent extends Unity's MonoBehaviour,
            // so MockScene cannot host an Agent — the C# type system forbids it.
            // The guard is kept for the Unity-only race condition: between Awake
            // throwing/disabling and the engine's next frame, Update may still fire
            // once on a frame where `_engine` was never assigned.
            if (_engine == null) return;
            // (Q-S115) Use injected ITimeProvider if available, else Unity Time.deltaTime.
            float dt = _time_provider != null ? _time_provider.deltaTime : Time.deltaTime;
            _engine.Live(dt: dt);
        }

        void OnDestroy() {
            // (v0.1.5, Q-S96 + Q-S101) Early-out if Awake's Q-S38 fail-loud
            // catch disabled this Agent before step (4) Register. Without
            // this guard, Store.Unregister(this) would dereference
            // agent_id (Q-S101 made it null-safe so it returns
            // "<uninitialized>"), and Store would log a "not registered"
            // Warning at scene-unload time for every Awake-failed Agent.
            // The early-out keeps the unload path silent for the
            // expected case.
            if (_composed_persona == null) return;
            // (v0.1.5, Q-S140) Release Lock on scene unload per §24.6.2:
            // "Agent.OnDestroy must call Engine.Unlock() to prevent
            // leftover lock state when scenes change." Unlock is
            // idempotent (no-op if not locked), so calling it here is
            // always safe. Phase 3 must keep this call BEFORE the
            // Store.Unregister below — Unlock may publish an OnSignal
            // event (behavior-change upon release), and the Agent must
            // still be registered at that point for Bus routing to work.
            _engine?.Unlock();
            // (Q-S22) Instance-equality-checked unregister.
            Animo.Store.Instance.Unregister(agent: this);
        }
    }
}

#endif // UNITY_5_3_OR_NEWER
