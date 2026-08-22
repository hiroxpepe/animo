# TASKLIST

Work items still open for this repository. Any person may put in a new
item; the person who does the work marks it done (`+ [x]`) and puts the
change in as a commit.

<!-- format: v1 | fields: status, id, title, phase -->

+ [ ] TASK-001 [P-XX]: Put the rest of the docs into Basic English
+ [ ] TASK-002 [P-XX]: Let a persona give a display name for a need or action
+ [ ] TASK-003 [P-XX]: Add a MockBus to check a Lock-time Bus.Publish direct
+ [ ] TASK-004 [P-04]: Move the true engine to netstandard2.1, for real Unity use
+ [xx] TASK-005 [P-04]: Write a spec for the adapter layer — moved out, to modio
+ [xx] TASK-006 [P-04]: Put that spec through a G review — moved out, to modio
+ [x] TASK-007 [P-04]: Decide the public shape — settled, it is a property
+ [xx] TASK-008 [P-04]: Build the adapter, by TDD — moved out, to modio
+ [x] TASK-009 [P-05]: Give the goblin_scout persona a true threat action (dropped, see detail)
+ [xx] TASK-010 [P-05]: Check a Germio rule can call it — moved out, to germio
+ [xx] TASK-011 [P-05]: Wire one agent into stemic — moved out, to modio
+ [ ] TASK-012 [P-XX]: Weigh a 100-agent, one-hour real Unity soak test
+ [ ] TASK-013 [P-XX]: Write the kind/persona pair for stemic's own two characters
+ [ ] TASK-014 [P-XX]: Restore an Engine from a Snapshot, Needs and all
+ [ ] TASK-015 [P-XX]: Restore a lock, so a held Behavior stays held
+ [ ] TASK-016 [P-XX]: Turn down a Snapshot whose Needs do not match
+ [ ] TASK-017 [P-XX]: Make no garbage on a Restore, the same bar as Live

## Detail

### TASK-001

`CLAUDE.md`, `TASKLIST.md`, `HANDOFF.md`, `writing_standard.md`,
`coding_standard.md`, `tech_terms.md`, and `docs/live_monitor_spec.md`
are all in Basic English now. The rest of the docs are not:
`README.md` and every other file under `docs/` still fail the check.
This holds true for `docs/animo_roadmap.md` too — its own state was
brought up to date tonight, but its words were not put into Basic
English.

**A small, still-open thing inside `README.md` by itself**: two
spots (the "Roadmap to v1.0.0" link near the top, and a mermaid box
inside the TDD part) still point at the old file names
`docs/animo_roadmap_to_v1.0.0.md` and `docs/test_plan_v0.1.4.md`,
neither of which holds that content any more. Fixing either spot
stages the whole file for the same check that blocks a commit on
its own many words not yet in Basic English. Bring the whole file
into Basic English first, then fix both spots as part of that same
pass.

Also still open: words put into `draft_words.md` in a hurry, to get
`coding_standard.md` and `tech_terms.md` to pass. Some of these are real
technical words that should move to `tech_terms.md`, each with its own
short sense, and not sit in `draft_words.md` with no sense given at all.
This move needs the master's own GO first, word by word.

### TASK-002

The Live Monitor's own dashboard chrome (its headings, its buttons)
switches between English and Japanese from a small, built-in word
set. The data itself does not: a need name such as `hunger` and an
action name such as `Socialize` come straight from a persona file's
own field names, with no display form given anywhere. So a need or
action name stays in English even when the dashboard is set to
Japanese.

A way to close it: let a persona file carry a display name for each
need and each action, in each language it wants to support (say,
`display_name: { en: "...", jp: "..." }` next to each need/action
entry). The monitor would read this at load time and fall back to
the raw key when a display name is missing, so an older persona file
with none still works with no change. This needs a small schema
addition on the persona file's own shape, and a short read path in
the monitor. See `docs/live_monitor_spec.md` for the full picture.

### TASK-003

During a Lock, `Step3_ThresholdEffectiveNeedsTests.cs` and
`LockEdgeCaseTests.cs` check that a need's own value still moves (an
indirect sign that the engine's inner steps are still running), but
neither checks straight that `Bus.Publish` truly fires a threshold
event during that Lock. A direct check needs a `MockBus` put into
the `Engine` at test time, which was filed during Phase 2 as a Phase
3 follow-up and is still open now that Phase 3 is done. Add the
`MockBus` injection point, then add the direct assertion these two
test files already point to in their own comments.

### TASK-004

**Checked true, 2026-08-18 (Master's own word).** `Scripts/Animo.csproj`
holds at `net8.0` today. Setting `TargetFramework` to `netstandard2.1`
alone gave 26 build errors — every one `CS8400` (a C# 8.0 language-
version ceiling; `netstandard2.1` itself sets no language-version
floor). Adding one line, `<LangVersion>12</LangVersion>` (the same
setting `Quyno`/`Signo`'s own `Core` .csproj already holds), gave a
true, clean build — 0 errors, 1 warning. Bring this real, checked
change into the true `Animo.csproj` (reverted after the check, not
yet landed).

### TASK-005

**Moved out 2026-08-21, to `modio`.** This asked for a spec of the
adapter layer, before any code.

That spec was written, and grew past what a small adapter could be. It
needs seeking, a past of its own, and a way to carry one deed out over
time. **It is now `modio`, a build of its own**, and its spec is
`modio`'s own `docs/modio_spec.md`.

`animo` is owed nothing here. Its own public shape already serves —
see TASK-007.

### TASK-006

**Moved out 2026-08-21, to `modio`.** The G review of that spec goes
with the spec (`modio`'s own TASK-005 there).

### TASK-007

**Settled 2026-08-21: a property.**

`Engine.Behavior` gives back what was last picked, and `modio` reads
it once a tick. An event was weighed and let go: `modio` already looks
at the world every tick to seek (`modio`'s own `docs/modio_spec.md`
§3), so it is there to read a property anyway. **An event would add
work to join it up, and take it apart again, for nothing.**

`modio` fires a signal only where the Behavior has **changed**
(§7.10 there), so reading every tick costs nothing further on.

### TASK-008

**Moved out 2026-08-21, to `modio`.** Building it is `modio`'s own
work, by TDD, from the spec TASK-005 speaks of.

### TASK-009

**Dropped, 2026-08-19, in a later plan talk with Master.** A real,
headless run of `examples/goblin_scout.json` through `ScenarioRunner`
(a true fear spike at t=5s, 40s total) showed `Socialize` → `Flee`
(13s straight) → `SearchFood`, switching away from `Flee` while fear
was still at 72 (still high). This persona holds no true threat/
attack action at all — only `Flee`, `SearchFood`, `Rest`, `Patrol`,
`Socialize`. A first plan (this same day) held this would call for
an `Attack`-class need/action pair, since `stemic`'s own new
character was, at that point, meant as one enemy NPC (the true
reference to `stemic`'s own TASKLIST TASK-005/006 above was itself
stale even then — `stemic`'s own true numbers for that work had
moved to TASK-015/016 by that point).

A later plan talk (this same day) dropped the "enemy" framing for
`stemic`'s own new work for good, in place of a true pair of
characters with no fixed side at all. `goblin_scout` itself is left
untouched (a true, standing example persona, not tied to any one
game); TASK-013 below holds the true kind/persona work `stemic`'s
own P-04 now calls for instead.

### TASK-010

**Moved out 2026-08-21, to `germio`.** A `germio` rule reaching this
engine is now `germio`'s own TASK-018 there: a `update_need` command,
firing an event out of the `Store`, which `modio` hears and turns into
a call to `Affect`.

`animo` gains nothing and loses nothing: `Affect(need, delta)` was
always its one way in, and still is.

### TASK-011

**Moved out 2026-08-21, to `modio`** (its own TASK-015 there).

This asked for one `Agent` to be wired into `stemic` and checked by
real play. Two things moved under it since:

+ **The joining up is `modio`'s own work**, not this engine's. See
  P-04 in `ROADMAP.md`.
+ **The PoC is no longer one agent with the `goblin_scout` persona.**
  It is two, `place_curious` and `company_seeking`, built as a pair so
  that Maslow's own holding-back shows as the bond between them
  (`docs/persona_design_spec.md` §6).

**What this engine still owes is TASK-013** — writing those two out as
a real `animo.json` file.

### TASK-012

Not owed at all, but worth a true weigh once P-04/P-05 both close:
a 100-agent, one-hour real Unity soak test (too great a real ask
for this true phase, and not this engine's own true worth right
now — held here in case a later, real need calls for it).

### TASK-013

**The one thing this engine still owes the PoC.**

`docs/persona_design_spec.md` §6 holds both personas worked out in
full — every Stage, Need, starting value, rate, exponent, Action and
suppression, plus the influence and commitment bonus for each, all
checked by real sums. **Nothing is left to decide. What is left is to
write it out** as a real `animo.json` file, and see it read.

**Two builds wait on this**: `modio`'s own TASK-015 (joining Modio to
`stemic`) and `stemic`'s own TASK-021, neither of which can be checked
by real play until these two personas run.

The older sketch of the pair, in `docs/adapter_spec.md`, is a sketch
only; §6 of the persona spec is the real thing.

### TASK-014

`Engine.Snapshot()` reads a whole state out — every base Need, every
effective Need, the Behavior, the lock, every action score.
**Nothing reads one back in.**

So an Engine can be watched, and never picked up again where it was
left. A run cannot be carried on from the middle; a game cannot be
saved and taken up later; a character cannot walk from one level into
the next and still feel what it felt.

**This is a hole in this engine itself**, not a want of some other
build. `Snapshot()` without a way back is half a thing.

Add `Restore(EngineSnapshot)`, its own other half.

**Test first:** take a Snapshot with `hunger` at 47.3, put every Need
somewhere else, Restore, and `GetNeed("hunger")` gives back 47.3.

### TASK-015

A Snapshot holds `is_locked` and `locked_behavior`. A Restore must put
those back too, or a state picked up again would let go of a hold it
was under.

**Test first:** Snapshot while locked, Restore, and `IsLocked` is
true with `LockedBehavior` unchanged.

### TASK-016

A Snapshot taken off one persona, put into an Engine built on another,
names Needs that Engine does not hold.

**Test first:** a Restore with an unknown Need throws, and leaves the
Engine as it stood. **Half a Restore does more harm than none at all.**

### TASK-017

`Live()` makes no garbage on the hot path, proven over 100,000 runs.
A Restore is not on the hot path, but it must not undo that work by
holding onto what it was given.

**Test first:** a Restore run many times over makes no garbage at all.
