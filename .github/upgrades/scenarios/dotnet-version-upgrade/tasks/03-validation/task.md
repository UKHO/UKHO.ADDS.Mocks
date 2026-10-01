# 03-validation: Validate the complete dual-target solution

Validate full solution builds and test suites for net9.0 and net10.0, including Aspire-backed functional tests where the environment supports them. Review the final diff for unintended changes, and document framework selection for run/publish commands if needed. Leave changes uncommitted.

Retain per-project package management for the permanent multi-target setup. Central package management is only a future optional recommendation after dependency-version alignment; do not claim the solution has consolidated to one TFM.

**Done when**: Both framework builds and all test results are recorded, verified blockers are reported, framework-specific launch behavior has been checked, and the final diff preserves user changes without unrelated modernization.
