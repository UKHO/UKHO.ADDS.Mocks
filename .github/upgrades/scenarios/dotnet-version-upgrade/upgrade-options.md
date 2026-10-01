# Upgrade Options — ADDS-Mock

Assessment: Seven SDK-style projects on net9.0; user requires permanent net9.0;net10.0 multi-targeting. Package recommendations and four source-compatibility findings require validation.

The requested targets remain **net9.0;net10.0 for every project**, including tests and Aspire LocalHost. Preserve existing infrastructure package edits. Keep the current branch, manual commits, and automatic execution after confirmation. Do not perform an unrelated Aspire major-version migration or broad package refresh; investigate compatibility and request input if maintaining both targets is blocked.

## Strategy

### Upgrade Strategy
All seven projects need the same dual-target configuration and full-solution validation.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Apply dual-target support across the solution in one coordinated pass, then validate both targets. |
| Top-Down | Update entry-point applications first, then shared libraries, retaining both required targets throughout. |

## Project Structure

### Package Management
Projects use per-project package references and will retain two target frameworks permanently.

| Value | Description |
|-------|-------------|
| **Per-Project (defer CPM to post-migration)** (selected) | Retain package versions in project files; use target-specific references only where required for compatibility. |
| Central Package Management (CPM) | Move package versions to Directory.Packages.props as an additional change. |

## Compatibility

### Unsupported API Handling
Assessment flags four source-incompatible API usages to verify during compilation for both frameworks.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve verified incompatibilities in the same task, preserving behavior on both target frameworks. |
| Defer Complex Changes | Apply simple fixes immediately and create tracked follow-up tasks with temporary stubs for complex replacements. |

Validation will build and test net9.0 and net10.0 independently, including checking Aspire launch/test behavior with multi-targeted projects. Package recommendations are not proof that an update is required; retain compatible dependencies and change only what dual-target support or security requires.
