# Review round 3 — 214-dynamic-workflows

**Date**: 2026-10-11T10:35:00Z
**Verdict**: REVISE

Context: maintainer asked for a self-contained module (SKRAFT = experiment/POC, only in Sources). Rewrite reuses the module 208 `issue-to-code` pipeline. 37 citations VERIFIED; code extracted and run (node --test passes; stub SDK run pauses at plan-approved and replays journaled steps). Requirements 2–4 met; requirement 1 met for concepts, blocked by reproducibility gaps.

## Required fixes
1. U1: cite or reframe "PreToolUse sans matcher s'exécute à chaque appel d'outil" (L266).
2. X1: exercise must not call `ctx.phase` inside `ctx.parallel` thunks (SDK warns against it).
3. R1: Step 6 command must include `--allow-tool` flags; mention `GITHUB_COPILOT_PROMPT_MODE_EXTENSIONS=true` for untrusted folders with the trust warning.
4. R2: clarify which launch path supports pause/resume via `/workflows` + R; don't promise undocumented resume for `copilot workflow run`.
5. Optional: define AI credits; gloss `ctx.pipeline`.
