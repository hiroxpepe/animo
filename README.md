# Animo

> **A Maslow-Driven Utility AI for Game Agents**
>
> Part of the **G+B+A stack** (Germio + Briko + Animo).

[![.NET](https://img.shields.io/badge/.NET-8-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/)
![Phase](https://img.shields.io/badge/phase-3-blue)
![Version](https://img.shields.io/badge/version-v0.3.9-orange)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## What is Animo?

Animo is a **Utility AI engine** for a game's own agents —
enemies, NPCs, anything at all that needs to *want* something.

It models an inner drive with **Maslow's own layers of need**. You
write a JSON file that says what an agent *cares about*. The
engine reads it, works out the inner needs as time passes, and
decides what the agent does next — with no behavior tree, no
state machine, and no string a search by name at all in the hot path.

> **Germio asks "what." Briko asks "where." Animo asks "why."**

---

## The three questions

```mermaid
flowchart LR
  subgraph Q["The Three Questions of Game AI"]
    direction LR
    QW["WHAT<br/>What happens<br/>game logic"]
    QH["WHERE<br/>Where it happens<br/>level the way things are laid out"]
    QY["WHY<br/>Why it acts<br/>agent inner state"]
  end
  G["Germio<br/>v0.5.19"]
  B["Briko<br/>v0.1.0"]
  A["Animo<br/>v0.3.0"]
  QW --> G
  QH --> B
  QY --> A
  style A fill:#fef3c7,stroke:#ca8a04,stroke-width:3px
  style G fill:#e8f4f8,stroke:#0369a1
  style B fill:#ede9fe,stroke:#7c3aed
```

Animo is the **WHY** layer.
Most game AI mixes *what* the agent does in with *why* it does it.
Animo keeps the two apart — and that is the whole point of it.

---

## Where things stand

🟢 **Phase 1 — Design done (v0.1.4 → v0.1.5)**
🟢 **Phase 2 — Schema + a red baseline, done (v0.2.0)**
🟢 **Phase 3 — Core Engine + ScenarioRunner, done (v0.3.0)**
⬜ **Phase 4 — Unity work + a CLI (next)**

```mermaid
flowchart LR
  P0["Phase 0<br/>Concept<br/>v0.1.0"]
  P1["Phase 1<br/>Design<br/>v0.1.5"]
  P2["Phase 2<br/>Schema + Red tests<br/>v0.2.0"]
  P3["Phase 3<br/>Core Engine<br/>v0.3.0"]
  P4["Phase 4<br/>Unity integration<br/>v0.4.0"]
  P5["Phase 5<br/>Stabilize<br/>v1.0.0"]
  P0 --> P1 --> P2 --> P3 --> P4 --> P5
  style P0 fill:#d1fae5,stroke:#059669
  style P1 fill:#d1fae5,stroke:#059669
  style P2 fill:#d1fae5,stroke:#059669
  style P3 fill:#d1fae5,stroke:#059669,stroke-width:3px
  style P4 fill:#fef3c7,stroke:#ca8a04
  style P5 fill:#f1f5f9,stroke:#64748b
```

**At v0.3.0** the core engine's own work is done, in full, proven by
mathematics (452 tests Green, checked to make no waste at all in memory),
and free of Unity (`Animo.Core` / `Animo.Model` / `Animo.Tools` use
no `UnityEngine` reference at all). Phase 4 puts the proven core
inside Unity's own parts, and ships a CLI runner.

+ 📄 [English the exact plan (current)](docs/animo_spec_v0.1.5_EN.md) — the true, real reference used to build it
+ 📄 [Japanese the exact plan](docs/animo_spec_v0.1.5_JP.md) — the first talk on its design
+ 📊 [State of Animo v0.3.0](docs/state_of_animo_v0.3.0.md) — a look back on Phase 3, and the gap left before Phase 4
+ ⚡ [Benchmarks v0.3.0](docs/benchmarks_v0.3.0.md) — how the "no memory waste" claim was measured
+ 📝 [CHANGELOG](CHANGELOG.md) — the release notes
+ 🗺️ [Roadmap to v1.0.0](docs/animo_roadmap_to_v1.0.0.md)

---

## Why Animo?

Most game AI uses a Behavior Tree, or a Finite State Machine.
Both put *what to do* into words, but force *why* to be put in too,
in an indirect way — through the order of points, the conditions
for a change, or a shared set of variables on a blackboard.

Animo turns this around. **You state the needs.** The engine works
out the rest, on its own.

```mermaid
flowchart TB
  subgraph Old["❌ Behavior Tree style"]
    BT["Sequence<br/>→ Selector<br/>→ Condition<br/>→ Action"]
    BT_Why["WHY is hidden<br/>inside transition logic"]
    BT --> BT_Why
  end
  subgraph New["✅ Animo style"]
    Need["Needs<br/>(hunger, fear, curiosity)"]
    Score["Utility scores<br/>(per action)"]
    Pick["Pick best"]
    Need --> Score --> Pick
    Why["WHY is data<br/>(LLM-writable JSON)"]
    Need -.-> Why
  end
  style Old fill:#fee2e2,stroke:#dc2626
  style New fill:#dcfce7,stroke:#16a34a
```

You write this:

```json
{
  "agent_id": "goblin_01",
  "kind_ids": ["goblin", "scout"],
  "needs": {
    "hunger": 40, "fear": 55, "curiosity": 45
  }
}
```

The engine takes it from there — decay, holding a need back,
working out a score, switching between acts, locking to an
animation, all of it.

---

## The build, at a glance

```mermaid
flowchart TB
  subgraph JSON["📄 animo.json"]
    direction LR
    JK["kinds[]<br/>type definitions"]
    JP["personas[]<br/>individual definitions"]
  end

  subgraph Model["🧬 Animo.Model"]
    direction LR
    MR["Root"]
    MK["Kind"]
    MP["Persona"]
    MN["Needs / Rates"]
    MI["Influence"]
    MA["Action"]
    MB["Binding"]
  end

  subgraph Core["⚙️ Animo.Core"]
    direction LR
    CC["Composer<br/>deep copy + cascade"]
    CE["Engine<br/>5-step Live(delta_time)"]
    CV["Validator<br/>A000–A039"]
  end

  subgraph Runtime["🎮 Animo (Unity)"]
    direction LR
    RA["Agent<br/>MonoBehaviour"]
    RS["Store<br/>the one, and only, copy"]
    RL["AnimoLog"]
  end

  Germio["Germio.Bus<br/>(external)"]

  JSON -->|"deserialize"| Model
  Model -->|"raw Persona"| CC
  CC -->|"composed Persona<br/>(deep copy)"| CE
  Model -->|"validate"| CV
  CE -.->|"behavior change"| RA
  RA -->|"Register/Unregister"| RS
  RS -->|"Affect relay"| CE
  RA -->|"Bus.Publish<br/>(cached strings)"| Germio

  style JSON fill:#fce7f3,stroke:#be185d
  style Model fill:#ede9fe,stroke:#7c3aed
  style Core fill:#e8f4f8,stroke:#0369a1
  style Runtime fill:#fef3c7,stroke:#ca8a04
  style Germio fill:#e8d5ff,stroke:#7e3ff2
```

### Where one layer ends (a strict rule)

```mermaid
flowchart TB
  Animo["Animo<br/>Agent / Store / Const<br/><i>Unity layer</i>"]
  Core["Animo.Core<br/>Engine / Composer / Validator<br/><i>logic layer</i>"]
  Model["Animo.Model<br/>Root / Kind / Persona / Needs<br/><i>pure data layer</i>"]
  Animo -->|"uses"| Core
  Animo -->|"uses"| Model
  Core -->|"uses"| Model
  Model -.->|"❌ forbidden"| Core
  Core -.->|"❌ forbidden"| Animo
  style Animo fill:#fef3c7,stroke:#ca8a04
  style Core fill:#e8f4f8,stroke:#0369a1
  style Model fill:#ede9fe,stroke:#7c3aed
```

A higher layer may use a lower one. A lower layer **must not** know
of a higher one at all.
This is what lets `Animo.Core` be tested with no Unity at all.

---

## How one frame works (`Engine.Live(delta_time)`)

Every Animo agent runs the same five steps, each frame. The Lock
part (v0.1.4) lets an animation's own state freeze a decision,
with no freeze on the simulation itself.

```mermaid
flowchart TB
  Start(["Live(delta_time) called"])
  S1["Step 1: natural decay<br/>update each Need with Rates<br/><i>Clamp [0, 100]</i>"]
  S2["Step 2: EffectiveNeeds<br/>apply influences in order-sorted order<br/><i>Clamp after each edge</i>"]
  S3["Step 3: Threshold check<br/>compare with previous frame<br/><i>Bus.Publish (cached strings)</i>"]
  S4["Step 4: Action score calc<br/>dynamic Maslow suppression<br/><i>add commitment.bonus to current</i>"]
  Lock{"is_locked?<br/>(v0.1.4)"}
  S5["Step 5: switch decision<br/>pick best score"]
  Skip["Skip Step 5<br/>keep locked behavior"]
  End(["update behavior"])
  Start --> S1 --> S2 --> S3 --> S4 --> Lock
  Lock -->|"No"| S5 --> End
  Lock -->|"Yes (Hard Lock)"| Skip --> End
  style S2 fill:#fef3c7,stroke:#ca8a04
  style S4 fill:#fecaca,stroke:#dc2626
  style S5 fill:#fecaca,stroke:#dc2626
  style Lock fill:#e8f4f8,stroke:#0369a1
  style Skip fill:#ede9fe,stroke:#7c3aed
```

---

## Maslow, and holding a need back

The one part that makes Animo *Maslow*. A low-tier need
(survival) holds a high-tier one (rising to your own full self)
back — but only while it is, in fact, unmet.

```mermaid
flowchart TB
  T1["Tier 1: Physiological<br/>hunger, thirst, sleep"]
  T2["Tier 2: Safety<br/>fear, shelter"]
  T3["Tier 3: Social<br/>belonging, affection"]
  T4["Tier 4: Esteem<br/>status, recognition"]
  T5["Tier 5: Self-actualization<br/>curiosity, creativity"]
  T1 -.->|"suppresses if<br/>Tier 1 high"| T2
  T2 -.->|"suppresses if<br/>Tier 2 high"| T3
  T3 -.->|"suppresses if<br/>Tier 3 high"| T4
  T4 -.->|"suppresses if<br/>Tier 4 high"| T5
  style T1 fill:#fecaca,stroke:#dc2626
  style T2 fill:#fed7aa,stroke:#ea580c
  style T3 fill:#fef3c7,stroke:#ca8a04
  style T4 fill:#dcfce7,stroke:#16a34a
  style T5 fill:#dbeafe,stroke:#2563eb
```

A in need of food goblin, near starving, will not go out to explore. A
safe, well-fed goblin will. This comes straight out of the
formula — nowhere do you write "if in need of food, then no exploring."

---

## Cascading: Kind × Persona

Much like CSS, Animo lets you set a type (`kinds`), then change it
per one, single agent (`personas`). The cascade is
**last-wins**, and copied deep, to keep clear of a shared-reference
bug.

```mermaid
flowchart LR
  K1["kind: goblin<br/>hunger=40<br/>fear=70"]
  K2["kind: scout<br/>curiosity=80"]
  P["persona: goblin_01<br/>fear=55"]
  Result["composed:<br/>hunger=40 (from goblin)<br/>fear=55 (from persona)<br/>curiosity=80 (from scout)"]
  K1 -->|"merge"| K2
  K2 -->|"merge"| P
  P -->|"deep copy"| Result
  style Result fill:#dcfce7,stroke:#16a34a
```

`kind_ids: ["goblin", "scout"]` means *be a goblin, but also a
scout*. The persona's own JSON lists only what is different.

---

## The JSON schema

Every `animo.json` is checked against
`Schemas/animo.schema.json` (JSON Schema **Draft-07**) before the
runtime Validator is ever run at all. The schema checks types,
structure, ranges, and patterns; the runtime Validator handles
meaning across fields, cycle checks, and template fill-in points
(see §13.6 of the spec).

```mermaid
flowchart LR
  JSON["animo.json"]
  Schema["Schemas/animo.schema.json<br/>Draft-07<br/>type / structure / range / pattern"]
  Validator["Animo.Core.Validator<br/>semantics<br/>cross-field / cycles / templates"]
  Engine["Engine accepts"]
  JSON --> Schema --> Validator --> Engine
  style Schema fill:#e8f4f8,stroke:#0369a1
  style Validator fill:#fef3c7,stroke:#ca8a04
  style Engine fill:#dcfce7,stroke:#16a34a
```

Three sample personas live under `examples/`:

| Sample | Style | Notes |
| --- | --- | --- |
| `goblin_scout.json` | a monster, like Zelda's own | more than one kind (`goblin` + `scout`), the standard 8 Needs, a threshold that holds steady between two points |
| `tanukichi.json` | an NPC, like Animal Crossing's own | a three-kind cascade (`villager` + `energetic`), a binding with no threshold at all |
| `shiori.json` | a heroine, like Tokimeki's own | its own Needs (`anger`, `longing`, `jealousy`), a three-kind cascade, two thresholds |

All three pass Green; a 25-case negative test proves the schema
turns down badly-formed input, as it should (including an empty
`thresholds[]`, a need out of range, a key not in snake_case, and
a field it does not know).

---

## The test test frame: MiniUnity

`Animo.Core` and `Animo.Model` must be fit for a test in **plain
C# alone**, with no need to start Unity at all. The
`Animo.Tests.MiniUnity` test frame gives Unity-shaped stand-
(`MockGameObject`, `MockMonoBehaviour`, `MockBus`, `MockTime`,
`MockScene`), so an EditMode-style test can drive
`Awake → Update → OnDestroy` straight, from a `dotnet test` runner.

```mermaid
flowchart LR
  Test["NUnit test"]
  Scene["MockScene"]
  Obj["MockGameObject"]
  Comp["MockMonoBehaviour subclass<br/>(your Animo.Agent under test)"]
  Time["MockTime<br/>virtual clock"]
  Bus["MockBus<br/>records Publish calls"]
  Test -->|"Tick(delta_time)"| Scene
  Scene -->|"sets deltaTime"| Time
  Scene -->|"Update()"| Obj
  Obj -->|"Update()"| Comp
  Comp -.->|"Publish(signal_id)"| Bus
  Test -->|"Assert published_signals"| Bus
  style Comp fill:#fef3c7,stroke:#ca8a04
  style Bus fill:#e8d5ff,stroke:#7e3ff2
```

The test frame ships with four tests of its own (the order things
happen in, `MockBus` keeping a record, `MockTime.Step` moving
forward, a check that a destroyed object is dropped) that **must
pass** before any test built on top of it can be trusted; with no
proof of these, Phase 2-3 would stand on a base that was never
checked, and still show red.

The asmdef states `"references": []` and
`"noEngineReferences": true`; the test frame holds no line at all
that says `using UnityEngine`. Unity leaves the whole `Tests~/`
folder alone, so the test frame lives only for the `dotnet` build.

---

## A red baseline (Phase 2-3)

Before any real logic is written, the test a group of tests is built
**red-first**. Every choice laid out in the spec — every Validator
rule, every case in the Composer's own cascade, every step of
`Engine.Live`, every edge case in a number, an empty value, an
amount, or time — has a `[Test]` method that *will* pass once
Phase 3 builds the class it needs. Until then, every test is red,
and that is the whole point of it.

```mermaid
flowchart LR
  Plan["docs/test_plan_v0.1.4.md<br/>decision tables"]
  Tests["Tests~/EditModeTests/<br/>183 [Test] methods"]
  Run["dotnet test"]
  Result["183 Failed / 0 Passed<br/>(Red baseline)"]
  Phase3["Phase 3<br/>implement classes"]
  Green["Tests turn Green<br/>one rule at a time"]
  Plan --> Tests --> Run --> Result
  Result -.->|"v0.2.0-red-baseline tag"| Phase3
  Phase3 --> Green
  style Result fill:#fecaca,stroke:#dc2626
  style Green fill:#dcfce7,stroke:#16a34a
```

The count matches how the spec breaks it down (§13 Validator
rules, §10 Composer, §9 Engine, §4.6.3 the list of edge cases):

| Layer | Files | Tests |
| --- | --- | --- |
| Validator (A000-A032, with A020 split into a/b/c) | 35 | 92 |
| Composer (deep copy, cascade, fill-in, more than one kind) | 4 | 24 |
| Engine (5 steps, plus Maslow / Commitment / Lock / ForceReset) | 9 | 44 |
| Edge cases (a number, an empty or null value, an amount, time) | 4 | 23 |
| **Total** | **52** | **183** |

The 4 MiniUnity tests of its own are the only Green tests, at this
point. The runner's own output, taken together:

```text
Tests run: 187, Passed: 4, Failed: 183
```

That output, taken at this commit, is the **v0.2.0-red-baseline**
snapshot.

For the full, round-by-round record of the hard review this
project went through, with Gemini, on the way to v0.1.5, see
[Animo Development Log](docs/animo_development_log.md) (151
things found, all checked by search).

## The Validator: 40 rules (A000-A039)

Every `animo.json` goes through 40 rules of the Validator, before
the engine ever touches it at all. Most run on the raw JSON
(stage 1); A019 (a a spelling mistake, a Warning, **moved to stage 2 in Q-S39**,
so it can see the merged `needs_meta`), A025 (a cycle, once
composed), A035 (after fill-in, `trigger > reset`), A036 (composed
`actions[]`, must not be empty), A037 (more than one edge, to the
same target — a Warning), A038 (`needs_meta[need].tier`, an unused
check, from Q-S30, Q-S41, Q-S49, and Q-S57), and A039 (a Warning,
for a threshold pair sitting close together, from Q-S47) all run
after `Composer.Compose` (stage 2), so they can see the graph, once
merged.

```mermaid
flowchart TB
  J["animo.json"]
  V["Validator stage 1<br/>A000–A018, A020–A034, A038<br/>(raw JSON)"]
  E["Errors<br/>(A000–A025, A034, A038: must fix)"]
  W["Warnings<br/>(A028–A033, A037, A038, A039: review)"]
  C["Composer.Compose"]
  V2["Validator stage 2<br/>A019 (a spelling mistake, sees needs_meta — Q-S39),<br/>A025 (composed cycle),<br/>A035 (post-fill trigger>reset),<br/>A036 (composed actions non-empty),<br/>A037 (multi-edge same target — Warn),<br/>A038 (unused, sees actions/influences/thresholds/rates — Q-S41+Q-S49+Q-S57),<br/>A039 (another built from the same start threshold proximity — Q-S47)"]
  G["Engine accepts"]
  J --> V
  V --> E
  V --> W
  V -->|"if no errors"| C --> V2
  V2 -->|"if no errors"| G
  V2 -->|"ghost cycle / trigger==reset / empty actions"| E
  style E fill:#fecaca,stroke:#dc2626
  style W fill:#fef3c7,stroke:#ca8a04
  style G fill:#dcfce7,stroke:#16a34a
```

A few examples:

+ **A002** — `agent_id` must be snake_case
+ **A025** — a cycle, in an Influence, is an Error (since v0.1.2)
+ **A028** — a `commitment.bonus` of 30 or more raises a Warning
  (a a chance of harm of chattering)
+ **A031** — a `lock.duration` above five seconds raises a Warning
  (an agent, frozen too long)

The full list stands in [§13 of the spec](docs/animo_spec_v0.1.5_EN.md).

---

## What it does

+ 🧠 **Driven only by a need** — every act rises from an inner
  need, never from a a small program
+ ⛰️ **Maslow, holding needs back, on its own** — a low-tier need
  holds a high-tier one back, all on its own
+ 🎨 **A cascade, much like CSS** — the `kind_ids` array gives more
  than one line of descent, with a merge that always gives the
  same, one answer
+ 🚀 **Built for the hot path** — no string a search by name, no waste of
  memory, values kept in an indexed `float[]`
+ 🤖 **LLM-first** — its own JSON schema is made to be edited by an
  LLM
+ 🔒 **A Behavior Lock, in its own API** — keeps an animation's own
  state in step with a decision (v0.1.4)
+ 📉 **A word back from failure** — an agent fails, learns from it,
  and switches (v0.1.1)
+ ⚡ **Commitment, that holds steady** — holds off chattering, with
  no decay over time at all (v0.1.3)
+ 🪶 **The engine is free of Unity** — `Animo.Core` and
  `Animo.Model` can be tested in plain C# alone

---

## Roadmap

| Phase | Its own aim | Where it stands |
| --- | --- | --- |
| Phase 0 | The idea (v0.1.0) | ✅ Done |
| Phase 1 | Design (v0.1.5) | ✅ Done |
| Phase 2 | Schema, plus a red baseline (v0.2.0) | ✅ Done |
| **Phase 3** | **The Core Engine, plus ScenarioRunner (v0.3.0)** | ✅ **Done** |
| Phase 4 | Work with Unity, plus a CLI (v0.4.0) | 🔥 Next |
| Phase 5 | Made steady, and put on the Asset Store (v1.0.0) | ⬜ |

See [animo_roadmap_to_v1.0.0.md](docs/animo_roadmap_to_v1.0.0.md)
for the full map of tasks left.

---

## License

The MIT License. See [LICENSE](LICENSE).

Copyright (c) STUDIO MeowToon. All rights held.

---

## Author

**h.adachi** ([STUDIO MeowToon](https://github.com/hiroxpepe))

---

## See also

+ [Germio](https://github.com/hiroxpepe/stemic) — a library for a
  game's own logic (**WHAT**)
+ [Briko](https://github.com/hiroxpepe/briko) — a library for
  building a level (**WHERE**)
+ **Animo** — a library for an agent's own, inner drive (**WHY**)
  ← you are here
