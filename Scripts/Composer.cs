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
