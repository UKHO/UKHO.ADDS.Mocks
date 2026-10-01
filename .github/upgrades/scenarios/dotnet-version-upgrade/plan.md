# .NET 9 and .NET 10 Multi-Targeting Plan

## Overview

**Target**: Permanent net9.0;net10.0 support across all seven projects.
**Scope**: Seven SDK-style projects; retain compatible packages and existing user edits.

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: Seven projects currently on .NET 9 require coordinated dual-target configuration, not a staged framework replacement.

Projects in scope:
- src/UKHO.ADDS.Mocks/UKHO.ADDS.Mocks.csproj
- src/UKHO.ADDS.Mocks.SampleService/UKHO.ADDS.Mocks.SampleService.csproj
- src/UKHO.ADDS.Mocks.Client/UKHO.ADDS.Mocks.Client.csproj
- src/UKHO.ADDS.Mocks.LocalHost/UKHO.ADDS.Mocks.LocalHost.csproj
- src/UnitTestExamples/UnitTestExamples.csproj
- tests/UKHO.ADDS.Mocks.Tests/UKHO.ADDS.Mocks.Tests.csproj
- tests/UKHO.ADDS.Mocks.Functional.Tests/UKHO.ADDS.Mocks.Functional.Tests.csproj

## Tasks

### 01-toolchain: Verify dual-target prerequisites

Verify SDK selection, installed runtimes, repository configuration, and test execution requirements for both net9.0 and net10.0. Inspect global.json and shared MSBuild configuration, and preserve the current branch and existing package changes. Identify any restore or environment blockers before changing project targets.

**Done when**: SDK/global.json compatibility is verified, .NET 9 and .NET 10 test runtime availability is recorded, and configuration and test prerequisites are documented.

---

### 02-dual-target: Add both target frameworks across the solution

Update all seven projects together to net9.0;net10.0. Retain per-project packages and compatible APIs, fixing only verified incompatibilities or security issues. Assessment flags four source-compatibility findings and package recommendations; investigate these against actual builds rather than blindly applying upgrades that could break .NET 9.

Inspect Aspire LocalHost project metadata, functional test launch behavior, framework-specific packages, and scripts that assume a single target. Preserve infrastructure packages at 0.0.60930-alpha.7. Do not perform unrelated language, test framework, or Aspire major-version migrations.

**Done when**: All seven projects evaluate to both requested targets, solution builds without errors or warnings for both targets, affected tests pass for both targets, and pre-existing package edits remain intact. Report environment blockers explicitly rather than claiming successful validation.

---

### 03-validation: Validate the complete dual-target solution

Validate full solution builds and test suites for net9.0 and net10.0, including Aspire-backed functional tests where the environment supports them. Review the final diff for unintended changes, and document framework selection for run/publish commands if needed. Leave changes uncommitted.

Retain per-project package management for the permanent multi-target setup. Central package management is only a future optional recommendation after dependency-version alignment; do not claim the solution has consolidated to one TFM.

**Done when**: Both framework builds and all test results are recorded, verified blockers are reported, framework-specific launch behavior has been checked, and the final diff preserves user changes without unrelated modernization.
