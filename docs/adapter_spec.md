# Adapter Spec

> How Animo's own Agent/Adapter layer talks with Germio, so a character's
> true wish can still reach toward a target, with no target field ever
> sitting inside Animo itself. Checked true, 2026-08-19, in a plan talk
> with Master, over a wish to give `stemic` a true, live pair of
> characters. Written in the project writing standard, so a reader whose
> first language is not English can follow it.

---

## Why

`stemic` wants two characters, each with its own true way of being, and
with no fixed side at all — not one friend and one enemy. A first plan
talk held that a character's true wish must stay fully inside itself (no
target at all), since Animo, by true design, never picks *who* an action
is toward. A second look found that rule too tight, and this spec sets
out the fix, plus the full shape of the Adapter that makes it work.

---

## Two kinds of true wish

There are two kinds of wish a character might hold, not one:

1. **A wish held by the player alone** — say, walking up to a fixed point
   to close out a level. Animo's own characters should never hold this
   kind of wish; it calls for a straight path to one fixed spot, which is
   outside what Animo, or this Adapter, takes on.
2. **A wish that comes from a Need itself**, which may well call for a
   target once some other agent (or the player) sits near enough.
   `Socialize` (already real, in `tanukichi.json`) is this kind: a true
   wish, come from `loneliness`, that only lands once carried out
   *toward* someone.

So a character's true wish may hold a target, so long as that target is
picked by a Need, not by an outside goal the player alone should carry.

---

## Why `Needs`/`Action` still hold no field for a target

A check of `Animo/Scripts/Model/Data.cs` found no target field on either
class, by true design:

+ `Needs` holds only `Dictionary<string, float> values` — a name and a
  number, nothing more.
+ `Action` holds only `id`, `need`, `tier`, `exponent` — again, no
  `GameObject`, no position, no name of another agent.
+ `Influence.source`/`Influence.target` are Need names (say, `"fear"` to
  `"confidence"`), not a place in the world or another agent.

This still stands, and this spec does not touch it. **The target itself
is held by the Germio-side Adapter, never by Animo.**

---

## The full call path

```mermaid
flowchart LR
  S["Germio's own Sensor<br/>(see germio TASK-014)"]
  A["Germio-side Adapter<br/>holds the target"]
  E["Animo Engine<br/>Affect(need, delta)"]
  B["Engine.Behavior<br/>e.g. 'Socialize'"]
  M["Adapter moves the<br/>character toward its<br/>held target"]
  S -- "found a target" --> A
  A -- "a plain number only" --> E
  E -- "picks the Behavior" --> B
  B -- "read back" --> A
  A --> M
```

1. Germio's own Sensor (a shared class, spec'd on the `germio` side as
   TASK-014) finds a target near the character — say, the other
   character.
2. The Adapter holds onto that target itself (a plain field on the
   Adapter, not on Animo).
3. The Adapter calls `Engine.Affect(need, delta)` with a plain number
   alone (say, `Affect("loneliness", +5)` while the target stays near,
   each true tick).
4. Animo never learns who or where the target is; it only feels the Need
   climb, runs its own true Step 1-5 pass, and picks a Behavior.
5. Once `Engine.Behavior` reads back as (say) `"Socialize"`, the Adapter
   moves the character toward the target **it**, not Animo, still holds.
6. When the two characters sit close enough, the Adapter calls
   `Affect("loneliness", -30)` (the same shape `animo_spec.md`'s own
   `Socialize succeeds` row already shows), and the Need falls back down.

**Animo picks *what* to do; Germio alone knows *who* or *where*.**

---

## The kind/persona pair for `stemic`

Two kinds, off one shared base (`stemic_character`, with a plain `idle`
and `fatigue` rate), the same shape `tanukichi.json` already shows (a
base kind plus a second kind that gives its own true way of being):

| Kind | High rate | Low rate | Action set |
| --- | --- | --- | --- |
| A wish for new places | `curiosity` | `loneliness` | `Idle`, `Patrol` |
| A wish for company | `loneliness` | `curiosity` | `Idle`, `Socialize` |

A sketch of the shape (values still open, not final):

```json
{
  "schema_version": "1.4",
  "kinds": [
    {
      "kind_id": "stemic_character",
      "rates": { "idle": 0.5, "fatigue": 0.8 },
      "actions": [
        { "id": "Idle", "need": "idle", "tier": 5, "exponent": 1.0 }
      ]
    },
    {
      "kind_id": "place_curious",
      "rates": { "curiosity": 1.2, "loneliness": 0.4 },
      "actions": [
        { "id": "Patrol", "need": "curiosity", "tier": 3, "exponent": 1.3 }
      ]
    },
    {
      "kind_id": "company_seeking",
      "rates": { "loneliness": 1.2, "curiosity": 0.4 },
      "actions": [
        { "id": "Socialize", "need": "loneliness", "tier": 3, "exponent": 1.3 }
      ]
    }
  ]
}
```

Neither kind holds `Flee` or a fight action at all — those call for a
true side (friend/enemy), which this pair does not have.

---

## Not part of this spec

+ Germio's own Sensor class (drop-off check, sight check, the call-in
  point for `DoFixedUpdate.Apply`) — see `germio`'s own TASK-014.
+ `GroupMind` (a Need spreading between more than two agents at once) —
  still a v0.2 idea only, not real code, per `animo_spec.md` §21.4a.

---

## Open points

+ The real number for `loneliness`'s own climb rate while a target sits
  near, and how close "near" truly is, are both still open — a Germio-
  side call, once the Sensor itself (TASK-014) is built and can be
  checked against real play.
+ Whether the Adapter should drop its held target the moment the Sensor
  no longer finds it (a hard cut), or let it fall away over a short true
  span (a soft cut), is still open.
