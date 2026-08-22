# ROADMAP

<!-- format: v1 | fields: status, phase, title -->

+ [x] P-01: Fix the spec and build the schema and test base
+ [x] P-02: Build the core engine, with zero-GC proved
+ [x] P-03: Build the Unity layer and a stand-alone tool
+ [ ] P-04: Build a true, public interface Germio can call
+ [ ] P-05: Prove a real Germio game can call this engine (a PoC)
+ [~] P-XX: Work that does not fit the phases above

## Detail

### P-01

Phase 1 (fixing the spec) and Phase 2 (the schema plus a full,
pure-C# test base) of the older, detailed plan (in
`animo_roadmap.md`) are done. Their own `Status` lines both read
"Complete" in that file.

### P-02

Phase 3 of the older plan: the real engine built, with a test that
proves it makes zero garbage on a hot path (`Live(delta_time)`, run
100,000 times). Done; its own `Status` line reads "Complete."

### P-03

Phase 4 of the older plan: `Agent.cs`, `Store.cs`, `AnimoLog.cs`,
`ScenarioRunner.cs`, and `TraceResult.cs` were all found built and
tested in the real code tonight, closing out this phase.

### P-04

**Master's own word, 2026-08-18.** A real check found `Agent.cs`'s
own `using Germio;` line is not truly used at all — `_animator` is
Unity's own plain `Animator`, and no true call reaches any real
`Germio` type or member anywhere in the file. **This engine's own
whole state is clean, and stays as it is** — but no true, public way
existed then for a `Germio`-based game to read this engine's own
behavior choice at all.

**Settled 2026-08-21: the layer that reads it is `modio`.** What was
first thought a small adapter, sitting beside a game's own code, grew
under hard questioning into three powers — seeking, a past of its own,
and carrying one deed out over time. It is now a build of its own, the HOW layer, between `animo` (WHY) and `germio` (WHAT).

**Nothing more is owed by `animo` here.** Its own public shape already
serves: `Engine.Behavior` gives back what was picked, `Affect(need,
delta)` takes back what landed, and `Lock(duration, LockMode.Soft)`
holds a Behavior steady while a deed plays out. `modio` reads all
three, and `animo` knows nothing of it — no place, no thing, no other
agent, just as §2 of `docs/animo_spec.md` sets out.

See `TASKLIST.md` for what stands and what was let go.

### P-05

**Master's own word, 2026-08-18.** Not a 100-agent, one-hour real
Unity soak test (too great a real ask, and not this engine's own
true worth) — a plain, small PoC instead: one true agent, inside a
real `Germio` game (`stemic`, first).

**Settled 2026-08-21: the joining up is `modio`'s own work**, and is
tracked there (its own TASK-015). P-04 above sets out why: what was
first thought a small adapter is now a build of its own, the HOW
layer.

**What `animo` still owes this phase is the personas themselves** —
the kind/persona pair for `stemic`'s own two characters, worked out in
full in `docs/persona_design_spec.md` §6 and not yet written as a real
`animo.json` file. See `TASKLIST.md` TASK-013.

### P-XX

Work that does not fit any of the phases above (putting the
project's own docs into Basic English, or a fix found by real,
day-to-day use that the older plan never named) is tracked here
instead. See `TASKLIST.md` for the open work under this phase.
