# Naming Standard

> How to spell the words inside code names, in this project and in the other
> projects too. This is a shared rule. All projects by this maker are to keep
> it. The convention tests read the code and check these rules by machine.

---

## The one rule: print form

A word inside a code name is spelled the way a printed page spells it — a
technical magazine, a data sheet, the label on a piece of gear. If you are not
sure how to spell a word in a name, picture how it looks in print, and follow
that.

This one rule is the base. Everything below is only the print rule worked out
for the three cases that come up most.

The rule holds for **every name we make**: types, methods, properties, fields,
locals, and the **file name** too. A file holds a type, so the file name
carries the same word in the same print form — a type named `JSON` lives in a
file named `JSON.cs`, not `Json.cs`.

---

## Short forms: write the word in full

Print does not cut a word short. It writes "Message", not "Msg"; "Button", not
"Btn". So a code name writes the word in full too.

Some words that must be written in full:

+ `Message`, not `Msg`
+ `Button`, not `Btn`
+ `Config`, not `Cfg`
+ `Index`, not `Idx`
+ `Parameter`, not `Param`
+ `Initialize`, not `Init`
+ `Calculate`, not `Calc`

A word is a short form when print would not cut it. A few short forms have
become words of their own in print (for example "sync" or "info"); those follow
print, which is the one rule again.

---

## Letter words (acronyms): all caps

When print writes a name as all capital letters, the code name does too. Print
writes "ID card" and "user ID", so the code writes `ID`, not `Id`. This holds
for two-letter forms as well as longer ones.

Some letter words that are all caps:

+ `ID`, `IO`, `UI`, `DB`
+ `API`, `URL`, `JSON`, `HTTP`, `CPU`, `CSV`
+ `DOM`, `HTML`, `CSS`
+ `LFO`, `FX`, `PCM`, `FM`, `VA`

This is the point where this rule parts from the Microsoft guideline, which
writes a long letter word with one capital only (`Json`, `Http`). This project
follows print instead: a reader sees the letter word stand out, which is easier
on the eye.

---

## Unit marks: the print form of the unit

A unit of measure keeps its own print form, which is not all caps. Print writes
"440 Hz", not "440 HZ". So the code writes `Hz`, not `HZ`. The unit rule sits
above the letter-word rule, because a unit mark has one fixed print form the
whole world shares.

Some unit marks and their print form:

+ `Hz` (not `HZ`), `kHz` (not `KHZ`)
+ `dB` (not `DB`)
+ `ms`, `Hz`, `kHz`

---

## How the tests use this

The convention tests hold two small word lists that put this standard to work:

+ **the full-word list** turns a short form into its full word (`Msg` →
  `Message`).
+ **the all-caps list** turns a letter word into all caps (`Api` → `API`).

Each project adds to these lists the short forms and letter words that show up
in its own code, judged by the print rule. The lists are the only part that
changes from project to project; the print rule itself does not change.
