# 01-toolchain: Verify dual-target prerequisites

Verify SDK selection, installed runtimes, repository configuration, and test execution requirements for both net9.0 and net10.0. Inspect global.json and shared MSBuild configuration, and preserve the current branch and existing package changes. Identify any restore or environment blockers before changing project targets.

**Done when**: SDK/global.json compatibility is verified, .NET 9 and .NET 10 test runtime availability is recorded, and configuration and test prerequisites are documented.
