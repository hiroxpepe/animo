# Animo Test Plan — Phase 2 Red Baseline (v0.1.4)

This document is the decision table specification for Phase 2 Task 2-3.
Every row of every table maps to one `[Test]` method in `Tests~/EditModeTests/`.

**Totals:**
- 35 Validator test files (covering A000–A032, including A020a/b/c split)
- 4 Composer test files
- 9 Engine test files (5 step files + 4 feature files)
- 4 EdgeCases test files
- **183 test methods** (target: 180+)

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
