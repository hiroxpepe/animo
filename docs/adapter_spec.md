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
is held by the Adapter, never by Animo.**

**Where the Adapter itself sits (Master's own word, 2026-08-19):** with
`Human_Animable`, on the `stemic` side, for as long as this stays a PoC.
The two are one true piece of work split in two — moving one alone to
`germio` would mean reaching across two separate places for every small
design fix, while the design is still finding its own shape. Once the
PoC shows this holds true for any game, both move to `germio` together
(the same true way `SoundSystem` and `Sensor` already sit there).
`Sensor` itself is the one part that belongs in `germio` from the
start, since a character with no Animo mind at all still uses it.

---

## The full call path

```mermaid
flowchart LR
  N["Animo Needs<br/>climb on their own<br/>(persona rates)"]
  B["Engine.Behavior<br/>e.g. 'Socialize'"]
  A["Adapter<br/>(stemic side, for now)<br/>holds the target"]
  S["Germio's own Sensor<br/>(see germio TASK-014)"]
  M["Adapter moves the<br/>character toward its<br/>held target"]
  F["Adapter calls<br/>Affect(need, -N)<br/>once it truly lands"]
  N -- "no outside push at all" --> B
  B -- "read back" --> A
  A -- "asks for a target" --> S
  S -- "found one" --> A
  A --> M
  M -- "got there" --> F
  F -- "the Need falls" --> N
```

1. A Need climbs on its own, with no outside push at all, at whatever
   rate the persona's own `rates` sets (say, `loneliness` at +1.2 a
   second, `curiosity` at +0.8). **This is Animo's own true work, and
   the Adapter never pushes a Need up.**
2. Animo runs its own true Step 1-5 pass and picks a Behavior, say
   `"Socialize"` or `"Patrol"`.
3. The Adapter reads `Engine.Behavior` back, and asks Germio's own
   Sensor (a shared class, spec'd on the `germio` side as TASK-014) for
   whatever that Behavior calls for — another character, for
   `"Socialize"`; a block or step not yet stood on, for `"Patrol"`.
4. The Adapter holds onto that target itself (a plain field on the
   Adapter, not on Animo), and moves the character toward it.
5. Animo never learns who or where the target is; it only ever feels
   its own Needs, and picks what to do.
6. Once the character truly gets there (close enough to the other
   character, or standing on the new block), the Adapter calls
   `Affect("loneliness", -30)` — or `Affect("curiosity", -N)` — and the
   Need falls back down. **This one call, telling Animo an action
   truly landed, is the Adapter's own only true reach into Animo.**

**Animo picks *what* to do; Germio alone knows *who* or *where*.**

Why the Adapter must make that last call at all: `Action` holds only
`id`, `need`, `tier`, `exponent` (checked true in
`Scripts/Model/Data.cs`) — no field at all for "how far this Need falls
once the action lands". Animo, holding no place in the world, can never
know on its own whether a character truly got anywhere. Germio alone
knows, so Germio alone can tell it.

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
