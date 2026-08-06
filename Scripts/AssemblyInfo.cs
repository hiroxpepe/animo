// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Runtime.CompilerServices;

// v0.1.5 (Q-S32): expose internal Engine debug accessors
// (GetEffectiveNeed, GetActionScore, GetAllNeedNames, GetAllActionIds)
// to the Animo.Tools assembly so ScenarioRunner can populate
// TraceFrame.{effective_needs, action_scores}. The hot path inside
// Engine still uses direct float[] index access; these accessors
// are explicitly cold-path (allocate Dictionary copies).
[assembly: InternalsVisibleTo("Animo.Tools")]

// v0.1.5: also expose to test assemblies for white-box assertions.
// The actual EditMode test assembly target is determined by the
// test csproj's AssemblyName.
[assembly: InternalsVisibleTo("Animo.Tests.EditMode")]
[assembly: InternalsVisibleTo("Animo.Tests.MiniUnity.SelfTests")]
