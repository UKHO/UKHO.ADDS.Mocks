# .NET 9 and .NET 10 Multi-Targeting

## Preferences
- **Flow Mode**: Automatic
- **Target Frameworks**: net9.0;net10.0 for all projects, including tests. Retain .NET 9 support.
- **Assessment Target**: net10.0; assessment findings must be evaluated against continued net9.0 compatibility.

## Source Control
- **Source Branch**: feature/310606-dotnet-10-upgrade
- **Working Branch**: feature/310606-dotnet-10-upgrade
- **Commit Strategy**: Manual; leave changes uncommitted.
- **Branch Sync**: Disabled
- Preserve existing edits in src/UKHO.ADDS.Mocks/UKHO.ADDS.Mocks.csproj: UKHO.ADDS.Infrastructure.Results and UKHO.ADDS.Infrastructure.Serialization at 0.0.60930-alpha.7.

## User Preferences
### Technical Preferences
- User confirmed both net9.0;net10.0, not replacement with net10.0 only.
- Focus on target-framework support; avoid unrelated modernization.

## Execution Constraints
- No agent-spawning tool is available. Follow sub-agent processes inline.

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once (user confirmed)

### Project Structure
- Package Management: Per-Project (defer CPM to post-migration)

### Compatibility
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: All-at-Once
**Rationale**: Seven SDK-style projects require the same permanent dual-target configuration and coordinated validation.

### Execution Constraints
- Update all seven projects together; retain net9.0;net10.0 permanently.
- Validate solution builds and tests independently on both target frameworks.
- Keep compatible dependencies; change only what dual-target support or security requires.
- No unrelated Aspire major-version migration or language modernization.
- Preserve manual commits and existing infrastructure package edits as explicitly confirmed.
