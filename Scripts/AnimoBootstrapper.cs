// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Animo {
    /// <summary>
    /// (v0.1.5, Q-S97) Unity scene-bootstrap MonoBehaviour for Animo.
    /// Pre-Q-S97 the spec described this class in §11.6.5 with full
    /// sample code, but no `Scripts/AnimoBootstrapper.cs` file existed
    /// in the repository — every spec reference to
    /// `Animo.AnimoBootstrapper` would have failed to resolve at compile
    /// time. Q-S97 ships the file with method declarations matching
    /// §11.6.5's signatures: Awake (loads JSON via Animo.Json.Parse +
    /// initializes PersonaCache; Q-S76+Q-S29) and OnDestroy (clears
    /// both PersonaCache and Store; Q-S58+Q-S78). Phase 3 wires up the
    /// bodies.
    ///
    /// The whole file is bracketed in `#if UNITY_5_3_OR_NEWER` so the
    /// headless dotnet test environment (which doesn't have UnityEngine)
    /// still compiles. Unity build pulls UnityEngine via the asmdef
    /// references (Q-S77).
    ///
    /// Usage: place a single instance of this MonoBehaviour in the
    /// initial scene with `[DefaultExecutionOrder(-1000)]` so its Awake
    /// runs before any Agent's Awake. Wire `_animo_json` to a TextAsset
    /// containing the JSON Root document.
    ///
    /// (v0.1.5, Q-S118) OnDestroy cleanup is editor-only —
    /// `if (!Application.isEditor || Application.isPlaying) return;`
    /// before the Q-S58 cleanup pair runs. Production scene transitions
    /// must NOT wipe the global `Store` because `DontDestroyOnLoad`
    /// Agents survive the scene change but their Store registrations
    /// would not (the bootstrapper attached to the OUTGOING scene's
    /// GameObject runs OnDestroy as that scene unloads). Q-S58's
    /// intent was *Editor Fast Play Mode static-state cleanup* — that
    /// remains correct, but the guard ensures the cleanup is scoped
    /// to that single use case.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [DefaultExecutionOrder(-1000)]
    public sealed class AnimoBootstrapper : MonoBehaviour {
        [SerializeField] TextAsset? _animo_json = null;

        void Awake() {
            // Phase 3 implementation per §11.6.5 spec narrative:
            //   (Q-S133) JSON parse failure contract — fail-loud:
            //   The parse can throw (malformed JSON, corrupted TextAsset).
            //   Do NOT silently swallow the exception. Let it propagate so
            //   the Unity console shows a fatal error with full stack trace,
            //   identifying the file and line in the JSON that caused the
            //   failure. Rationale: if Initialize is never called, every
            //   Agent.Awake immediately throws PersonaCacheNotInitializedException
            //   (Q-S111) with a cryptic message. The root cause (malformed JSON)
            //   is buried. Fail-loud at the parse site is always preferable.
            //   Phase 3 implementation pattern:
            //     Root root;
            //     try {
            //         root = Animo.Json.Parse(_animo_json!.text);
            //     } catch (System.Exception ex) {
            //         AnimoLog.Error($"AnimoBootstrapper: failed to parse animo.json: {ex.Message}");
            //         throw;  // re-throw: keep Unity's exception dialog and full stack trace
            //     }
            //     Animo.PersonaCache.Initialize(root: root);
            throw new System.NotImplementedException();
        }

        void OnDestroy() {
            // (v0.1.5, Q-S58 + Q-S78 + Q-S118) Phase 3 cleanup:
            //   if (!Application.isEditor || Application.isPlaying) return;
            //   Animo.PersonaCache.ClearForTesting();
            //   Animo.Store.ResetForTesting();   // type-name form, not Instance.
            //
            // Q-S118 editor-only guard: pre-Q-S118 the cleanup ran on
            // EVERY scene unload — including production scene
            // transitions. A `DontDestroyOnLoad` companion NPC
            // surviving the scene change would have its Store entry
            // wiped by the OUTGOING scene's bootstrapper, leaving
            // the alive-but-unrouted Agent unable to receive Bus
            // events. Q-S58's intent was *Editor Fast Play Mode
            // static-state cleanup* — purely a development concern.
            // The guard runs cleanup ONLY when isEditor && !isPlaying
            // (Editor after Stop), skipping production runtime and
            // any in-Play-mode scene transitions.
            //
            // Idempotent + cheap when the guard does fire; safe under
            // Unity Editor "Enter Play Mode Options (Fast)".
        }
    }
}

#endif // UNITY_5_3_OR_NEWER
