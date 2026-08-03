# Animo Test Plan — Phase 2 Red Baseline (v0.1.5)

> **Status**: This is the **Phase 2 design document**. The Red baseline has
> since transitioned to **Phase 3 Green**: 447 EditMode tests + 5 MiniUnity
> self-tests = 452 Green, 0 Red, 0 Skipped (Debug + Release) as of
> v0.3.0. The numerical totals below describe the original Phase 2
> Red baseline at planning time, not the current state. See
> [`state_of_animo_v0.3.0.md`](state_of_animo_v0.3.0.md) and
> [`benchmarks_v0.3.0.md`](benchmarks_v0.3.0.md) for the current state.

This document is the decision table specification for Phase 2 Tasks 2-3
and 2-4. Every row of every table maps to one `[Test]` method in
`Tests~/EditModeTests/`.

**Totals (v0.1.5 = Phase_2_4_1):**
- 36 Validator test files (A000–A033, A020 split into a/b/c). v0.1.5 added A033.
- 4 Composer test files
- 11 Engine test files (5 step files + 6 feature files: Maslow, Commitment,
  Lock, ForceReset, AffectEdgeCase, LockEdgeCase, GetNeed — added in v0.1.5)
- 4 EdgeCases test files
- **~205 test methods** (v0.1.4 baseline 183 + v0.1.5 deltas)

All tests are Red until Phase 3 implements the production classes. The 4
MiniUnity self-tests are the only Green tests in this phase.

---

## Validator Decision Tables (A000–A032)

### A000 — schema_version exists and is not empty
| # | Input | Expected |
|---|---|---|
| 01 | missing schema_version key | Error A000 |
| 02 | empty string schema_version | Error A000 |
| 03 | valid schema_version `"1.4"` | Pass |

### A001 — personas exists and is not empty
| # | Input | Expected |
|---|---|---|
| 01 | missing personas key | Error A001 |
| 02 | personas is `[]` | Error A001 |
| 03 | one persona present | Pass |

### A002 — agent_id snake_case, not empty, unique, ≤128
| # | Input | Expected |
|---|---|---|
| 01 | empty | Error A002 |
| 02 | PascalCase `GoblinScout` | Error A002 |
| 03 | double underscore `goblin__scout` | Error A002 |
| 04 | trailing underscore `goblin_` | Error A002 |
| 05 | digit first `1goblin` | Error A002 |
| 06 | 129 chars | Error A002 |
| 07 | duplicate ids | Error A002 |
| 08 | valid `agent_a` | Pass |

### A003 — kind_id snake_case, not empty, unique, ≤128
| # | Input | Expected |
|---|---|---|
| 01 | empty | Error A003 |
| 02 | hyphen `gob-lin` | Error A003 |
| 03 | duplicate kind_id | Error A003 |
| 04 | valid kind_id | Pass |

### A004 — persona.kind_ids reference exists in kinds
| # | Input | Expected |
|---|---|---|
| 01 | undefined kind_id `"ghost"` | Error A004 |
| 02 | defined kind_id | Pass |
| 03 | partially undefined `["goblin","ghost"]` | Error A004 |

### A005 — needs values in [0, 100]
| # | Input | Expected |
|---|---|---|
| 01 | need = 150 | Error A005 |
| 02 | need = -1 | Error A005 |
| 03 | need = 100 (boundary) | Pass |
| 04 | need = 0 (boundary) | Pass |

### A006 — suppression keys tier2..tier5, values [0, 1]
| # | Input | Expected |
|---|---|---|
| 01 | tier2 = 1.5 | Error A006 |
| 02 | tier2 = -0.1 | Error A006 |
| 03 | all tiers in range | Pass |

### A007 — actions[].tier in [1, 5]
| # | Input | Expected |
|---|---|---|
| 01 | tier 0 | Error A007 |
| 02 | tier 6 | Error A007 |
| 03 | tier 5 | Pass |
| 04 | tier 1 | Pass |

### A008 — actions[].exponent in [0.1, 5.0]
| # | Input | Expected |
|---|---|---|
| 01 | exponent 0.05 | Error A008 |
| 02 | exponent 5.1 | Error A008 |
| 03 | exponent 5.0 | Pass |

### A009 — actions[].id not empty
| # | Input | Expected |
|---|---|---|
| 01 | id = "" | Error A009 |
| 02 | id = "Idle" | Pass |

### A010 — thresholds[].trigger_threshold in [0, 100]
| # | Input | Expected |
|---|---|---|
| 01 | trigger 200 | Error A010 |
| 02 | trigger -10 | Error A010 |
| 03 | trigger 100, reset 90 | Pass |

### A011 — persona without kind_ids must have actions
| # | Input | Expected |
|---|---|---|
| 01 | no kind_ids, no actions | Error A011 |
| 02 | with kind_ids, no own actions | Pass |
| 03 | no kind_ids, with own actions | Pass |

### A012 — influences[].coefficient in [-1, 1]
| # | Input | Expected |
|---|---|---|
| 01 | coefficient 2 | Error A012 |
| 02 | coefficient -2 | Error A012 |
| 03 | coefficient 1 | Pass |

### A013 — rates keys subset of needs (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | rates key not in needs | Warning A013 |
| 02 | rates key in needs | Pass |

### A014 — on_action_change placeholders only {agent_id}/{behavior}
| # | Input | Expected |
|---|---|---|
| 01 | `x_{tier}_y` | Error A014 |
| 02 | `animo_{agent_id}_{behavior}` | Pass |
| 03 | plain string no placeholders | Pass |

### A015 — thresholds[].trigger placeholders only {agent_id}
| # | Input | Expected |
|---|---|---|
| 01 | `x_{behavior}_y` | Error A015 |
| 02 | `x_{agent_id}_y` | Pass |

### A016 — binding missing (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | binding = null | Warning A016 |
| 02 | binding present | Pass |

### A017 — DEPRECATED (decay removed in v0.1.3)
| # | Input | Expected |
|---|---|---|
| 01 | rule should not be enforced | A017 not present in result |

### A018 — id length ≤ 128 (merged into A002/A003)
| # | Input | Expected |
|---|---|---|
| 01 | kind_id 129 chars | Error A018 OR A003 |

### A019 — unknown needs key looks like typo (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | `hungrr` (typo of hunger) | Warning A019 |
| 02 | `hunger` (exact) | Pass |
| 03 | `longing` (genuine custom) | Pass (no A019) |

### A020a — kind.rates key not in persona.needs (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | rates `ghost_need` with persona needs `[idle]` | Warning A020a |

### A020b — kind.influences source/target not in needs (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | influence source `phantom` not in needs | Warning A020b |

### A020c — kind.actions[].need not in needs (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | action need `phantom` not in needs | Warning A020c |

### A021 — schema_version must be 1.3 or 1.4
| # | Input | Expected |
|---|---|---|
| 01 | "1.0" | Error A021 |
| 02 | "1.3" | Pass |
| 03 | "1.4" | Pass |
| 04 | "1.5" (future) | Error A021 |

### A022 — actions[].need is required
| # | Input | Expected |
|---|---|---|
| 01 | action need = "" | Error A022 |
| 02 | action need present | Pass |

### A023 — trigger_threshold > reset_threshold
| # | Input | Expected |
|---|---|---|
| 01 | trigger 70, reset 70 | Error A023 |
| 02 | trigger 60, reset 70 | Error A023 |
| 03 | trigger 80, reset 70 | Pass |

### A024 — idle action should be tier 5 (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | idle at tier 1 | Warning A024 |
| 02 | idle at tier 5 | Pass |

### A025 — cycle in influences (Error since v0.1.2)
| # | Input | Expected |
|---|---|---|
| 01 | no influences | Pass |
| 02 | one-way only `a→b` | Pass |
| 03 | direct cycle `a→b, b→a` | Error A025 |
| 04 | triangle `a→b→c→a` | Error A025 |
| 05 | self reference `a→a` | Error A025 |
| 06 | multiple cycles `a→b→a, c→d→c` | Error A025 |
| 07 | independent DAGs `a→b, c→d` | Pass |

### A026 — info: formula keeps commitment_bonus inside suppression
| # | Input | Expected |
|---|---|---|
| 01 | informational rule, severity ≠ Error | Pass |

### A027 — info: clamp after each edge
| # | Input | Expected |
|---|---|---|
| 01 | informational rule | Pass |

### A028 — commitment.bonus > 30 (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | bonus 20 | Pass |
| 02 | bonus 30 (boundary) | Pass |
| 03 | bonus 50 | Warning A028 |

### A029 — commitment omitted but actions ≥ 2 (Warning)
| # | Input | Expected |
|---|---|---|
| 01 | omitted with 2 actions | Warning A029 |
| 02 | omitted with 1 action | Pass |

### A030 — frustration unused (Warning, v0.1.4)
| # | Input | Expected |
|---|---|---|
| 01 | no actions/influences use frustration | Warning A030 |
| 02 | frustration used in actions | Pass |

### A031 — Lock(duration) > 30s (Warning, runtime)
| # | Input | Expected |
|---|---|---|
| 01 | rule registered; static-time severity ≠ Error | Pass |

### A032 — info: hint about a low-tier fallback action
| # | Input | Expected |
|---|---|---|
| 01 | informational rule | Pass |

---

## Composer Decision Tables

### DeepCopy (spec §10.2)
| # | Input | Expected |
|---|---|---|
| 01 | composed persona vs input | different reference |
| 02 | needs dictionary | different reference |
| 03 | actions list | different reference |
| 04 | action items | different references |
| 05 | two personas from same kind | independent |
| 06 | mutating composed action | does not affect kind |

### KindCascade (spec §8.3)
| # | Input | Expected |
|---|---|---|
| 01 | persona without kind_ids | keeps own fields |
| 02 | kind actions cascade | inherited |
| 03 | persona action overrides kind by id | overridden |
| 04 | persona new action id | appended |
| 05 | persona commitment overrides kind | overridden |
| 06 | persona rates merge per key | merged |
| 07 | persona suppression overrides kind | overridden |
| 08 | persona influence by source+target | overridden |

### MissingNeedFill (spec §8.8)
| # | Input | Expected |
|---|---|---|
| 01 | rates key missing from needs | filled to 0 |
| 02 | influence source missing | filled to 0 |
| 03 | action need missing | filled to 0 |
| 04 | all needs present | no extras added |

### MultiKindMerge (spec §8)
| # | Input | Expected |
|---|---|---|
| 01 | two kinds, last wins | last value |
| 02 | three kinds, persona last | persona value |
| 03 | order matters for actions by id | last value |
| 04 | distinct ids accumulate | all kept |
| 05 | kind-only attributes contribute | merged |
| 06 | empty kind_ids = persona-only | works |

### DedupKindIdsLastWins (v0.1.5, Q7 — added in Phase_2_4_2)
| # | Input | Expected |
|---|---|---|
| 01 | `["goblin","scout","goblin"]` with conflicting action exponent | last `goblin` wins (exponent from first kind, not second) |
| 02 | `["goblin","goblin"]` with single kind def | behaves like `["goblin"]` |
| 03 | `["a","b","c","a"]` with conflicting commitment bonus | `a` applied last → bonus from kind `a` wins |

---

## Engine Decision Tables (spec §9)

### Step1_NaturalDecay
| # | Input | Expected |
|---|---|---|
| 01 | positive rate | need rises |
| 02 | negative rate | need falls |
| 03 | rate 0 | no change |
| 04 | clamp upper at 100 | clamped |
| 05 | clamp lower at 0 | clamped |

### Step2_EffectiveNeeds (5 cases)
### Step3_Threshold (4 cases)
### Step4_ScoreCalc (5 cases)
### Step5_ActionSwitch (4 cases)
### MaslowSuppression (5 cases)
### Commitment (4 cases)
### Lock (7 cases)
### ForceReset (5 cases)

---

## EdgeCases (spec §4.6.3)

### NumericEdgeTests (7 cases)
NaN / +Inf / -Inf / -0 / float.MaxValue / boundary+ε / coefficient NaN

### EmptyAndNullTests (6 cases)
empty agent_id / empty actions / empty kind_ids / null needs / null binding / empty influences

### HighVolumeTests (5 cases)
0 / 1 / 1000 personas, 1000 duplicates, 100-kind cascade

### TimeEdgeTests (5 cases)
dt=0 / dt<0 / dt=NaN / dt=1e6 / many small ticks

---

## v0.1.5 Deltas (Phase_2_4_1, ambiguity resolution)

The 17 ambiguities Q1–Q17 were resolved here; see
`docs/decisions/v0.1.5_ambiguity_resolution.md` for the full rationale.
The test impact:

### Changed table — A021 (schema_version supported set)
| # | Input | Expected (v0.1.5) |
|---|---|---|
| 01 | "1.0" | Error A021 |
| 02 | "1.3" | Pass |
| 03 | "1.4" | Pass |
| **04** | **"1.5"** | **Pass (changed from Error)** |
| **05** | **"1.6" (new future case)** | **Error A021** |

### Changed table — A028 (commitment.bonus range, Q8)
| # | Input | Expected (v0.1.5) |
|---|---|---|
| 01 | bonus 20 | Pass |
| 02 | bonus 30 (boundary) | Pass |
| 03 | bonus 40 (warn zone) | Warning A028 |
| **04** | **bonus -5 (new)** | **Error A028** |
| **05** | **bonus 100 (over ceiling 50, new)** | **Error A028** |
| **06** | **bonus 50 (ceiling, new)** | **Warning A028, no Error** |
| **07** | **bonus 0 (new)** | **Pass** |

### New table — A033 (duplicate kind_ids, Q7)
| # | Input | Expected |
|---|---|---|
| 01 | `["goblin", "goblin"]` | Warning A033 |
| 02 | `["goblin", "goblin", "goblin"]` (triple) | Warning A033 |
| 03 | `["goblin", "scout"]` (distinct) | Pass |
| 04 | `["goblin", "scout", "goblin"]` (non-adjacent) | Warning A033 |

### New table — Engine.Affect edge cases (Q1–Q5, spec §11.3.1)
9 cases: NaN delta throws; ±Inf clamps; unknown need warns + no-ops;
empty/null need throws; normal delta applies; overflow clamps both ends.

### New table — Engine.Lock edge cases (Q9, Q10, Q14, Q15, spec §24.5.1)
7 cases: `Lock(0)` immediate Unlock; `Lock(<0)` throws; re-Lock replaces
duration / mode / locked_behavior snapshot; `Unlock` while not locked
no-ops (called once and twice).

### New table — Engine.GetNeed (v0.1.5 new, spec §9.1)
6 cases: known need returns initial; another known need returns initial;
after Affect reflects new value; unknown need returns 0 with Warning;
null throws; empty throws.

### TimeEdge cases — already decided in Phase_2_3_2 (Q11/Q12/Q13)
| # | Input | Expected (v0.1.5 confirmed) |
|---|---|---|
| 01 | dt = 0 | no-op |
| 02 | dt = -1 | throw `ArgumentException` |
| 03 | dt = NaN | throw `ArgumentException` |

### ForceReset Case05 — confirmed semantics (Q16)
Hard-lock + `force_reset = true`: behavior frozen, Need value updates.
The new `Engine.GetNeed` API (added for Q16) lets the test assert the
Need value side; it is now covered by `GetNeedTests.Case03`.

### Threading (Q17, spec §27)
Documented as main-thread only. No test added — the contract is
documentary. A misuse test would require a thread to misuse, which is
outside the EditMode test runner's scope.

---

## Phase_2_4_3 — Lock pipeline sub-questions (Q-S1, Q-S2, Q-S3)

These three resolutions pinned previously-undefined corners of the Lock
specification. See `docs/decisions/v0.1.5_ambiguity_resolution.md` and
spec §24.3.1 / §24.4.1 / §9.2 (T0 phase) for the rationale.

### LockEdgeCase additions
| # | Sub-question | Test name | Asserts |
|---|---|---|---|
| 08 | Q-S1 | `Case08_SoftLock_CommitmentBonusFollowsLockedBehavior` | locked_behavior is preserved during Soft lock even when fear is pushed to drive an internal switch — proves bonus rides locked_behavior, not internal leader |
| 09 | Q-S2 | `Case09_HardLock_NeedsContinueToUpdate` | GetNeed reflects post-Affect change while is_locked == true (Hard) — Steps 1-2 still run during lock |
| 10 | Q-S2 | `Case10_SoftLock_NeedsContinueToUpdate` | same contract, Soft mode |
| 11 | Q-S3 | `Case11_LockExpiresMidFrame_SwitchHappensSameFrame` | After Live(dt) where dt covers remaining duration, is_locked == false in the same call — proves T0 decrement happens at frame head, not frame tail |

### Bus injection (deferred to Phase 3)
Direct verification that Step 3 Bus.Publish *fires* during Lock (vs.
"Need updates" indirection) requires a MockBus injection point on the
Engine constructor. That is filed as a Phase 3 testability follow-up;
v0.1.5 nails the Need-update precondition that makes the Threshold
firing possible at all.

---

## Phase_2_4_4 — API surface sub-questions (Q-S4, Q-S5, Q-S6)

These three close C# idiom and operational-reality gaps in the user-
facing API. See `docs/decisions/v0.1.5_ambiguity_resolution.md` and
spec §9.7.2 / §11.2 / §26.3 for the rationale.

### Q-S4 — ScenarioRunner timed events
Spec-only fix; `ScenarioRunner` is implemented in Phase 6 so no Red test
is added in the Phase 2 baseline. The signature change (Dict → list)
is recorded in spec §26.3.

### Q-S5 — `force_reset` OR-latch contract (added to AffectEdgeCaseTests)
| # | Test | Asserts |
|---|---|---|
| 10 | `Case10_ForceResetLatches_NotClearedByLaterFalseCall` | A second within-frame `Affect` with `force_reset: false` does not clobber a previously-latched true; the engine processes the OR-latched flag without throwing. Direct latch-state assertion deferred to Phase 3 debug API. |

### Q-S6 — Store re-Register contract (new file)
| # | Test | Asserts |
|---|---|---|
| 01 | `Case01_FirstRegister_Succeeds` | first Register succeeds; IsRegistered true |
| 02 | `Case02_ReRegisterSameInstance_IsIdempotentNoOp` | re-Register with same instance no-throw, stays registered |
| 03 | `Case03_ReRegisterDifferentInstance_KeepsFirstNoThrow` | duplicate id from a different instance must NOT throw (Awake safety) |
| 04 | `Case04_AfterUnregisterFirst_CanRegisterDifferentInstance` | once first unregisters, the slot is reusable without warning |

---

## Phase_2_4_5 — Frame-1 / startup sub-questions (Q-S7, Q-S8, Q-S9)

These three close startup-time gaps (Awake crash, first-frame threshold
storm, all-zero tie-break). See decision log Q-S7/S8/S9 and spec §10.3,
§13.1 A016, §16.4-5, §9.2.0a, §9.2 Step 5.

### Q-S7 — Composer fills missing Binding (new Composer test file)
| # | Test | Asserts |
|---|---|---|
| 01 | `Case01_NullBindingInPersona_ComposerFillsDefault` | `binding == null` in input → composed.binding non-null with `Const.DEFAULT_ON_ACTION_CHANGE` |
| 02 | `Case02_NullBindingInKind_ComposerFillsDefault` | binding null in inherited Kind too → still default-filled |
| 03 | `Case03_PartialBindingProvided_OnlyMissingTemplatesFilled` | user-provided values preserved |

### Q-S8 — first-frame _previous_needs seed (added to Step3_Threshold)
| # | Test | Asserts |
|---|---|---|
| 05 | `Case05_FrameOne_HighSpawnNeed_DoesNotFireSpuriousThreshold` | Persona spawned with `fear: 80` survives first Live without throwing; GetNeed reflects spawn value (Bus-publish direct check deferred to Phase 3 with MockBus injection) |

### Q-S9 — Step 5 tie-break + first-Live (added to Step5_ActionSwitch)
| # | Test | Asserts |
|---|---|---|
| 05 | `Case05_AllZeroScores_FirstDeclaredActionWins` | All-zero needs → tie → `actions[0]` ("Flee") wins |
| 06 | `Case06_AllZeroScores_ReorderedActions_NewFirstWins` | Same Persona, actions[] reordered → new index-0 ("Idle") wins (proves rule is declaration order, not tier/exponent) |
| 07 | `Case07_FirstLive_NoCommitmentBonus_PureScoreCompetition` | first Live with high hunger → "Eat" wins; no commitment cushion exists yet |
