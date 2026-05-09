// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;

namespace Animo.Tests.MiniUnity {
    /// <summary>
    /// Pure-C# stand-in for <c>Germio.Bus</c>. Records every signal id that
    /// <c>Animo.Agent</c> publishes (action change, threshold trigger, etc.)
    /// so tests can assert sequence and content.
    ///
    /// Order is preserved. Reset between test cases via <see cref="Reset"/>.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class MockBus {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<string> _published_signals = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun]

        /// <summary>
        /// Read-only view of every signal published since the last <see cref="Reset"/>,
        /// in publish order.
        /// </summary>
        public IReadOnlyList<string> published_signals => _published_signals;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>Record a published signal id.</summary>
        /// <param name="signal_id">The signal identifier as it would appear on the real Bus.</param>
        public void Publish(string signal_id) {
            _published_signals.Add(item: signal_id);
        }

        /// <summary>Drop every recorded signal. Call between test cases for isolation.</summary>
        public void Reset() {
            _published_signals.Clear();
        }
    }
}
