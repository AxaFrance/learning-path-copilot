# Review round 4 — 214-dynamic-workflows

**Date**: 2026-10-11T10:40Z
**Verdict**: REVISE (last allowed round → resolved by parent thread with human-maintainer mandate)

47 citations VERIFIED (incl. new Copilot CLI programmatic reference). Code consistent; node --test 2/2; pause/resume simulation spawns each agent once. Maintainer reqs 1 (SKRAFT only in Sources), 2, 3 met; 4 met with undocumented deltas.

## Findings and resolution (applied by parent thread)
1. U1 (blocking): agents in `.agents/` are not loaded by Copilot CLI (`~/.copilot/agents/`, `.github/agents/`, `.claude/agents/`). → Demo tree, step-1 comment and exercise moved to `.github/agents/`, with a sourced citation of the CLI command reference.
2. U2/P1: module 210 covers `gh copilot`, not the agentic `copilot` CLI. → Pré-requis and Pour aller plus loin reworded; install doc linked.
3. U3: `githubRepo` → MCP `github` mapping unsourced. → Hedged: verify with `/agent` which tool the agent gets and adjust.
4. P2: "thunk" defined on first use.
5. Editorial `<!-- round N response -->` comments removed.
6. 208 deltas (spec-writer reads the issue itself; summary step becomes the workflow return value) now stated explicitly.
7. Length above target — accepted (code-heavy, self-contained by maintainer request).
