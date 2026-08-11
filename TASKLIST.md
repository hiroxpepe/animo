# TASKLIST

Work items still open for this repository. Any person may put in a new
item; the person who does the work marks it done (`+ [x]`) and puts the
change in as a commit.

<!-- format: v1 | fields: status, id, title -->

+ [ ] TASK-001: Put the rest of the docs into Basic English
+ [ ] TASK-002: Let a persona give a display name for a need or action
+ [ ] TASK-003: Add a MockBus to check a Lock-time Bus.Publish direct

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
