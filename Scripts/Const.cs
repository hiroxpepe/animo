// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace Animo {
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    // public Classes

    /// <summary>Animo domain constants. See spec §14.</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Const {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        public const int NEED_INDEX_HUNGER      = 0;
        public const int NEED_INDEX_FATIGUE     = 1;
        public const int NEED_INDEX_FEAR        = 2;
        public const int NEED_INDEX_LONELINESS  = 3;
        public const int NEED_INDEX_CONFIDENCE  = 4;
        public const int NEED_INDEX_CURIOSITY   = 5;
        public const int NEED_INDEX_IDLE        = 6;
        public const int NEED_INDEX_FRUSTRATION = 7;

        public const float MIN_NEED         =   0.0f;
        public const float MAX_NEED         = 100.0f;
        public const float MIN_EXPONENT     =   0.1f;
        public const float MAX_EXPONENT     =   5.0f;
        public const float MIN_COEFFICIENT  =  -1.0f;
        public const float MAX_COEFFICIENT  =   1.0f;
        public const float MIN_SUPPRESSION  =   0.0f;
        public const float MAX_SUPPRESSION  =   1.0f;
        public const int   MIN_TIER         =   1;
        public const int   MAX_TIER         =   5;
        public const int   MAX_ID_LENGTH    = 128;
        public const int   IDLE_TIER        =   5;

        public const float DEFAULT_RESET_OFFSET = 5.0f;
        public const float DEFAULT_COMMITMENT_BONUS = 0.0f;
        public const float MIN_COMMITMENT_BONUS = 0.0f;
        public const float MAX_COMMITMENT_BONUS = 50.0f;
        public const float COMMITMENT_BONUS_WARN_THRESHOLD = 30.0f;

        public const float LOCK_DURATION_WARN_THRESHOLD = 30.0f;
        public const float LOCK_DURATION_MAX = 600.0f;

        public const string CURRENT_SCHEMA_VERSION = "1.5";

        public const string DEFAULT_ON_ACTION_CHANGE = "animo_{agent_id}_{behavior}";

        // (v0.1.5, Q-S131) Type widened to `IReadOnlyList<string>` — same
        // pattern as Q-S128 which widened NEED_INDICES_BY_TIER.
        // Pre-Q-S131 the type was `string[]`: although the field is
        // `static readonly`, C# `readonly` only forbids *reassigning*
        // the field itself; the array elements remained mutable, so
        // external code calling `Const.STANDARD_NEEDS[0] = "fake"` would
        // corrupt the standard Need definition for every Engine in the
        // process — a process-global Maslow hierarchy collapse with no
        // compiler warning. `Array.AsReadOnly` returns a
        // `ReadOnlyCollection<string>` which implements `IReadOnlyList<string>`;
        // the public surface has no index-setter. Phase 3 code that needs
        // the count uses `.Count` (same value as the pre-Q-S131 `.Length`).
        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Fields

        public static readonly System.Collections.Generic.IReadOnlyList<string> STANDARD_NEEDS =
            System.Array.AsReadOnly(new[] {
                "hunger", "fatigue", "fear",
                "loneliness", "confidence", "curiosity",
                "idle", "frustration"
            });

        // v0.1.5 (Q-S16): standard Need → Tier map. The Maslow suppression
        // formula in §9.3.4 (`max_lower_tier_intensity = max(eff_needs[tier1
        // needs] / 100, eff_needs[tier2 needs] / 100, ...)`) needs this
        // mapping to compute. Pre-Q-S16 the §3.5 table was authoritative
        // documentation but the Engine had no way to read it. Non-standard
        // Needs (those reported as A019 Warning) are NOT in this map and
        // are EXCLUDED from `max_lower_tier_intensity` (treated as if they
        // had no tier). See §3.5 / §9.3.4.
        //
        // (v0.1.5, Q-S150) Type widened to `IReadOnlyDictionary<string, int>`
        // — same pattern as Q-S128 (NEED_INDICES_BY_TIER) and Q-S131
        // (STANDARD_NEEDS et al). Pre-Q-S150 the type was mutable
        // `Dictionary<string, int>`: external code could call
        // `Const.NEED_TIER_BY_NAME["hunger"] = 99` and silently corrupt the
        // Maslow tier mapping for every Engine in the process, causing the
        // dynamic suppression formula to compute wrong tier intensities.
        // Q-S128 + Q-S131 hardened three other Const members in the same
        // sweep; this completes the set.
        public static readonly System.Collections.Generic.IReadOnlyDictionary<string, int> NEED_TIER_BY_NAME =
            new System.Collections.Generic.Dictionary<string, int> {
            { "hunger",      1 },
            { "fatigue",     1 },
            { "fear",        2 },
            { "frustration", 2 },
            { "loneliness",  3 },
            { "confidence",  4 },
            { "curiosity",   5 },
            { "idle",        5 }
        };

        // v0.1.5 (Q-S16 + Q-S128): inverse map for Engine hot-path use.
        // (Q-S128) Type widened to `IReadOnlyDictionary<int, IReadOnlyList<int>>`
        // — both the outer Dictionary AND the inner int[] arrays are
        // exposed as read-only. Pre-Q-S128 the type was
        // `Dictionary<int, int[]>`: although the outer field was
        // `static readonly`, C# `readonly` only forbids reassigning
        // the field itself; the int[] array elements remained
        // mutable, so external code calling
        // `Const.NEED_INDICES_BY_TIER[1][0] = 99;` would corrupt the
        // tier mapping for every Engine in the process. Q-S128 wraps
        // each int[] with `Array.AsReadOnly` (returns
        // `ReadOnlyCollection<int>` which implements `IReadOnlyList<int>`)
        // and exposes the outer dictionary through `IReadOnlyDictionary`
        // (no Add / index-setter on the public surface). Phase 3
        // implementations should snapshot this into an int[][] keyed
        // by tier index for zero-allocation lookup; the snapshot copy
        // is the place where indexed-write access can happen safely
        // (Engine-local mutable state, never the shared Const).
        public static readonly System.Collections.Generic.IReadOnlyDictionary<int, System.Collections.Generic.IReadOnlyList<int>> NEED_INDICES_BY_TIER = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.IReadOnlyList<int>> {
            { 1, System.Array.AsReadOnly(new[] { NEED_INDEX_HUNGER, NEED_INDEX_FATIGUE     }) },
            { 2, System.Array.AsReadOnly(new[] { NEED_INDEX_FEAR,   NEED_INDEX_FRUSTRATION }) },
            { 3, System.Array.AsReadOnly(new[] { NEED_INDEX_LONELINESS                   }) },
            { 4, System.Array.AsReadOnly(new[] { NEED_INDEX_CONFIDENCE                   }) },
            { 5, System.Array.AsReadOnly(new[] { NEED_INDEX_CURIOSITY, NEED_INDEX_IDLE   }) }
        };

        // (v0.1.5, Q-S131) Same IReadOnlyList widening applied to all
        // public string[] constants — same mutable-element risk as
        // STANDARD_NEEDS above.
        public static readonly System.Collections.Generic.IReadOnlyList<string> SUPPORTED_SCHEMA_VERSIONS =
            System.Array.AsReadOnly(new[] { "1.3", "1.4", "1.5" });

        public static readonly System.Collections.Generic.IReadOnlyList<string> TEMPLATE_PLACEHOLDERS_ACTION =
            System.Array.AsReadOnly(new[] { "agent_id", "behavior" });

        public static readonly System.Collections.Generic.IReadOnlyList<string> TEMPLATE_PLACEHOLDERS_THRESHOLD =
            System.Array.AsReadOnly(new[] { "agent_id" });
    }
}
