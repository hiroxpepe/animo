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
exists yet for a `Germio`-based game to read this engine's own
behavior choice at all.

Sound (through `Signo`/`Quyno`) holds a true, shared, one-size
shape across every game, so `Germio` itself holds that shared
layer (`SoundSystem.cs`). A persona's own mind is the true
opposite — each given game's own true, different content — so
`Animo` itself must hold its own true, public interface (a public
property, or an event fired on a true behavior change), for
`Germio`'s own Command/Rule layer to read or hear. See
`TASKLIST.md` for the open work under this phase.

### P-05

**Master's own word, 2026-08-18.** Not a 100-agent, one-hour real
Unity soak test (too great a real ask, and not this engine's own
true worth) — a plain, small PoC instead: one true agent, inside a
real `Germio` game (`stemic`, first), read through the true
interface P-04 built. Depends on P-04 landing first. See
`TASKLIST.md` for the open work under this phase.

### P-XX

Work that does not fit any of the phases above (putting the
project's own docs into Basic English, or a fix found by real,
day-to-day use that the older plan never named) is tracked here
instead. See `TASKLIST.md` for the open work under this phase.
