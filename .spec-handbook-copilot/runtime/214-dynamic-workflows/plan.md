---
slug: 214-dynamic-workflows
topic: "Dynamic workflows de GitHub Copilot — module 214 « Dynamic workflows — orchestrer en code », Bloc 02 — Composition"
round: 4
stage: done
status: CERTIFIED-WITH-HUMAN-FIXES
created_at: 2026-10-11T09:50:49Z
---

# Plan — module 214-dynamic-workflows

## Sujet (verbatim utilisateur, résumé des contraintes imposées)

Construis un chapitre handbook sur les « Dynamic workflows » de GitHub Copilot.

Contraintes de placement imposées par l'utilisateur (non négociables) :

- Module 214 — titre « 214 — Dynamic workflows — orchestrer en code », Bloc 02 — Composition.
- Fichier final : `docs/02-composition/214-dynamic-workflows.md` (remplace la cible par défaut `docs/learning-path/<bloc>/<slug>.md`).
- Front-matter dans le style de `docs/02-composition/208-workflows.md` (id: dynamic-workflows, title, sidebar_position: 214, description). Durée estimée en tête.
- Sections : Pourquoi ce module, Pré-requis, Concepts clés, Démonstration, Pièges et anti-patterns, Exercice ⭐⭐, Validation, Pour aller plus loin, Sources, Module suivant.
- Pré-requis : 104, 105, 208, 210.
- Module suivant : `../03-ingenierie-de-contexte/310-evals.md`.
- Ne modifier aucun autre fichier sous `docs/`.
- Sources obligatoires, fetchées réellement : docs.github.com (concept + how-to dynamic workflows), copilot-sdk `nodejs/docs/workflows.md`, dépôt SebastienDegodez/skraft-plugin branche `integration/v2` (ADR-010, commits 07214d4e et e21ffbc4, pipeline-definition.mjs, extension.mjs, docs/site/fr/explanation/hooks.md).
- Pousser jusqu'à CERTIFY, 3 rounds max.

## Constats de l'orchestrateur au bootstrap

- Aucun état préexistant pour ce slug (pas de reprise).
- `docs/02-composition/213-apm-vs-plugins.md` n'a **pas** de section « Module suivant ». Le module frère `208-workflows.md` pointe vers `[310 — Tester ses primitives (evals binaires)](../03-ingenierie-de-contexte/310-evals.md)` : c'est ce libellé qui sera transmis au writer.

## Historique

- 2026-10-11T09:50:49Z — bootstrap (round 0, stage architect).
- 2026-10-11T09:50:49Z — BLOQUÉ avant la Phase 2 : la session d'exécution n'expose pas d'outil de spawn de sous-agent (`agent/runSubagent` requis par la frontmatter de l'orchestrateur et le design packet), ni de binaire `copilot`, ni les outils GitHub MCP demandés pour le dépôt skraft-plugin. L'architecte, le writer et le reviewer (fresh context) ne peuvent pas être invoqués. Escalade humaine.
- 2026-10-11T10:00:00Z — Déblocage : le thread parent invoque directement architect → writer → reviewer. placement.md enregistré (INSERT, docs/02-composition/214-dynamic-workflows.md). Corpus pré-téléchargé dans /tmp/dw-sources/.
- round 1 review: REVISE (2 UNSUPPORTED, 4 UNCITED, 3 PEDAGOGICAL-GAP, E1).
- round 2 review: CERTIFY. Polish appliqué par le parent (commentaires de round retirés, « quinzaine » de ports, ADR-010 Proposed, glose DecisionStore). Commits 07214d4e/e21ffbc4 vérifiés via GitHub MCP par le parent.
- 2026-10-11 — Retour mainteneur : module autonome, SKRAFT seulement en Sources. Round 3 réécriture (démo sur issue-to-code du 208) → review REVISE (U1, X1, R1, R2).
- round 4 review: REVISE (U1 .agents/ location, U2, U3, P1, P2). Rounds exhausted; fixes applied by parent thread per maintainer mandate (self-contained module). Jekyll build OK.
