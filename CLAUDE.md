# CLAUDE

> How the agent works in this repository. These are rules for the agent
> (a language model) that helps build this project. They are kept short and
> plain, by the writing standard.

## Set up once, before any other work

+ Run `git config core.hooksPath .githooks` once for each clone. This
  makes `git commit` run the shared checks in `.githooks/pre-commit`
  and `.githooks/commit-msg` on every commit, before the commit is
  let through. **Without this one command, neither hook runs at all
  — a broken commit message, or a markdown file still holding an
  error, would pass straight through, with nothing to stop it.**
  Check it is truly set, with `git config core.hooksPath`, which must
  answer `.githooks`. Running a hook by hand, on a file, is **not**
  proof the hook itself is live; only a true `git commit` proves that.

## Work record

+ When the master asks you to write the work record, in any words, that one request is the whole order. Do the steps below, and nothing more.
+ Keep one work record file for one topic. Never put two topics in one file.
+ The steps and the manners are in the documents repository (`hiroxpepe/documents`). Read the section on the personal work record in its `CLAUDE.md` first. It names the procedure file. That file is the one true source. This section only points to it.
+ This repository keeps its work records in the documents repository, in the folder `05_X_*/12_Animo_*/X_*/2026/` (use the pattern to find it). The file name is `X_MM-DD_N.md`.
+ Write the record in Japanese. The hours are always in the table, never empty. If the master gives no hours, work them out from the commit times, and write the basis line under the table.
+ Use the tools in the documents repository, in the folder `05_X_*/*/Claude_Tools/app/`. `worklog_new_cli.js` makes the file. `worklog_hours_cli.js` works out the hours. `task_ops_cli.js` adds the row to the task sheet. `repo_sync_cli.js` pulls and pushes.
+ Pull the documents repository before you write. Push it after. The commit message there is `update`.
+ Never show a token. Never put a secret, such as a key, in a record.
+ When it is done, tell the master what you wrote, as a short list.

## Three files, three jobs

+ `CLAUDE.md` (this file) — the rules and the word given: how the
  agent works here, checked every time, not tied to any one act of
  work.
+ `TASKLIST.md` — the full list of open work, with a plan for when.
  A short checkbox line up top for each item, a full write-up below
  it.
+ `HANDOFF.md` — the hand-off to the next chat: where things stand
  right now, and the next move. Kept short; the full list lives in
  `TASKLIST.md`.

## Documents

+ Every document follows the writing standard in `docs/standard/`.
  The words are kept simple, so a reader whose first language is not
  English can take in the sense. See
  `docs/standard/writing_standard.md`.
+ Every hard word used in a document must be in the word list first.
  If a word is not there, add it to `docs/standard/tech_terms.md`
  before you use it.
+ Badges follow `docs/standard/badge_convention.md`.

## Markdown check

+ Before you commit any markdown file, run the check and get no
  errors at all. Do not commit a markdown file that still has errors.
+ The rules are set in `.markdownlint.json` at the root of the
  repository. Use that file, not your own idea of the rules.
+ The list mark is the plus sign. Use `+` for every list line, not
  `-` or `*`.

Run the check like this:

```bash
npx --yes markdownlint-cli -c .markdownlint.json <file>
```

## Commits

+ The commit message is one line, with no body under it.
+ The form is `type: Verb subject`. The verb is one of Add, Update,
  or Delete. The type is one of `feat`, `fix`, `refactor`, `docs`,
  `chore`, or `test`.
+ Keep the first line between 57 and 61 letters long.
+ Do not put square marks or forward lines in the message; keep it
  plain.

## History

+ Keep the history one straight line. Do not make a commit that
  joins two lines back into one.
+ If a push is turned down because the copy on the server is ahead,
  put your work back on top of it first, then push. Do not join the
  two lines back into one.
