# Animo

> **Maslow-driven Utility AI for Game Agents**
>
> Part of the **G+B+A stack** (Germio + Briko + Animo).

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Status: Design Complete](https://img.shields.io/badge/Status-Design%20Complete-green.svg)](docs/animo_spec_v0.1.4_EN.md)
[![Spec: v0.1.4](https://img.shields.io/badge/Spec-v0.1.4-blue.svg)](docs/animo_spec_v0.1.4_EN.md)
[![Phase 1: Done](https://img.shields.io/badge/Phase%201-Done-brightgreen.svg)](docs/animo_roadmap_to_v1.0.0.md)

---

## What is Animo?

Animo is a **Utility AI engine** for game agents — enemies, NPCs, anything that needs to *want* something.

It models inner motivation using **Maslow's hierarchy of needs**.
You write a JSON file describing what an agent *cares about*.
The engine reads it, simulates internal needs over time, and decides what the agent does next — with no behavior tree, no state machine, and no string lookups in the hot path.

> **Germio asks "what". Briko asks "where". Animo asks "why".**

---

## The Three Questions

```mermaid
flowchart LR
  subgraph Q["The Three Questions of Game AI"]
    direction LR
    QW["<b>WHAT</b><br/>What happens<br/>game logic"]
    QH["<b>WHERE</b><br/>Where it happens<br/>level layout"]
    QY["<b>WHY</b><br/>Why it acts<br/>agent inner state"]
  end
  G["<b>Germio</b><br/>v0.5.19-alpha"]
  B["<b>Briko</b><br/>v0.1.0-alpha"]
  A["<b>Animo</b><br/>v0.1.4-design"]
  QW --> G
  QH --> B
  QY --> A
  style A fill:#fef3c7,stroke:#ca8a04,stroke-width:3px
  style G fill:#e8f4f8,stroke:#0369a1
  style B fill:#ede9fe,stroke:#7c3aed
```

Animo is the **WHY** layer.
Most game AI mixes *what* the agent does with *why* it does it. Animo separates them — and that separation is the whole point.

---

## Status

🟢 **Phase 1 — Design Complete**
🔥 **Phase 2 — Schema and Test Foundation (in progress)**

```mermaid
flowchart LR
  P0["<b>Phase 0</b><br/>Concept<br/>v0.1.0"]
  P1["<b>Phase 1</b><br/>Design<br/>v0.1.4"]
  P2["<b>Phase 2</b><br/>Schema + Red tests<br/>v0.2.0-test"]
  P3["<b>Phase 3</b><br/>Implementation<br/>v0.3.0-impl"]
  P4["<b>Phase 4</b><br/>Unity integration<br/>v0.4.0-unity"]
  P5["<b>Phase 5</b><br/>Stabilize<br/>v1.0.0"]
  P0 --> P1 --> P2 --> P3 --> P4 --> P5
  style P0 fill:#d1fae5,stroke:#059669
  style P1 fill:#d1fae5,stroke:#059669
  style P2 fill:#fef3c7,stroke:#ca8a04,stroke-width:3px
  style P3 fill:#f1f5f9,stroke:#64748b
  style P4 fill:#f1f5f9,stroke:#64748b
  style P5 fill:#fce7f3,stroke:#be185d
```

The architecture is locked. Eight commits of iterative spec design (v0.1.0 → v0.1.4) put us at a point where **every formula, every validator rule, every namespace boundary is decided**. Implementation can now start without redesigning anything mid-flight.

- 📄 [English specification (reference)](docs/animo_spec_v0.1.4_EN.md) — implementation truth
- 📄 [Japanese specification](docs/animo_spec_v0.1.4_JP.md) — original design discussion
- 🗺️ [Roadmap to v1.0.0](docs/animo_roadmap_to_v1.0.0.md)

---

## Why Animo?

Most game AI uses Behavior Trees or Finite State Machines.
Both encode *what to do* but force you to also encode *why* indirectly — through node ordering, transition conditions, blackboard variables.

Animo flips the model. **You declare needs.** The engine works out the rest.

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

The engine handles the rest — decay, suppression, score calculation, action switching, animation locking, all of it.

---

## Architecture at a Glance

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
    CE["Engine<br/>5-step Live(dt)"]
    CV["Validator<br/>A000–A032"]
  end

  subgraph Runtime["🎮 Animo (Unity)"]
    direction LR
    RA["Agent<br/>MonoBehaviour"]
    RS["Store<br/>singleton"]
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

### Layer Boundaries (Strict)

```mermaid
flowchart TB
  Animo["<b>Animo</b><br/>Agent / Store / Const<br/><i>Unity layer</i>"]
  Core["<b>Animo.Core</b><br/>Engine / Composer / Validator<br/><i>logic layer</i>"]
  Model["<b>Animo.Model</b><br/>Root / Kind / Persona / Needs<br/><i>pure data layer</i>"]
  Animo -->|"uses"| Core
  Animo -->|"uses"| Model
  Core -->|"uses"| Model
  Model -.->|"❌ forbidden"| Core
  Core -.->|"❌ forbidden"| Animo
  style Animo fill:#fef3c7,stroke:#ca8a04
  style Core fill:#e8f4f8,stroke:#0369a1
  style Model fill:#ede9fe,stroke:#7c3aed
```

A higher layer can use a lower one. A lower layer **must not** know about a higher one.
This makes `Animo.Core` testable without Unity.

---

## How a Frame Works (`Engine.Live(dt)`)

Every Animo agent runs the same 5 steps each frame. The Lock mechanism (v0.1.4) lets animation states freeze decisions without freezing the simulation.

```mermaid
flowchart TB
  Start(["Live(dt) called"])
  S1["<b>Step 1: natural decay</b><br/>update each Need with Rates<br/><i>Clamp [0, 100]</i>"]
  S2["<b>Step 2: EffectiveNeeds</b><br/>apply influences in topo-sorted order<br/><i>Clamp after each edge</i>"]
  S3["<b>Step 3: Threshold check</b><br/>compare with previous frame<br/><i>Bus.Publish (cached strings)</i>"]
  S4["<b>Step 4: Action score calc</b><br/>dynamic Maslow suppression<br/><i>add commitment.bonus to current</i>"]
  Lock{"is_locked?<br/>(v0.1.4)"}
  S5["<b>Step 5: switch decision</b><br/>pick best score"]
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

## Maslow Dynamic Suppression

The piece that makes Animo *Maslow*. Low-tier needs (survival) suppress high-tier ones (self-actualization) — but only when actually unmet.

```mermaid
flowchart TB
  T1["<b>Tier 1: Physiological</b><br/>hunger, thirst, sleep"]
  T2["<b>Tier 2: Safety</b><br/>fear, shelter"]
  T3["<b>Tier 3: Social</b><br/>belonging, affection"]
  T4["<b>Tier 4: Esteem</b><br/>status, recognition"]
  T5["<b>Tier 5: Self-actualization</b><br/>curiosity, creativity"]
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

A starving goblin won't go exploring. A safe, well-fed goblin will. This emerges from the formula — you don't write "if hungry then no exploring" anywhere.

---

## Cascading: Kind × Persona

Like CSS, Animo lets you define types (`kinds`) and override per-individual (`personas`). The cascade is **last-wins**, deep-copied to avoid shared-reference bugs.

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

`kind_ids: ["goblin", "scout"]` means *be a goblin, but also a scout*. The persona JSON only lists the differences.

---

## JSON Schema

Every `animo.json` is checked against `Schemas/animo.schema.json` (JSON Schema **Draft-07**) before the runtime Validator ever runs. The schema covers types, structure, ranges, and patterns; the runtime Validator handles cross-field semantics, cycle detection, and template placeholders (see §13.6 of the spec).

```mermaid
flowchart LR
  JSON["animo.json"]
  Schema["Schemas/animo.schema.json<br/><b>Draft-07</b><br/>type / structure / range / pattern"]
  Validator["Animo.Core.Validator<br/><b>semantics</b><br/>cross-field / cycles / templates"]
  Engine["Engine accepts"]
  JSON --> Schema --> Validator --> Engine
  style Schema fill:#e8f4f8,stroke:#0369a1
  style Validator fill:#fef3c7,stroke:#ca8a04
  style Engine fill:#dcfce7,stroke:#16a34a
```

Three reference personas live under `examples/`:

| Sample | Style | Notes |
|---|---|---|
| `goblin_scout.json` | Zelda-style monster | multi-kind (`goblin` + `scout`), standard 8 Needs, threshold with hysteresis |
| `tanukichi.json` | Animal Crossing-style NPC | three-kind cascade (`villager` + `energetic`), binding without thresholds |
| `shiori.json` | Tokimeki-style heroine | custom Needs (`anger`, `longing`, `jealousy`), three-kind cascade, two thresholds |

All three validate Green; a 25-case negative test confirms the schema rejects malformed input as expected (including empty `thresholds[]`, out-of-range needs, non-snake_case keys, and unknown fields).

---

## Validator: 33 Rules (A000–A032)

Every `animo.json` goes through 33 validator rules before the engine ever touches it.

```mermaid
flowchart TB
  J["animo.json"]
  V["Validator<br/>A000–A032"]
  E["Errors<br/>(A000–A025: must fix)"]
  W["Warnings<br/>(A028–A032: review)"]
  G["Engine accepts"]
  J --> V
  V --> E
  V --> W
  V -->|"if no errors"| G
  style E fill:#fecaca,stroke:#dc2626
  style W fill:#fef3c7,stroke:#ca8a04
  style G fill:#dcfce7,stroke:#16a34a
```

Examples:
- **A002** — `agent_id` must be snake_case
- **A025** — Influence cycles are an error (since v0.1.2)
- **A028** — `commitment.bonus` ≥ 30 raises a warning (chattering risk)
- **A031** — `lock.duration` over 5s raises a warning (frozen agent)

The full list is in [§13 of the spec](docs/animo_spec_v0.1.4_EN.md).

---

## Key Features

- 🧠 **Pure need-driven** — every action emerges from an inner need, not a script
- ⛰️ **Maslow dynamic suppression** — low-tier needs suppress high-tier ones automatically
- 🎨 **CSS-style cascading** — `kind_ids` array gives multiple inheritance with deterministic merge
- 🚀 **Hot path optimized** — zero string lookup, zero GC allocation, indexed `float[]` storage
- 🤖 **LLM-first** — JSON schema designed for LLM editing
- 🔒 **Behavior Lock API** — sync animation states with decisions (v0.1.4)
- 📉 **Frustration feedback** — agents fail, learn, switch (v0.1.1)
- ⚡ **Commitment hysteresis** — anti-chattering without time-decay (v0.1.3)
- 🪶 **Engine is Unity-free** — `Animo.Core` and `Animo.Model` test in pure C#

---

## Roadmap

| Phase | Goal | Status |
|---|---|---|
| Phase 0 | Concept (v0.1.0) | ✅ Done |
| **Phase 1** | **Design (v0.1.4)** | ✅ **Done** |
| Phase 2 | Schema + Red tests (v0.2.0-test) | 🔥 In progress |
| Phase 3 | Implementation (v0.3.0-impl) | ⬜ |
| Phase 4 | Unity integration (v0.4.0-unity) | ⬜ |
| Phase 5 | Stabilize (v1.0.0) | ⬜ |

See [animo_roadmap_to_v1.0.0.md](docs/animo_roadmap_to_v1.0.0.md) for the full task graph.

---

## License

MIT License. See [LICENSE](LICENSE).

Copyright (c) STUDIO MeowToon. All rights reserved.

---

## Author

**h.adachi** ([STUDIO MeowToon](https://github.com/hiroxpepe))

---

## See also

- [Germio](https://github.com/hiroxpepe/stemic) — game logic library (**WHAT**)
- [Briko](https://github.com/hiroxpepe/briko) — level construction library (**WHERE**)
- **Animo** — agent inner motivation library (**WHY**) ← you are here
