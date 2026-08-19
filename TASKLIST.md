# TASKLIST

Work items still open for this repository. Any person may put in a new
item; the person who does the work marks it done (`+ [x]`) and puts the
change in as a commit.

<!-- format: v1 | fields: status, id, title, phase -->

+ [ ] TASK-001 [P-XX]: Put the rest of the docs into Basic English
+ [ ] TASK-002 [P-XX]: Let a persona give a display name for a need or action
+ [ ] TASK-003 [P-XX]: Add a MockBus to check a Lock-time Bus.Publish direct
+ [ ] TASK-004 [P-04]: Move the true engine to netstandard2.1, for real Unity use
+ [ ] TASK-005 [P-04]: Write a true spec for the adapter layer itself
+ [ ] TASK-006 [P-04]: Put the adapter spec through a true G review
+ [ ] TASK-007 [P-04]: Decide the public shape (a property, or an event)
+ [ ] TASK-008 [P-04]: Build the adapter, by TDD, from the checked spec
+ [x] TASK-009 [P-05]: Give the goblin_scout persona a true threat action (dropped, see detail)
+ [ ] TASK-010 [P-05]: Check a Germio Command/Rule can call the new adapter
+ [ ] TASK-011 [P-05]: Wire one agent into stemic, and check it by real play
+ [ ] TASK-012 [P-XX]: Weigh a 100-agent, one-hour real Unity soak test
+ [ ] TASK-013 [P-XX]: Write the kind/persona pair for stemic's own two characters

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

**Master's own word, 2026-08-18.** A real check found `Agent.cs`'s
own `using Germio;` line is not truly used at all — no true call
reaches any real `Germio` type or member anywhere in the file.
Write a true spec for this engine's own adapter layer, before any
code: what this engine passes out (the `Behavior` string alone, or
need values too), who starts it up, and how often a real `Germio`
game is meant to read or hear it, each true frame.

### TASK-006

Put the true spec TASK-005 wrote through a real, hard-questioning `G`
review, the same way every other true spec in this repository
already stands on one.

### TASK-007

Given TASK-006's own true review, decide the adapter's own public
shape: a plain, read-any-time property, or an `event` fired only on
a true behavior change. `Animo` holds this true interface itself,
since a persona's own mind is each given game's own true, different
content, unlike sound (a true, shared, one-size shape `Germio`
itself holds through `SoundSystem.cs`).

### TASK-008

Build the true adapter, by TDD, from the checked spec (TASK-005)
and the true shape TASK-007 picked.

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

Check, with a plain, given test `Rule`, that a real `Germio`
Command/Rule can truly read the adapter TASK-008 built — a small,
given check, ahead of any full `stemic`-side wiring at all.

### TASK-011

Wire one true `Agent` (given the `goblin_scout` persona, given
TASK-009 lands first) into a real `germio` game (`stemic`, first),
read through the true adapter, and check it by real play — the
true, final check that closes this whole phase. A plain, small
PoC — one agent, no true scale or timing ask at all.

### TASK-012

Not owed at all, but worth a true weigh once P-04/P-05 both close:
a 100-agent, one-hour real Unity soak test (too great a real ask
for this true phase, and not this engine's own true worth right
now — held here in case a later, real need calls for it).

### TASK-013

**Checked true, 2026-08-19, in a plan talk with Master.** A true
re-shape of the Adapter (bridge) layer itself, over a wish to give
`stemic` a true pair of characters. The full true design (why a
target may still be held, who holds it, and the kind/persona pair
itself) sits in `docs/adapter_spec.md` — this task closes once
that spec is checked true and the kind/persona JSON it calls for is
written.
