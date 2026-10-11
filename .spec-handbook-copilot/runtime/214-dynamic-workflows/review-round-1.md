# Review round 1 — 214-dynamic-workflows

**Date**: 2026-10-11T10:05:00Z
**Verdict**: REVISE

## Summary
51 citation blocks re-checked: 49 VERIFIED, 2 UNSUPPORTED (S1 invented "mesuré à grande échelle"; C13 non-verbatim "GitHub Copilot app"). Claude Code incompatibility stated as fact instead of deduction (E1). 4 UNCITED (U1–U4), 3 PEDAGOGICAL-GAP (mod Claude Code, port/adaptateur, canvas). All 8 internal links resolve; SDK snippets match documented signatures; "hook node lent" correctly labelled as field experience.

## Required fixes
1. S1: remove "a mesuré ce coût à grande échelle".
2. C13: quote verbatim "Dynamic workflows are available in the Copilot app, Copilot CLI, and Copilot SDK." + plans sentence.
3. E1: frame Claude Code incompatibility as explicit deduction (official availability list + separate mod); "a dû écrire" → "compose".
4. U1/U2: refer back to the deduction instead of restating as fact.
5. U3/U4: add ADR-010 "unit-tested with in-memory doubles" and gh-howto "Start a new session…" sources.
6. C33: "ne sont pas ré-exécutées : leur résultat est relu depuis le journal".
7. C39: attribute "Avant" column to ADR-010, "Après" to hooks.md.
8. P1–P3: define mod Claude Code, port/adaptateur, canvas.
9. Optional: align pseudo-code agentRunner.run args (agent/label); mark hooks snippet as jsonc.
