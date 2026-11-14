# TASKLIST

Work items still open for this repository. Any person may put in a new
item; the person who does the work marks it done (`+ [x]`) and puts the
change in as a commit.

<!-- format: v1 | fields: status, id, title, phase -->

+ [ ] TASK-001 [P-XX]: Put the first part of the old decisions paper into Basic English
+ [ ] TASK-002 [P-XX]: Let a persona give a display name for a need or action
+ [ ] TASK-003 [P-XX]: Add a MockBus to check a Lock-time Bus.Publish direct
+ [x] TASK-004 [P-04]: Move the true engine to netstandard2.1, for real Unity use
+ [xx] TASK-005 [P-04]: Write a spec for the adapter layer — moved out, to modio
+ [xx] TASK-006 [P-04]: Put that spec through a G review — moved out, to modio
+ [x] TASK-007 [P-04]: Decide the public shape — settled, it is a property
+ [xx] TASK-008 [P-04]: Build the adapter, by TDD — moved out, to modio
+ [x] TASK-009 [P-05]: Give the goblin_scout persona a true threat action (dropped, see detail)
+ [xx] TASK-010 [P-05]: Check a Germio rule can call it — moved out, to germio
+ [xx] TASK-011 [P-05]: Wire one agent into stemic — moved out, to modio
+ [ ] TASK-012 [P-XX]: Weigh a 100-agent, one-hour real Unity soak test
+ [x] TASK-013 [P-XX]: Write the kind/persona pair for stemic's own two characters
+ [ ] TASK-014 [P-XX]: Restore an Engine from a Snapshot, Needs and all
+ [ ] TASK-015 [P-XX]: Restore a lock, so a held Behavior stays held
+ [ ] TASK-016 [P-XX]: Turn down a Snapshot whose Needs do not match
+ [ ] TASK-017 [P-XX]: Make no garbage on a Restore, the same bar as Live
+ [x] TASK-018 [P-XX]: Point the contract tests at where the source files truly sit
+ [ ] TASK-019 [P-XX]: Bring shiori and tanukichi up to the design spec
+ [ ] TASK-020 [P-XX]: Run goblin_scout for a whole minute, and make all five show
+ [ ] TASK-021 [P-XX]: Note that the first game's own plan asks for Needs the pair does not hold
+ [ ] TASK-022 [P-XX]: Put the second part of the old decisions paper into Basic English
+ [ ] TASK-023 [P-XX]: Put the third part of the old decisions paper into Basic English
+ [ ] TASK-024 [P-XX]: Put the last part of the old decisions paper into Basic English
+ [ ] TASK-025 [P-XX]: Put the long roadmap into Basic English
+ [ ] TASK-026 [P-XX]: Put the papers on the state, the changes, the measures, and the monitor into Basic English

## Detail

### TASK-001

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/decisions/v0.1.5_ambiguity_resolution.md` has 914 words not allowed by the Basic English check in 6,371 lines (counted 2026-11-14); one task takes lines 1 to 1600 of it, as of 2026-11-14. The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: no line from 1 to 1600 is named by `tools/basic_english_cli.js` on that file.

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

**Done 2026-09-22 (commit `8646a82`), found and checked 2026-11-07.** `Scripts/Animo.csproj` holds `<TargetFramework>netstandard2.1</TargetFramework>` and
`<LangVersion>12</LangVersion>` (the same setting `Quyno`/`Signo`'s own `Core` .csproj holds). `dotnet build Scripts/Animo.csproj` builds with 0 errors and 0 warnings.
The commit also put six lines into `Scripts/Agent.cs`, the version `0.1.5` into `package.json` (it was `0.1.5-design`), and the author name `STUDIO MeowToon`.
Every test passes: 605 in `Tests~/EditModeTests` and 5 in `Tests~/MiniUnity.SelfTests`. The task of `stemic` that waits on this (its `TASK-015`) can move.

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

**Done 2026-08-22**: `examples/poc_pair.json`, with 9 tests in
`Tests~/EditModeTests/Integration/PoCPairEndToEndTests.cs`.

**Running it changed three numbers.** Every value in §6 had been
worked out by hand at t=0 alone; run for a whole minute, three things
broke that no hand-sum caught — an influence that took the pair's own
round apart, a suppression that kept 4 Stages from ever winning, and rates too slow for two Stages to reach where they could
win. All three are put right, and written up in
`docs/persona_design_spec.md` §6.

Over a minute, with `modio`'s own `Affect` calls standing in, both
show all five Stages:

    place_curious    Approach → Explore → ShowFind → GoHome → Rest
    company_seeking  Approach → Give → Tend → Approach → Call → Tend → Rest

**Two builds were waiting on this**: `modio`'s own TASK-015 and
`stemic`'s own TASK-021. Both may go ahead.

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

**Read on 2026-11-14 (`Engine.cs`).** `Live` makes the effective needs and the action scores again from the base needs each time, so the base needs are the source. `GetNeed` gives the effective value and `GetBaseNeed` gives the base value. `Snapshot` holds both, and the scores. This task does not say whether the first test reads the base value or the effective value (a Restore does not run `Live`, so after it the effective value is still the old one), or whether a restored behavior gives the signal that a behavior changed (`_previous_behavior` is how a change is seen). The master decides these before the test is written.

### TASK-015

A Snapshot holds `is_locked` and `locked_behavior`. A Restore must put
those back too, or a state picked up again would let go of a hold it
was under.

**Test first:** Snapshot while locked, Restore, and `IsLocked` is
true with `LockedBehavior` unchanged.

**Read on 2026-11-14 (`Engine.cs`).** `IsLocked` is not kept: it is true while `_lock_remaining` is more than zero. `EngineSnapshot` holds `is_locked` and `locked_behavior` only. It does not hold the time that is left, or the lock mode (Hard or Soft). So a Restore cannot say how long the lock holds, or of what kind. The master decides: put the time left and the mode into `EngineSnapshot` and give them back as they were, or give back no lock.

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

### TASK-018

Six tests fail today, and have for some time:

    Case01_DataCs_DocumentsNeedsJsonBridgeContract
    Case01_PhysicalValidator_DocstringMentionsA040
    Case02_Engine_Declares_LockedBehaviorIndex
    Case02_EngineCs_DoesNotContainNullCoalesceForResetThreshold
    Case03_Validator_DocumentsValidationResultInternalDesign
    Case04_SpecDocumentsDeepCopyContract

**Every one fails the same way**: `Assert.That(File.Exists(path))`
gives back false. They look for `Scripts/Data.cs`,
`Scripts/Engine.cs`, `Scripts/Validator.cs` — and those files moved
into `Scripts/Model/` and `Scripts/Core/` some time back. The tests
never followed.

**Done 2026-08-22.** Three files had moved — `Data.cs` into
`Scripts/Model/`, `Engine.cs` and `Validator.cs` into `Scripts/Core/`
— and six tests across six files still looked in the old place.

Every one now points where its file truly sits, and **all 605 tests
are green.**

These are contract tests: they hold a docstring to what a spec says.
**A wrong path means nothing is held at all**, and the test passes for
no reason or fails for no reason. Six of them had been failing
quietly, long enough that they read as part of the furniture.

### TASK-019

`examples/shiori.json` and `examples/tanukichi.json` hold the older
forms of those two: the ones with `idle` as a Need, and Stages left
empty (see `docs/persona_design_spec.md` §1).

**Both are wanted, and neither is to be taken away.** They are the
worked examples the spec itself leans on — a reader who wants to see
how a persona is built reads them.

What is owed is that they be brought up to §6: `idle` out, all five
Stages filled, and — as TASK-013 showed — **run for a whole minute,
not weighed at t=0 alone.**

### TASK-020

`examples/goblin_scout.json` runs, and runs well at the moment it
starts. **Over a minute it shows two behaviours out of five**:

    goblin_scout   Socialize → Rest

The same three things TASK-013 turned up are likely at work here:
an influence pulling a Need down, a suppression too heavy for the
higher Stages, and rates too slow for Stage 2 and Stage 4 to reach
where they could win.

**Run it for a minute, find which, and put them right.** §6 of the
spec holds the shape this persona should take.

### TASK-021

**Found 2026-09-19, by setting two real things side by side.** The
first game's own plan (`documents`, `X_ファーストゲーム企画.md`,
2026-09-14) builds its whole NPC on this: *when `Fear` and
`Frustration` climb past a mark, the character turns from quiet to
angry*. But `examples/poc_pair.json` (2026-08-20) holds neither.
`place_curious_01` keeps `fatigue`, `exposure`, `loneliness`,
`recognition`, `curiosity`; `company_seeking_01` keeps `fatigue`,
`separation`, `loneliness`, `usefulness`, `togetherness`. No `fear`,
no `frustration`, in either one.

**Neither is wrong.** The plan was written from `Const.cs`'s own
standard eight (`hunger`, `fatigue`, `fear`, `loneliness`,
`confidence`, `curiosity`, `idle`, `frustration`); the pair was
written a month before, for a wholly different job. §13.3 lets a
persona hold Needs outside the standard set, and the pair does.

**What was checked, and found right:** the pair holds no `needs_meta`,
so its five own Needs have no tier. This is not a thing left out.
`persona_design_spec.md` §5 asks that every persona be given a pair of
Stages picked on purpose to compete, by close `tier` and close
`exponent` — and the pair carries those tiers on its **Actions**
(`Rest` 1, `GoHome` 2, `Approach` 3, `ShowFind` 4, `Explore` 5), which
is where the design puts them. Nothing to fix.

**What this task is:** not to add `fear` and `frustration` to the two
personas. Doing that would break the competing pair each was built
around. The two are for the first check that `stemic`, `Modio` and
`animo` run together at all; an angry NPC is the step after. **When
that step comes, it asks for a third persona of its own, built around
`fear` and `frustration` from the start.** Write that down where the
plan can see it, so no one later tries to bend the pair into it.

### TASK-022

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/decisions/v0.1.5_ambiguity_resolution.md` has 914 words not allowed by the Basic English check in 6,371 lines (counted 2026-11-14); one task takes lines 1601 to 3200 of it, as of 2026-11-14. The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: no line from 1601 to 3200 is named by `tools/basic_english_cli.js` on that file.

### TASK-023

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/decisions/v0.1.5_ambiguity_resolution.md` has 914 words not allowed by the Basic English check in 6,371 lines (counted 2026-11-14); one task takes lines 3201 to 4800 of it, as of 2026-11-14. The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: no line from 3201 to 4800 is named by `tools/basic_english_cli.js` on that file.

### TASK-024

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/decisions/v0.1.5_ambiguity_resolution.md` has 914 words not allowed by the Basic English check in 6,371 lines (counted 2026-11-14); one task takes lines 4801 to 6371 of it, as of 2026-11-14. The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: no line from 4801 to 6371 is named by `tools/basic_english_cli.js` on that file.

### TASK-025

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/animo_roadmap.md` (237) do not pass the Basic English check (the number is of words not allowed, counted 2026-11-14). The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: `tools/basic_english_cli.js` gives no error on each of these files, and the commit hook takes them.

### TASK-026

Cut out of the old TASK-001 on 2026-11-14, so that each task is 8 hours or less. `docs/state_of_animo_v0.3.0.md` (58), `CHANGELOG.md` (34), `docs/benchmarks_v0.3.0.md` (12), `docs/live_monitor_spec.md` (3) do not pass the Basic English check (the number is of words not allowed, counted 2026-11-14). The rule of the master: a word that has a simple word to take its place is replaced; only a word that no simple word can take the place of goes into `draft_words.md`, and no hard word goes there. The word lists in the `vocabulary` folder are not changed by these tasks. **Done, once**: `tools/basic_english_cli.js` gives no error on each of these files, and the commit hook takes them. `README.md` has one line with a word not allowed (`Roadmap`); put it right in the same pass.
