# Technical Terms

> The one place where the technical terms are given their sense.
> The writing standard turns off the simple-word rule only for words in this
> list. If a term is not here, do not use it in a document — add it here first.
> The sense of each term is given in simple words, so a reader whose first
> language is not English, and an agent too, can take it in.

Each entry has a short sense, and where it helps, a note on how the term is
used in real work. This list is tuned to this repository: it keeps the terms
that any repository needs, and adds the terms of the utility AI engine.

---

## Version control (Git)

**repository** — The place where all the files of a project are kept, together
with the full record of their changes over time. Often said in the short form
"repo".

**commit** — To put a set of changes into the record of the repository, as one
step with a note on what changed. Also the name of that one saved step.

**push** — To send the commits made on your own machine up to the shared
repository.

**pull** — To get the newest commits from the shared repository down to your
own machine.

**branch** — A line of commits that goes its own way, apart from the main line.
The main line is often named `master` or `main`.

**PAT** — Short for "personal access token". A secret key that lets a tool act
on a repository in your name, without a password each time.

---

## Language models and agents

**LLM** — Short for "large language model". A model that takes in words and
gives back words. It is not sure or fixed: the same input may give a different
output. For this reason its work is kept to judgment, not to sure steps.

**agent** — An LLM set up to do work on its own: it reads, it makes a choice,
it takes an act, and it goes on by steps toward an end.

**prompt** — The words given to an LLM to set it to work.

---

## Programs and interfaces

**API** — Short for "application programming interface". A fixed way for one
program to ask another program to do something or give data.

**CLI** — Short for "command-line interface". A program run by typing a line,
not by clicking.

**C#** — The language this engine is written in. It runs on the dot-net
platform.

**JSON** — A plain-text way to write down data as names and values. In this
project you write a persona as a JSON file, and the engine reads it.

**Unity** — A program for making games in 3D. The engine can run inside it, or
on its own with no window.

---

## The engine

**agent** (in game) — A thing in a game that acts on its own: an enemy, a
non-player character, anything that needs to want something. Not to be mixed up
with an LLM agent above; here it is the game character the engine drives.

**utility AI** — A way to choose what an agent does by giving each possible act
a score, then taking the act with the top score. No behavior tree, no state
machine.

**need** — One inner drive of an agent, such as hunger or rest. It rises and
falls over time. The set of needs is what the agent cares about.

**Maslow hierarchy** — A known ordering of human needs, from the base ones
(food, safety) up to the higher ones. The engine uses this shape to order an
agent's needs.

**tier** — The level of a need in the Maslow order. Base needs are tier 1; the
higher needs are tier 2 and up.

**suppression** — The way the engine holds down the higher tiers while a base
need is not yet met, so first things come first.

**score** — A number the engine works out for each possible act, from the
current needs. The act with the top score is the one the agent takes.

**influence** — A tie between two needs, where a change in one moves the other.

**cascade** — The run of influences in order, where each one may move the next.

**threshold** — A set point on a need. When the need goes past it, the engine
fires a signal, so the agent can act on it.

**commitment** — A short hold on the agent's current act, with a bonus to its
score, so the agent does not flip from one act to another every frame.

**hot path** — The part of the code that runs very often, every frame. It must
be fast and must make no new memory, or the game slows down.

**allocation** — The making of new memory while the code runs. The hot path
aims for zero allocation.

**zero-alloc** — Short for zero allocation: a path that makes no new memory.

**zero-GC** — A state where the hot path makes no new memory, so the system
that cleans up memory never has to run there. This keeps the frame time even.

**persona** — The JSON file that describes one agent: its needs, and what it
cares about. The engine reads a persona and brings the agent to life.

**Composer** — The part that builds a full persona from its kinds, folding
their fields together in order.

**Validator** — The part that checks a persona file for faults before it runs,
by a set of named rules (A000 and up).

**ScenarioRunner** — A tool that runs a persona through a set path of steps, so
its behavior can be watched and tested without a full game.

---

## Live monitor and networking

**console** — A program with no window of its own, run and read from a text
command line. The live monitor runs the engine as a console program.

**server** — A program that waits for others to connect and then answers them.
The monitor's server sits in the same process as the engine.

**socket** — One end of a live link between two programs, kept open so both can
send at any time.

**WebSocket** — A kind of socket that runs over the web and stays open both
ways, so a page in a browser and a program can send to each other freely.

**endpoint** — The named place a client connects to on a server.

**polling** — A way to get news by asking again and again on a timer, rather
than being told when it happens. Slower to react than a WebSocket.

**effective need** — A need value after the influence cascade has run for the
frame, as against the raw need value before it. The engine chooses actions from
the effective needs.

**live** — Happening now, as the program runs, with no wait and no saved copy.
A live monitor shows the state at the same time the engine works it out.

**monitor** — A tool that watches a running program and shows its state as it
changes, so a person can follow along and step in.

---

**mock** — A stand-in object used in a test in place of a real one, so the test
can drive the code under known, made-up conditions.

**snapshot** — A read-only copy of a thing's whole state at one moment, taken so
it can be read or sent on without holding up the thing itself.

**socket** — One end of a live two-way link between two programs over a network.

**port** — A numbered door on a machine that a network link connects to.

**token** — A small value passed along to stand for a right, a place, or a
request to stop.

**buffer** — A block of memory held aside to take in bytes as they arrive.

**serve** — To answer requests that come in over a network link.

**node** — A single point in a graph or tree, joined to others by edges.

**region** — A named area of a larger space, marked off for its own handling.

**master** — The one person this tool is built for and works with.

**task** — One piece of work, with its own id, on a `TASKLIST.md`.

**TDD** — Short for "test-driven development": write a failing test
first, then write the code that makes it pass.

**Publish** — To send an event out to every part of the code that
listens for it.

**injection** — The act of putting one piece of code or data inside
another, at run time.

**garbage** — Memory a program no longer needs, cleared out
later by the run time.

**FPS** — Short for "frames a second": how many pictures a game
draws on the screen in one second.

**leak** — Memory a program keeps holding on to by mistake, long
after it is done with it.

## How to keep this list

+ One term, one sense. Give the sense in one place only — here.
+ Keep the sense in simple words, by the writing standard.
+ Add a term **before** it is first used in any document.
+ When a term is no longer used anywhere, it may be taken out.

**byte / bytes** — A small block of computer memory, eight bits wide. A MIDI
message is written as a short run of eight-bit values.

**garbage collector** — The part of a managed language that clears out heap
memory no longer in use, on its own, with no word from the program itself.

**heap** — The part of computer memory a program asks for, and gives back,
while it runs. A managed language's own "garbage collector" clears out heap
memory no longer in use.

**tick / ticks** — One small step forward, in time. A clock or a sequencer moves
forward, one tick at a time.

**generational** — A way a garbage collector may sort held room by how new
it is, so it can sweep the newest, most short-lived room first, and fast.

**closure** — A method, built on the spot, that reads a value held outside
itself — the value it reads is held, together with the method, as one,
single, new object.

**sequencer** — A program that reads a song's own score, and sends out a
note, at the right moment, to make music.

**netstandard** — A shared, true .NET target that lets one build run
true across many true, different engines (Unity included), given no
single one's own newer feature.

**csproj** — A project file, given to .NET's own build tool, naming
what to build and what it needs.

**adapter** — A true, given layer that lets one part of a system
talk to another, apart part, with no change owed to either one's
own true inner work.
