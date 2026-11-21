# HANDOFF

> New chat starts here. Read this file first, before anything else.

## Read this first, in every new chat

**Read `CLAUDE.md`, in this repository, before any other work.** It
holds the one command that must be run once, for each clone
(`git config core.hooksPath .githooks`), and every rule the agent
holds to, here. **Every other repository in this family holds its own
`CLAUDE.md` too — read that one, in that repository, before any work
begins there.**

## Where things stand

State on 2026-11-07, checked against the code.

| # | Part | State |
| --- | --- | --- |
| 1 | Phases | `PHASE-01` to `PHASE-04` are done; `PHASE-05` is in progress: the joining up to a real game is `modio`'s `TASK-015`, and what `animo` still owes are the persona tasks |
| 2 | The engine | `Scripts/Animo.csproj` is `netstandard2.1` with `LangVersion` 12 and builds clean (`TASK-004`, done) |
| 3 | The tests | 605 in `Tests~/EditModeTests` and 5 in `Tests~/MiniUnity.SelfTests`, all pass |

## Next move

Sixteen tasks are open, all in `PHASE-XX` (see `TASKLIST.md`; it sets no order): the docs into Basic English (`TASK-001` and `TASK-022` to `TASK-026`, one part to a task), the personas (`TASK-002`, `019`,
`020`, `021`), the restore of an Engine from a Snapshot (`TASK-014` to `017`, one group; `TASK-014` and `TASK-015` wait for the word of the master, see the notes), and a MockBus and a long soak test (`TASK-003`, `012`).
