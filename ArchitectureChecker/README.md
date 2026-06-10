# Architecture Checker

WinForms tool for checking architecture boundaries in the FTMachines solution.

## Architecture Intent

The intended dependency direction is:

```text
Common / Hardware / SubTask
        ^
        |
Machines: FiberCoupling, LBT4500, LDT6110, ...
```

Base and shared projects may be used by machine projects, but they should not depend back on machine application projects.

`LBT4500/Scopes/**` is the composition and adapter layer. It is allowed to connect application-specific objects, but task and module code should not depend on Scope directly.

## Current Rules

### LBT4500

- `LBT4500/Modules/**/*.cs` must not depend on `LBT4500.Scopes` or `Scope.`
- `LBT4500/SubTasks/**/*.cs` must not depend on `LBT4500.Scopes` or `Scope.`
- `LBT4500/Forms/**/*.cs` depending on Scope is reported as a warning.
- `LBT4500/Modules/**/*.cs` depending on `System.Windows.Forms` is reported as a warning.

### Common / Hardware / SubTask

- Common projects must not depend on `LBT4500`.
- Hardware projects must not depend on `LBT4500`.
- SubTask project must not depend on `LBT4500` or `LBT4500.Scopes`.
- Hardware projects depending on `FTClient` customer selection are reported as a warning.

### Machine Reverse Dependency

Common, Hardware, and SubTask must not depend on these machine application projects:

- `FiberCoupling`
- `FiberCoupling_Configurator`
- `FTMachineBase`
- `FTMachineExample`
- `LBT4500`
- `LBT4500_Configurator`
- `LDT6110`
- `LDT6110_Configurator`

This protects shared layers from becoming tied to a specific machine implementation.

## Usage

1. Build `ArchitectureChecker`.
2. Run the tool.
3. Select the solution root folder containing `FTMachines.sln` and `LBT4500`.
4. Click `Run Check`.

Results:

- `OK`: no rule findings.
- `OK with`: warnings only.
- `NG`: at least one error.

## Rule Files

Rules are loaded from every `*.json` file under the executable folder's `Rules` directory.

Current rule files:

- `Rules/lbt4500.json`
- `Rules/common.json`
- `Rules/hardware.json`
- `Rules/subtask.json`
- `Rules/machine.json`

Each rule defines:

- `Include`: file patterns to scan.
- `Exclude`: file patterns to skip.
- `Forbidden`: text patterns that should not appear in matching files.
- `Severity`: `error` or `warning`.
