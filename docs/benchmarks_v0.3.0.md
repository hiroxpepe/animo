# Animo Performance Benchmarks — v0.3.0

**Date**: Phase_3_5_1
**Build**: .NET 8.0 / Release build
**Platform**: Windows 11 (developer machine)
**Status**: ✅ All zero-GC contracts honored

## Summary

| Operation                              | Allocation       | Per-call (Release) | Contract |
| -------------------------------------- | ---------------- | ------------------ | -------- |
| `Engine.Live(dt)` — hot path           | 0 B / 100K calls | < 1 µs             | §16.1    |
| `Engine.Live(dt)` during Lock          | 0 B / 100K calls | < 1 µs             | §16.1    |
| `Engine.Affect(need, delta)`           | 0 B / 100K calls | < 1 µs             | §16.1    |
| `Engine.Affect(..., force_reset:true)` | 0 B / 100K calls | < 1 µs             | §16.1    |
| `Engine.Lock + Unlock` cycle           | 0 B / 10K cycles | < 1 µs             | §16.1    |
| `ScenarioRunner.Run` per-frame growth  | Linear           | N/A                | bounded  |

## Measurement Method

```csharp
// Warm up: JIT + one-time allocations
for (int i = 0; i < 1000; i++) engine.Live(dt: 0.016f);

GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();

long alloc_before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 100000; i++) engine.Live(dt: 0.016f);
long alloc_after  = GC.GetAllocatedBytesForCurrentThread();

Assert.That(alloc_after - alloc_before, Is.EqualTo(0));
```

## A Real-Case Persona

Benchmarks use a representative Persona profile:

+ **8 needs**: hunger, fatigue, fear, anger, loneliness, confidence, curiosity, idle
+ **4 actions**: Eat (tier 1), Rest (tier 1), Flee (tier 2), Idle (tier 5)
+ **3 influences**: fear→confidence, fatigue→confidence, loneliness→curiosity
+ **2 thresholds**: fear_critical (80), hunger_critical (90)
+ **suppression**: tier2=0.5, tier3=0.4, tier4=0.3, tier5=0.2
+ **commitment**: bonus=5.0

This profile exercises every Step (1–5) of `Live(dt)` including Maslow dynamic
suppression, Influence cascade with topological order, commitment bonus,
threshold hysteresis with OnSignal, and tie-break logic.

## What Makes Zero-GC Hold

The zero-allocation contract is achieved through these architectural decisions
(spec §16.1–16.5):

1. **Pre-cache Principle (§16.3.4)** — `need_index` baked into Action/Threshold/
   Influence at construction. Hot path uses `int[]` indexing, not `Dictionary[string]`.
2. **Topo-sorted influences (§16.3.4)** — sorted once by Composer, stored as
   `int[] _sorted_influence_order`. Step 2 iterates pre-ordered indices.
3. **Flat rates array** — `_rates_flat float[]` parallel to `_needs`. Step 1 uses
   a plain `for` loop, eliminating `foreach` over `Dictionary` (boxing).
4. **Cached threshold List** — `_thresholds` field caches `_persona.binding.thresholds`
   to avoid per-frame `IReadOnlyList<Threshold>` cast (would box the enumerator).
5. **`!.Value` over `??`** — Composer's Q-S11 guarantee that `reset_threshold` is
   non-null lets Step 3 use `t.reset_threshold!.Value` (zero overhead) instead of
   the per-frame `??` fallback expression.
6. **Computed properties** — `is_locked => _lock_remaining > 0f` is a single field
   read, no state mirror.

## Per-Call Timing (Release, .NET 8)

Measured on a developer-grade machine; representative not authoritative.

+ `Live(dt)` ≈ **0.86 µs** per call (86ms / 100,000 calls)
+ Per-Live aim: < 10 µs ✅ (10x room)
+ 100 agents @ 60 fps frame time: 16.67 ms — Animo consumes ~86 µs (0.5%)
+ 1000 agents @ 60 fps: ~860 µs (5% of frame time)

## What Is NOT Zero-Alloc (and Why)

`ScenarioRunner.Run` allocates intentionally:

+ `TraceResult` and its `frames` List
+ Per-frame `TraceFrame` + 3 inner Dictionaries (needs, effective, scores)
+ `signals_fired` capture buffer

These are part of the observation surface so a run can be studied and sent out as CSV.
The underlying engine loop remains zero-alloc; allocation is linear in frame
count (verified by `ScenarioRunnerAllocationTests`).

For production runtime (no trace observation), `Agent.Update` calls
`_engine.Live(dt)` directly and stays in the zero-alloc path.
