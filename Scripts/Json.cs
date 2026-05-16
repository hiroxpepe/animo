// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using Animo.Model;

namespace Animo {
    /// <summary>
    /// (v0.1.5, Q-S76) JSON parsing facade for `animo.json` files.
    /// Wraps the underlying serializer (Newtonsoft.Json in Phase 3 default
    /// build, or System.Text.Json in lean builds) and returns a fully-
    /// populated `Animo.Model.Root`.
    ///
    /// Pre-Q-S76 the §11.6.5 AnimoBootstrapper sample called
    /// `Animo.Json.Parse(...)` but neither the class nor any Parse method
    /// declaration existed anywhere in `Scripts/` — confirmed missing-
    /// type compile error. Q-S76 adds this stub so Bootstrapper compiles.
    ///
    /// Hosts that prefer a different JSON library can substitute by
    /// calling their library's deserializer directly in the bootstrapper
    /// — the wrapper exists for ergonomic parity with the rest of Animo's
    /// API surface.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Json {
        /// <summary>
        /// Parse an `animo.json` text payload into a `Root` aggregate.
        /// Phase 3 implementation calls `JsonConvert.DeserializeObject&lt;Root&gt;`
        /// (or equivalent) and validates basic shape. v0.1.5 stub
        /// throws NotImplementedException; Phase 3 implements.
        ///
        /// (v0.1.5, Q-S151) Phase 3 implementation MUST handle the
        /// `Needs` / `Rates` flat-JSON contract — see <c>Animo.Model.Needs</c>
        /// docstring. The JSON shape is <c>{"hunger": 40, "fatigue": 20}</c>;
        /// the C# class is <c>Needs { Dictionary&lt;string, float&gt; values }</c>;
        /// Newtonsoft's default deserializer DOES NOT bridge the two
        /// automatically (empirically: produces <c>values.Count == 0</c>).
        /// Phase 3 must either:
        ///   - register a custom <c>JsonConverter&lt;Needs&gt;</c> with the
        ///     <c>JsonSerializerSettings</c> used here, OR
        ///   - add <c>[JsonExtensionData]</c> attribute on a private
        ///     backing field inside <c>Needs</c> / <c>Rates</c> and project
        ///     to <c>values</c>.
        /// Without this, every Agent spawns with no Needs at all.
        /// </summary>
        public static Root Parse(string text) {
            throw new System.NotImplementedException();
        }
    }
}
