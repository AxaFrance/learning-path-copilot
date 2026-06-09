# Lint round 1 — 02-prompts-personnalises

**Verdict**: ❌ FAIL (4/9)

## Failures (8)

- L14 — check 3: `## Pourquoi ce module` missing. Found non-standard `## Objectif` instead.
- L23 — check 3: `## Pré-requis` missing. Found non-standard `## Ce que tu vas apprendre` instead. Pré-requis merged into header L10, not a standalone section.
- L31 — check 3: `## Concepts clés` missing. Not present anywhere.
- L31 — check 3: `## Démonstration` missing. Found non-standard `## Contenu pédagogique` instead.
- L114 — check 3: `## Exercice` present but missing difficulty level indicator (⭐ / ⭐⭐ / ⭐⭐⭐).
- L10 — check 7: ⭐ emoji used outside Exercice section (in `**Complexité** : ⭐` header line).
- L107–L109 — check 7: ✅ and ❌ emoji used in comparison table (outside Exercice section).
- L156, L161 — check 9: cross-link `../03-skills.md` resolves to `docs/_drafts/03-skills.md` — not found. Cross-link `./03-skills.md` resolves to `docs/_drafts/learning-path/03-skills.md` — not found.

## Warnings (3)

- L10 — check 2: Duration line uses `**Durée** : ~30 min` instead of template pattern `Durée estimée : … min`.
- L10 — check 9: cross-link `../01-instructions-personnalisees.md` resolves to `docs/_drafts/01-instructions-personnalisees.md` — not found. Published version exists at `docs/learning-path/01-instructions-personnalisees.md` but relative path from `_drafts/learning-path/` doesn't reach it.
- L1–L161 — check 4: estimated word count ~700–750, below the 800-word minimum.
