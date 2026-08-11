# ROADMAP

<!-- format: v1 | fields: status, phase, title -->

+ [x] PHASE-01: Fix the spec and build the schema and test base
+ [x] PHASE-02: Build the core engine, with zero-GC proved
+ [x] PHASE-03: Build the Unity layer and a stand-alone tool
+ [~] PHASE-04: Prove the engine holds up at real scale in Unity
+ [~] PHASE-05: Work that does not fit the first four phases

## Detail

### PHASE-01

Phase 1 (fixing the spec) and Phase 2 (the schema plus a full,
pure-C# test base) of the older, detailed plan (in
`animo_roadmap.md`) are done. Their own `Status` lines both read
"Complete" in that file.

### PHASE-02

Phase 3 of the older plan: the real engine built, with a test that
proves it makes zero garbage on a hot path (`Live(delta_time)`, run
100,000 times). Done; its own `Status` line reads "Complete."

### PHASE-03

Phase 4 of the older plan: `Agent.cs`, `Store.cs`, `AnimoLog.cs`,
`ScenarioRunner.cs`, and `TraceResult.cs` were all found built and
tested in the real code tonight, closing out this phase.

### PHASE-04

Phase 5 of the older plan: 100 agents at 60 fps, stable, in an
empty scene, with a one-hour run showing no leak. This needs a real
Unity run to check; no sandbox can confirm FPS or a long soak run
on its own. Phases 6, 7, and 8 of the older plan (joining with
`germio` and `briko`, writing up the docs, and the v1.0.0 release
itself) all still wait on this one. See `TASKLIST.md` for the open
work under this phase.

### PHASE-05

Work that does not fit any of the four phases above (putting the
project's own docs into Basic English, or a fix found by real,
day-to-day use that the older plan never named) is tracked here
instead. See `TASKLIST.md` for the open work under this phase.
