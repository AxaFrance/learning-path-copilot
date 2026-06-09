# Review round 1 — pipeline-agents-handbook

**Date**: 2026-05-28
**Verdict**: CERTIFY

## Summary
The draft is dense with verbatim citations and every one of the 17 cited claims re-fetches cleanly to the source with the surrounding context supporting the assertion made. The two Mermaid diagrams are faithful to the orchestrator's Phase 1–4 procedure (single-writer interlock, FORCED spawns, bounded loop, REVISE/REJECT branching, HUMAN CHECKPOINT exit). Word count (2733) sits inside the placement target (2500–3500), prerequisites are leveraged (208 outside-in pattern explicitly invoked, 103/104 cited as further reading), and no claim is sourced from training recall.

## Audit table

| ID | Type | Section | Claim (résumé FR) | Source cited | Status | Note |
|----|------|---------|-------------------|--------------|--------|------|
| C1 | citation | ## Pourquoi ce module | "un agent racine pilote le flux et délègue chaque étape à un sous-agent spécialisé via `runSubagent`" | `docs/learning-path/02-composition/208-workflows.md` | VERIFIED | Exact verbatim line 18 |
| C2 | citation | ### Topologie | "You are the only entry point — the user must not invoke `handbook-chapter-architect`, `handbook-chapter-writer`, or `handbook-chapter-reviewer` directly." | `.github/agents/handbook-chapter-orchestrator.agent.md` | VERIFIED | Verbatim under "Invocation mode" |
| C3 | citation | ### Topologie | "orchestrator | Séquencer + persister état | Ne fetch pas, ne rédige pas" | `.spec-handbook-copilot/2026-05-27-initial-design/plan.md` | VERIFIED | Line 273 verbatim |
| C4 | citation | ### Single-writer interlock | "You are the **only** writer under `.spec-handbook-copilot/runtime/<slug>/`. Sub-agents return artifacts as their final message; you persist them to disk yourself." | orchestrator.agent.md | VERIFIED | Verbatim |
| C5 | citation | ### Single-writer interlock | "return your placement plan as your final message. The orchestrator persists it ... You do not write files yourself." | architect.agent.md | VERIFIED | Verbatim (ellipsis preserves meaning) |
| C6 | citation | ### Single-writer interlock | "Writes allowed: `docs/learning-path/<bloc>/<slug>.md` (create or overwrite only)." | writer.agent.md | VERIFIED | Verbatim under Locked scope |
| C7 | citation | ### FRESH CONTEXT | "You start cold. You MUST NOT read: `review-round-{N-1}.md` or any prior review of this slug." | reviewer.agent.md | VERIFIED | Verbatim (formatted as bullet in source, joined here without distortion) |
| C8 | citation | ### FRESH CONTEXT | "Warm-context review is the failure mode this agent exists to prevent." | reviewer.agent.md | VERIFIED | Verbatim |
| C9 | citation | ### FRESH CONTEXT | "Re-fetch the cited URL or path ... Use a fresh tool call; do not trust the writer's quoted snippet without re-fetching." | reviewer.agent.md | VERIFIED | Verbatim Phase 2 step 1 |
| C10 | citation | ### Boucle bornée | "Writer ↔ Reviewer alignment loop (bounded, max 3 rounds) ... For each round N from 1 to 3" | orchestrator.agent.md | VERIFIED | Verbatim Phase 3 heading + loop opening |
| C11 | citation | ### Boucle bornée | "`REJECT` -> exit loop immediately. Go to Phase 4 REJECT path." | orchestrator.agent.md | VERIFIED | Verbatim Phase 3 step 5 |
| C12 | citation | ### Boucle bornée | "PROSE Safety Boundaries | OK (S4 entre stages, B10 sortie de boucle)" | design plan.md | VERIFIED | Line 302 verbatim |
| C13 | citation | ### Étape 1 | "Slug : `pipeline-agents-handbook` ... Stage courant : `architect` ... Round : 0" | `.spec-handbook-copilot/runtime/pipeline-agents-handbook/plan.md` | VERIFIED | All three fields verbatim in the runtime plan |
| C13b | implicit | ### Étape 1 | Verbatim user prompt (« On va construire dans ressources … vrai exemple de workflow ») | runtime plan.md | VERIFIED | Quote matches plan.md "Sujet (verbatim utilisateur)" with explicit ellipsis |
| C14 | citation | ### Étape 2 | "**Verdict**: INSERT ... Inséré en fin du bloc `02-composition`, après `210-copilot-cli.md`. Slug `pipeline-agents-handbook`, numéro proposé `211`." | runtime placement.md | VERIFIED | Verbatim |
| C15 | citation | ### Étape 2 | "Any request to edit a module or to draft chapter content -> refusal: 'Out of scope. I only emit placement plans. Use `handbook-chapter-writer` to draft.'" | architect.agent.md | VERIFIED | Verbatim under Locked scope |
| C16 | citation | ### Étape 3 | "No claim ships without a fetched source. Training recall is permitted to suggest what to look up — never to assert a fact." | writer.agent.md | VERIFIED | Verbatim Grounding contract |
| C17 | citation | ### Étape 4 | "0 `UNSUPPORTED` AND 0 `CHERRY-PICKED` AND 0 `UNCITED` AND ≤ 2 `PEDAGOGICAL-GAP` AND ≤ 1 `OUTDATED` -> CERTIFY." | reviewer.agent.md | VERIFIED | Verbatim Phase 5 |
| D1 | diagram | ### Diagramme séquence | Orchestrator writes plan.md, spawns architect FORCED, persists placement, loops N=1..3 with writer→reviewer, branches on CERTIFY/REVISE/REJECT, HUMAN CHECKPOINT on N==3 or REJECT | orchestrator.agent.md Phases 1–4 | VERIFIED | Sequence matches step-for-step |
| D2 | diagram | ### Machine à états | Architect→Done_Reject on REJECT, Architect→WriteRound on INSERT/REPLACE/NEW-TRACK, Review→Human_Checkpoint on REVISE&N==3 or REJECT | orchestrator.agent.md Phase 2 gate + Phase 3 step 5 | VERIFIED | All transitions match the spec |
| P1 | pedagogy | ### Boucle bornée | Tag "B10 HUMAN CHECKPOINT" used without defining what `B10` denotes | placement prereqs (103/104/207/208) | PEDAGOGICAL-GAP (mild) | "HUMAN CHECKPOINT" is self-explanatory; B10 is a Genesis label that the reader hits without prior introduction — borderline but acceptable in context |

## Required fixes (REVISE) or blocking findings (REJECT)
None blocking. Per Phase 5 thresholds: 0 UNSUPPORTED, 0 CHERRY-PICKED, 0 UNCITED (every assertive sentence under "est/permet/fonctionne/supporte/nécessite/ne peut pas" sits inside a paragraph that carries a citation block or is verifiable narrative restating a cited artifact), ≤ 1 PEDAGOGICAL-GAP (P1, mild), 0 OUTDATED. CERTIFY conditions met.

Optional polish (non-blocking, writer may ignore):
- The `B10` label in "B10 HUMAN CHECKPOINT" could be glossed in one sentence (e.g. "B10 = gate de sortie humaine du framework Genesis") to remove the only mild pedagogical bump.
- The paraphrased orchestrator-to-architect task description ("plan placement for slug `pipeline-agents-handbook`") is a soft simplification of the actual instruction ("Plan placement for slug `<slug>`, topic `<topic>`. Read `.spec-handbook-copilot/runtime/<slug>/plan.md` first."). Not misleading, but a verbatim quote would tighten it.
