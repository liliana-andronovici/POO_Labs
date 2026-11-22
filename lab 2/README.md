# Laboratory 2 — Formatters, Sinks, and a Cup of Coffee

Complete C# console project for Part A and Part B, Track 1 (Display Shop).

## Run

Requires the .NET 10 SDK, matching the Lab 1 project in this repository.

```powershell
dotnet run --project .\Lab2.csproj
dotnet run --project .\Lab2.csproj -- .\shop-tuesday.txt
```

With no path argument, the program reads `shop-monday.txt`. A missing or unreadable file produces an `ERROR` log and exits with status 1. Each malformed command produces an `ERROR` with its source line number; processing continues with the next line.

## Files

- `Logger/` contains `LogLevel`, `Formatter`, `ConsoleSink`, `Logger`, and the Lab 1 `ParenthesesValidator` updated for the injected logger.
- `DisplayShop/` contains the display and manager models, command reader, parser, and command executor.
- `Program.cs` wires shared logger dependencies and includes a hard-coded three-display/two-manager example, including assignment of one display reference to both managers and a purchase from one.
- `shop-monday.txt` and `shop-tuesday.txt` are copied from the handout examples.

## Formatter behavior

The format string supports `%t` (time), `%l` (level), `%n` (logger name), `%m` (message), and `%r` (a random phrase selected for the log level). Unknown tokens such as `%x` are preserved. The formatter expands placeholders in the format string once. Thus a message such as `literal %n` passed through `%m` stays `literal %n`; it is never treated as a second format string.

## Case sensitivity

Command keywords are case-insensitive. Thus the handout's `assist carol` command is accepted. Manager names and display model names are case-sensitive ordinal identifiers: `Carol` and `carol` refer to different names. Numbers use invariant-culture notation and must be positive and finite.

## Tuesday file: all problems and decisions

The line numbers below refer to the exact `shop-tuesday.txt` from the handout. The malformed line does not create its intended display, so later commands that use that model fail at runtime.

| Line | Problem | Classification / behavior |
|---:|---|---|
| 4 | `abc` is not a number for the AOC display's PPI | `ERROR`: invalid numeric value; continue |
| 5 | `DISPLAY Asus-PA278QV` is missing its PPI argument | `ERROR`: wrong argument count; continue |
| 6 | Manager `Carol` is created a second time | `ERROR`: duplicate manager name; continue |
| 7 | Manager `Dave` does not exist | `ERROR`: lookup failure; continue |
| 8 | Display `Philips-Evnia` does not exist | `ERROR`: lookup failure; continue |
| 10 | `BenQ-PD2705U` is assigned to `Carol` twice | `ERROR`: duplicate assignment in one manager; continue |
| 11 | `Asus-PA278QV` was not created because line 5 was invalid | `ERROR`: display lookup failure; continue |
| 12 | `SELL` is not a supported command | `ERROR`: unknown command; continue |
| 13 | The command keyword `assist` is lowercase and the manager argument is `carol` | The keyword is accepted because commands are case-insensitive, but `carol` does not match the case-sensitive manager name `Carol`, so this line logs `ERROR` and continues |
| 15 | `BenQ-PD2705U` was already bought on line 14 | `ERROR`: it is no longer assigned to Carol; continue |
| 16 | Carol has no displays left after the purchase | `WARN`: valid `ASSIST`, but nothing is available to compare |
| 17 | `COMPARE` is missing the second model | `ERROR`: wrong argument count; continue |

The duplicate manager is an `ERROR` because the requested creation cannot be completed without silently changing the existing manager. Asking an existing manager with fewer than two displays to assist is a `WARN`: the command is understood and the manager exists, but no comparison can be produced. A successful purchase is `INFO`; when it leaves the manager with zero displays, it also emits `WARN`. Commands that cannot complete due to invalid input, missing objects, bad arity, or an unknown keyword are `ERROR`.

## Composition versus aggregation

`new Logger("Standalone")` uses composition: that logger creates its own formatter and console sink. The three-argument constructor uses aggregation: the caller creates a formatter and sink and passes the same objects to multiple loggers. They can exist independently and are shared. The minimum log level remains static and is shared by every logger.

## Shared display reference and BUY

`AssignDisplay` stores a reference to the display; it does not clone it or enforce exclusive ownership. Two managers can therefore hold the same `Display` object. `BuyDisplay` removes and returns the reference only from the manager receiving the command. It does not search other managers or remove their references. The hard-coded example assigns the Dell display to both Bob and Alice, buys it from Bob, and shows that Bob's list loses the reference while Alice still has it.
