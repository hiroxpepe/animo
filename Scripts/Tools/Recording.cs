// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using Animo.Core;

namespace Animo.Tools {

    /// <summary>
    /// Keeps the snapshot of each frame of a run so the run can be looked at
    /// again. It grows as the loop ticks; on play-back a reader seeks to any
    /// frame by index, and an index outside the run clamps to the nearest end
    /// rather than throwing, so a scrub bar can never fall off.
    /// </summary>
    public sealed class Recording {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly List<EngineSnapshot> _frames = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Properties [noun, adjective]

        /// <summary>How many frames the recording holds.</summary>
        public int Count => _frames.Count;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>Append one frame — the snapshot the loop just handed out.</summary>
        public void Add(EngineSnapshot snapshot) {
            _frames.Add(snapshot);
        }

        /// <summary>
        /// The frame at an index. An index past either end clamps to the nearest
        /// frame, so a scrub never asks for a frame that is not there.
        /// </summary>
        public EngineSnapshot Frame(int index) {
            if (_frames.Count == 0) throw new System.InvalidOperationException("Recording is empty.");
            if (index < 0) index = 0;
            if (index >= _frames.Count) index = _frames.Count - 1;
            return _frames[index];
        }

        /// <summary>Drop every frame, ready for a fresh run.</summary>
        public void Clear() {
            _frames.Clear();
        }
    }
}
