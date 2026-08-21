# Persona Design Spec

> How to design a new Animo persona. Checked true, 2026-08-19, in a plan
> talk with Master. Still being written — more rules to come.

---

## 0. The persona itself is Animo's own true heart

**Master's own word, 2026-08-19.** A true persona (Need paired with
Action, each Stage held whole) is Animo's own core true work — not the
Adapter, not the Sensor, not the true call path joining Animo to
Germio. Settle the persona first, whole and true, before writing one
line of Adapter code. Once a persona truly stands, the Adapter's own
whole job is small and plain: carry out whatever Behavior the persona
picks, faithfully, with no true say of its own in what gets picked.

---

## 1. Every one of Maslow's own 5 stages must hold a true Need

**Master's own word, 2026-08-19: when a persona is made, one Need must
be given to each of Maslow's own 5 true stages — no stage left with no
Need at all.**

Maslow's own 5 stages (unchanged, kept apart from Animo's own true
code):

| Stage | Name                      | Sense                                                                             |
| ----- | ------------------------- | --------------------------------------------------------------------------------- |
| 1     | Body needs                | The most root-level true need — food, water, sleep, breath, staying warm.         |
| 2     | Safety                    | Being safe from harm, a steady place to live, a steady true way to earn.          |
| 3     | Love / Belonging          | Friends, family, a close true bond — being part of a group, given and given back. |
| 4     | Being held in true regard | Being held in true regard by others; one's own true trust in the self.            |
| 5     | Becoming one's true self  | Reaching one's own true, full growth — making, building, becoming more.           |

`goblin_scout` (checked true against `examples/goblin_scout.json`) held
no true Need at Stage 4 — the first real case this rule caught. §6
below holds its own whole, made-new form.

---

## 2. A Need never stands alone — it is always given one Action too

**Checked true, 2026-08-19, against every persona real code or this
document holds.** A `Need` on its own does nothing at all; it must be
given one true `Action` (an `id`, tied to that `Need`, at a given
`tier`), so the Engine has something real to pick once that Need
climbs. Design a persona by picking this pair — Need plus Action —
together, one true pair per Stage, never a Need alone.

A real, given check of every persona found today (2026-08-19), each
row a true Need/Action pair, held to one Maslow Stage:

### `goblin_scout` (`examples/goblin_scout.json`)

| Stage | Need (sense)          | Action (sense)                   |
| ----- | --------------------- | -------------------------------- |
| 1     | `hunger` (want food)  | `SearchFood` (go find food)      |
| 1     | `fatigue` (worn out)  | `Rest` (stop and rest)           |
| 2     | `fear` (afraid)       | `Flee` (run away)                |
| 3     | `loneliness` (alone)  | `Socialize` (go be near someone) |
| 5     | `idle` (with no want) | `Patrol` (wander around)         |

### `shiori` (`examples/shiori.json`)

| Stage | Need (sense)                | Action (sense)                      |
| ----- | --------------------------- | ----------------------------------- |
| 2     | `anger` (cross)             | `Confront` (face it head-on)        |
| 2     | `frustration` (held back)   | `Sulk` (go quiet and cross)         |
| 3     | `loneliness` (alone)        | `Withdraw` (pull back and hide)     |
| 4     | `longing` (want to be seen) | `Demand` (ask for it, straight out) |
| 5     | `idle` (with no want)       | `Daydream` (think of other things)  |

### `tanukichi` (`examples/tanukichi.json`)

| Stage | Need (sense)               | Action (sense)                   |
| ----- | -------------------------- | -------------------------------- |
| 1     | `fatigue` (worn out)       | `Rest` (stop and rest)           |
| 3     | `loneliness` (alone)       | `Socialize` (go be near someone) |
| 5     | `curiosity` (want to know) | `Craft` (make something)         |
| 5     | `idle` (with no want)      | `Stroll` (walk around)           |

Both `place_curious` and `company_seeking` (`docs/adapter_spec.md`,
made for `stemic`) hold one true pair alone (Stage 3 each) — under this
whole spec, neither is whole, and both must be built out further.

More still owed here — this section alone is settled for now.

---

## 3. `idle` must never be used as a Need, in any persona at all

**Master's own word, 2026-08-19: "if `idle` is there, every persona
just runs to `idle` at Stage 5."** Stage 5 is a character's own true
"self" — its own real, given want, held by no one else. `idle` (with
no want at all, save true rest) gives every Stage-5 Action an easy way
out, so a true, given want (`curiosity`, say) never gets a real, fair
chance against it. Every persona checked true above (`goblin_scout`,
`shiori`, `tanukichi`, both `_2` forms) still holds `idle` at Stage 5 —
under this rule, none of them are whole; `idle` must be dropped, and a
true, given want put in its place, for every one.

---

## 4. Germio's own "Idle" and Animo's own `idle` are not the same true thing

**Checked true, 2026-08-19, in a plan talk with Master.** These two
words look close, but sit at two whole, different layers:

+ **Germio's own `FixedUpdate.Idle`** is a **body-layer** true state —
  the character simply stands still, with no motion at all. `Rest`
  (Stage 1, `fatigue`) still calls this same true state; the body still
  stands still, whatever the true reason underneath.
+ **Animo's own `idle` (a `Need`, now dropped by Section 3 above) was a
  **want-layer** true idea — "nothing at all is truly wanted." Dropping
  this `Need` never touches Germio's own `Idle` state at all; a
  character still stands still when `Rest` runs, but for a true, given
  reason (`fatigue`), never "no true reason at all."

One true word, at two true layers — never to be read as the same true
thing.

---

## 5. Two Needs must be built to truly compete — or `suppression` and `commitment.bonus` never once get to work

**Checked true, 2026-08-19, against `docs/animo_spec.md` §8.3 and §8.8.**
Picking Stage/Need/Action pairs alone is not the whole true job. If no
two Actions ever truly sit close in score, at the same true moment, two
of the Engine's own core true parts never once get real, true work:

+ **`suppression`** (§8.3) — "a low Need, still unmet, holds a higher
  one back." This only shows itself true where a low-tier Need climbs
  *while* a high-tier Action is close to winning — never where only one
  Need is ever truly high at a time.
+ **`commitment.bonus`** (§8.8) — the true "stick with it" gap
  (`goblin_scout`'s own `+10`, keeping `Patrol` over `Flee` until
  `fear`'s own score truly clears `Patrol + 10`). This only shows
  itself true where two Actions' own scores sit close enough that the
  bonus alone decides which one wins.

**So: for every true persona, at least one true pair of Stages must be
picked on purpose to compete** — given close `tier` numbers (so
`suppression` does not simply put one whole down) and close `exponent`
values (so both climb at a true, matched rate). Pick this pair before
settling final numbers; it is not something that falls out on its own.

---

## 6. Full, real persona builds (checked true, 2026-08-19)

Every element `goblin_scout.json` itself holds
(`rates`/`suppression`/`influences`/`actions`, each with
`id`/`need`/`tier`/`exponent`, plus `commitment.bonus` and each Need's
own starting value) must stand true for a persona to be whole — not
Need/Action pairs alone (§2, above).

### `shiori`, made new again

A plan talk with Master, based on the real Tokimeki Memorial
character Fujisaki Shiori's own, widely-known true manner — a
girl, quiet and true in her feeling, wanting to be seen.

| Stage | Need (sense)                                     | Start | Rate | Exponent | Action (sense)                                | Suppression |
| ----- | ------------------------------------------------ | ----- | ---- | -------- | --------------------------------------------- | ----------- |
| 1     | `fatigue` (worn out)                             | 30    | +1.5 | 1.5      | `Rest` (stop and rest)                        | —           |
| 2     | `insecurity` (a place not steady)                | 40    | +1.0 | 2.0      | `GoHome` (go back to her own place)           | 0.2         |
| 3     | `loneliness` (alone)                             | 55    | +1.2 | 1.5      | `StayNear` (stay close, with no true push)    | 0.4         |
| 4     | `longing` (want to be seen)                      | 50    | +0.8 | 1.3      | `Study` (work hard, to be seen)               | 0.6         |
| 5     | `expression` (a true wish to speak her own mind) | 20    | +0.5 | 1.0      | `Write` (put her own true feeling into words) | 0.8         |

**Influence:** `loneliness → insecurity`, coefficient `0.4` (feeling
alone makes her own place feel less steady too).
**Commitment bonus:** `+8` (a true, given "stick with it" gap, held
lower than `goblin_scout`'s own `+10` — a true, unsure teenager holds
less firm than a survival-driven goblin).
**Competing pair (§5, above):** Stage 3 (`loneliness`) against Stage 4
(`longing`) — close `tier` (3/4) and close `exponent` (1.5/1.3), so
"go be close to him" and "study, to be seen" truly compete.
**`agent_id`:** `shiori_v2`. **`kind_id`:** `heroine_v2` (held apart
from the older `heroine` kind, `docs/animo_spec.md` §19.3).

### `tanukichi`, made new again

A plan talk with Master, based on the Animal Crossing character
Tanukichi's own, widely-known true manner — one who runs a small store, warm,
given to helping the whole true village.

| Stage | Need (sense)                           | Start | Rate | Exponent | Action (sense)                        | Suppression |
| ----- | -------------------------------------- | ----- | ---- | -------- | ------------------------------------- | ----------- |
| 1     | `fatigue` (worn out)                   | 20    | +1.0 | 1.3      | `Rest` (stop and rest)                | —           |
| 2     | `stock_worry` (worry over trade/stock) | 30    | +0.8 | 1.8      | `Restock` (bring in new true stock)   | 0.3         |
| 3     | `loneliness` (alone)                   | 40    | +0.6 | 1.5      | `Socialize` (go be near someone)      | 0.5         |
| 4     | `reliance` (want to be leaned on)      | 35    | +0.5 | 1.4      | `HelpVillager` (help a true villager) | 0.6         |
| 5     | `curiosity` (want to know)             | 50    | +0.8 | 1.0      | `Craft` (make or size up true goods)  | 0.7         |

**Influence:** `stock_worry → reliance`, coefficient `+0.4` (worry over
trade makes him want, all the more, to be leaned on by the village —
held true inside the same 5 Needs, never reaching outside them).
**Commitment bonus:** `+5` (held lower still than `shiori`'s own — a
true, easy-going small-store owner holds to nothing firmly for long).
**Competing pair (§5, above):** Stage 2 (`stock_worry`) against Stage 4
(`reliance`) — "worry over trade" against "wanting to be leaned on by
the village" truly compete.
**`agent_id`:** `tanukichi_v2`. **`kind_id`:** `villager_v2`.

---

### `goblin_scout`, made new again

A plan talk with Master: a scout who feels fear quickly, who cannot fight, and so holds
his own true worth in the one thing he can do — go out, watch, and
bring word back.

| Stage | Need (sense)                                 | Start | Rate | Exponent | Action (sense)                        | Suppression |
| ----- | -------------------------------------------- | ----- | ---- | -------- | ------------------------------------- | ----------- |
| 1     | `hunger` (want food)                         | 40    | +2.0 | 1.8      | `SearchFood` (go find food)           | —           |
| 2     | `fear` (afraid)                              | 20    | -2.0 | 0.6      | `Flee` (run away)                     | 0.3         |
| 3     | `loneliness` (alone)                         | 30    | +1.2 | 1.3      | `Socialize` (go be near someone)      | 0.5         |
| 4     | `recognition` (want to be given true regard) | 25    | +0.5 | 1.3      | `Report` (tell the group what he saw) | 0.7         |
| 5     | `duty` (pride in the work given him)         | 60    | +0.9 | 1.1      | `Patrol` (go out and watch)           | 0.9         |

**Influence:** `fear → duty`, coefficient `-0.5` (real fear breaks down
his own sense of the work given him — held true inside the same 5
Needs).
**Commitment bonus:** `+10` (the highest of all three — a scout on
watch holds to the work given him harder than a teenager or an
easy-going small-store owner).
**Competing pair (§5, above):** Stage 4 (`recognition`) against Stage 5
(`duty`) — close `tier` (4/5) and close `exponent` (1.3/1.1), so "go
back and tell them" and "keep watching" truly compete: a scout's own
true, everyday pull.
**`agent_id`:** `goblin_scout_v2`. **`kind_id`:** `scout_v2` (one kind
alone, matching `shiori`/`tanukichi` above — the older file's own two
kinds, `goblin` plus `scout`, are folded into one).

**What was dropped, and why:**

+ `exponent` on `Flee` was `2.5` in the older file — by §8.5.3's own
  table, that reads "holds back, then bursts out" — the mark of one
  who holds firm under fear, not one who feels it quickly. Set to
  `0.6` ("reacts early").
+ `fatigue`/`Rest` was dropped: one true Need to a Stage, and `hunger`
  speaks more to what a goblin is.
+ `confidence`, `frustration`, `curiosity`, `idle` were all dropped:
  each held a `rate` but no true Action at all, a shape neither
  `shiori` nor `tanukichi` holds.

---

### `place_curious` and `company_seeking`, the two for `stemic`

**A plan talk with Master, 2026-08-20.** These two alone are the real
PoC pair (see `docs/adapter_spec.md`); the three above are design
examples. They were built as one true pair, not one at a time: Maslow's
own holding-back (§8.3) becomes, in play, the bond between them.

`place_curious` cannot reach Stage 5 (`Explore`) while Stage 3
(`loneliness`) sits high — so it can only truly go exploring once
`company_seeking` comes near. And `company_seeking`, each time
`place_curious` walks off, feels Stage 2 (`separation`) climb, and
calls out. One true, closed round.

#### `place_curious` (one who wants new places)

| Stage | Need (sense)                                   | Start | Rate | Exponent | Action (sense)                                | Suppression |
| ----- | ---------------------------------------------- | ----- | ---- | -------- | --------------------------------------------- | ----------- |
| 1     | `fatigue` (worn out)                           | 20    | +1.2 | 1.5      | `Rest` (stop and rest)                        | —           |
| 2     | `exposure` (open to harm, out in the wild)     | 25    | +0.7 | 1.8      | `GoHome` (go back to a known place)           | 0.3         |
| 3     | `loneliness` (alone)                           | 50    | +1.0 | 1.2      | `Approach` (go to the other one)              | 0.6         |
| 4     | `recognition` (want what was found to be seen) | 20    | +0.5 | 1.3      | `ShowFind` (carry the find back, and show it) | 0.7         |
| 5     | `curiosity` (want to know)                     | 60    | +1.2 | 1.0      | `Explore` (go where it has not yet been)      | 0.9         |

**Influence:** `curiosity → loneliness`, coefficient `-0.3` (deep in a
find, it forgets it is alone).
**Commitment bonus:** `+6` (its mind is easily turned).
**Competing pair (§5, above):** Stage 3 (`loneliness`) against Stage 5
(`curiosity`) — checked by real sums: at `loneliness` 40 `Explore`
wins by 10.1; at 50 `Approach` wins by 4.0. A true crossing point sits
right between, and the `+6` bonus holds it steady, with no chattering.
**`agent_id`:** `place_curious_01`. **`kind_id`:** `wanderer`.

#### `company_seeking` (one who wants to be with)

| Stage | Need (sense)                            | Start | Rate | Exponent | Action (sense)                    | Suppression |
| ----- | --------------------------------------- | ----- | ---- | -------- | --------------------------------- | ----------- |
| 1     | `fatigue` (worn out)                    | 20    | +0.6 | 1.5      | `Rest` (stop and rest)            | —           |
| 2     | `separation` (fear of being cut off)    | 30    | +1.0 | 1.4      | `Call` (stand still and call out) | 0.3         |
| 3     | `loneliness` (alone)                    | 60    | +1.5 | 0.8      | `Approach` (go to the other one)  | 0.6         |
| 4     | `usefulness` (want to be of true use)   | 20    | +0.8 | 1.4      | `Tend` (care for the other one)   | 0.7         |
| 5     | `togetherness` (being as one, together) | 40    | +0.9 | 1.1      | `Give` (give to the other one)    | 0.9         |

**Influence:** `loneliness → togetherness`, coefficient `+0.4` (the
more alone it feels, the more it wants to be as one).
**Commitment bonus:** `+12` (the highest of all five — once it picks
someone, it holds on).
**Competing pair (§5, above):** Stage 4 (`usefulness`) against Stage 5
(`togetherness`) — checked by real sums: they cross at `usefulness`
near 72, about 65 seconds in at `+0.8` a second.
**`agent_id`:** `company_seeking_01`. **`kind_id`:** `companion`.

#### The round, checked by real sums

| Moment                                   | `place_curious`                                   | `company_seeking` |
| ---------------------------------------- | ------------------------------------------------- | ----------------- |
| At the start                             | `Approach` 37.0 (`Explore` 33.0, only 4.0 behind) | `Approach` 54.5   |
| Once they meet (`loneliness` -30 each)   | **`Explore` 46.5**                                | `Approach` 31.3   |
| The other walks off (`separation` up 70) | (out exploring)                                   | **`Call` 56.1**   |

Every one of the 10 Actions was checked, by real sums, to truly win at
some point — not one sits dead in the file.

---

## 7. Still owed

+ `ganon` (§19.1), the older `tanukichi`/`villager` form (§19.2), and
  the older `shiori`/`heroine` form (§19.3), all in `animo_spec.md`
  itself, still hold `idle` and gaps of their own — not yet touched.
+ `docs/adapter_spec.md` still shows the older, one-pair-only form of
  `place_curious`/`company_seeking` — to be brought in line with §6
  above.
+ Every persona in §6 stands as a design alone: none is yet written out
  as a real `animo.json` file.
