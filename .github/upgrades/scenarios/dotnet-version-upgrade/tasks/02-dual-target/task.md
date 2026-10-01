# 02-dual-target: Add both target frameworks across the solution

Update all seven projects together to net9.0;net10.0. Retain per-project packages and compatible APIs, fixing only verified incompatibilities or security issues. Assessment flags four source-compatibility findings and package recommendations; investigate these against actual builds rather than blindly applying upgrades that could break .NET 9.

Inspect Aspire LocalHost project metadata, functional test launch behavior, framework-specific packages, and scripts that assume a single target. Preserve infrastructure packages at 0.0.60930-alpha.7. Do not perform unrelated language, test framework, or Aspire major-version migrations.

**Done when**: All seven projects evaluate to both requested targets, solution builds without errors or warnings for both targets, affected tests pass for both targets, and pre-existing package edits remain intact. Report environment blockers explicitly rather than claiming successful validation.
