# Animo Changelog

All notable changes to the Animo Utility AI engine are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v0.3.0-alpha] — Phase 3 Complete (Core Engine + ScenarioRunner)

### Added

- **Animo.Model**: full data classes (`Root`, `Persona`, `Kind`, `Needs`, `Rates`,
  `Suppression`, `Influence`, `Action`, `Commitment`, `Binding`, `Threshold`, `NeedMeta`)
  with `DeepCopy()` for runtime isolation.
- **`Composer.Compose`**: Kind cascade with Persona-first ordering (Q-S19/Q-S20),
  topological sort of influences at compose time (§16.3.4), `FillResetThresholds`
  (Q-S11/Q-S86), `FillMissingNeeds` (Q-S7).
- **`Validator`**: full A000-A040 rule coverage (Stage 1 + Stage 2). Includes
  A031 runtime warning on `Engine.Lock(duration > 30s)`, A039 sibling threshold
  proximity in both raw and composed forms, NaN/Infinity guards on rates.
- **`Engine`**: 5-step `Live(dt)` lifecycle (decay → EffectiveNeeds → Threshold
  → Score → Switch) with §16.3.4 Pre-cache Principle (`_rates_flat` flat array,
  pre-sorted influence order, baked `need_index`). `Affect`, `Lock` (with NaN
  guard + `LOCK_DURATION_MAX` hard cap), `Unlock`. `OnSignal` event for both
  threshold crossings (Step 3) and behavior switches (Step 5).
- **`ScenarioRunner`**: headless simulation harness routed through
  `PersonaCache.GetComposed` for Stage 2 validation parity with Unity Agents.
  Stable `OrderBy` event sort, next-pointer event consumption, RecordFrame
  with `is_locked` / `locked_behavior` / `signals_fired`, cached need/action
  identifiers (no per-frame allocation).
- **`TraceResult`**: `ToCsv()` (InvariantCulture), `ToJson()`, `BuildAnalysis()`
  with zero-time-frame skip for accurate `behavior_count` / `behavior_total_time`.
- **`Agent`** (`MonoBehaviour`): full `Awake` implementing Q-S28/Q-S38/Q-S64/
  Q-S102/Q-S111/Q-S112/Q-S144. `OnSignalHandler` gated by behavior change
  (prevents Animator restart on threshold signals). `Affect` relay from Store.
- **`AnimoBootstrapper`** (`MonoBehaviour`): `Awake` with fail-loud JSON parse
  per Q-S133.
- **`PersonaCache`**: Flyweight cache with Stage 2 validation on demand. Two
  distinct exceptions (`PersonaCacheNotInitializedException` for architectural
  errors, `PersonaTemplateRejectedException` for per-template authoring errors).
- **`Json`**: `Newtonsoft.Json` facade with `NeedsConverter` / `RatesConverter`
  for the Q-S151 flat-object shape.
- **`ITimeProvider`** DI seam (Q-S115) for headless time injection.
- **`AnimoLog.OnLog`** static hook for emitted message capture in tests.
- **Zero-GC benchmarks**: `EngineLiveAllocationTests`, `AffectAllocationTests`,
  `LockAllocationTests`, `ScenarioRunnerAllocationTests` — all Green.
- **Goblin example end-to-end test**: loads `examples/goblin_scout.json` from
  disk, runs `ScenarioRunner` for 10 seconds @ 60Hz, verifies trace integrity.
- **Spec §9.3.5 simulation table** reproduced exactly in tests (4 rows,
  drift < 1e-3).

### Performance

- `Engine.Live(dt)` per-call: **≈ 0.86 µs** (Release, .NET 8) — well under
  the 10 µs target.
- `Engine.Live` over 100,000 calls: **0 bytes** allocated.
- `Engine.Affect`, `Engine.Lock` / `Unlock`: **0 bytes** allocated.
- 100 agents @ 60 fps frame budget: ~0.5% consumption.

### Documentation

- `docs/animo_spec_v0.1.5_EN.md` / `_JP.md` — 151 Q-S spec patches.
- `docs/decisions/v0.1.5_ambiguity_resolution.md` — full decision log.
- `docs/test_plan_v0.1.5.md` — test design rationale.
- `docs/benchmarks_v0.3.0.md` — Zero-GC architecture enablers + measurement
  methodology.

### Quality Gates

- **445 tests Green** (440 EditMode + 5 MiniUnity), 0 Red, 0 Skipped.
- 0 Build Warnings, 0 Build Errors (Debug + Release).
- Release configuration parity verified.
- No `UnityEngine` reference in `Animo.Core` / `Animo.Model` / `Animo.Tools`.

### Refinement Rounds (Gemini Adversarial Review)

Across 4 review rounds, 28 critiques evaluated:
- 22 adopted (real bugs / quality issues)
- 5 rejected as hallucinations (verified non-existent in current code)
- 1 deferred to Phase 4 spec extension (Affect-then-GetNeed semantics)

---

## [v0.2.0-alpha] — Phase 2 (Red Baseline + Spec Stabilization)

### Added

- Full spec v0.1.5 with 151 Q-S patches stabilizing every ambiguity discovered
  through 32 rounds of adversarial review.
- Red baseline test suite (427 tests, all failing — implementations were stubs).
- `MiniUnity.SelfTests` (5 tests) for Unity-less MockScene infrastructure.

## [v0.1.0] — Phase 1 (Spec Definition)

### Added

- Initial spec definition.
- `animo.schema.json` and three example personas (`tanukichi`, `goblin_scout`,
  `shiori`).

---

[v0.3.0-alpha]: https://github.com/<owner>/animo/releases/tag/v0.3.0-alpha
[v0.2.0-alpha]: https://github.com/<owner>/animo/releases/tag/v0.2.0-alpha
[v0.1.0]:       https://github.com/<owner>/animo/releases/tag/v0.1.0
