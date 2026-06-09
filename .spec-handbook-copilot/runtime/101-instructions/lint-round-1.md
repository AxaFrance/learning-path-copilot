# Lint — docs/_drafts/learning-path/01-instructions-personnalisees.md (Round 1)

**Verdict global** : PASS (avec warnings non bloquants)
**Score** : 9 / 9
**Références** : spec 01 (gabarit) + spec 11 (conventions)

---

## Per-criterion verdict

| # | Critère | Verdict |
|---|---|---|
| 1 | Frontmatter Docusaurus (id/title/sidebar_position/description ≤ 160) | PASS |
| 2 | En-tête (Durée · Complexité · Pré-requis + accroche) | PASS |
| 3 | Sections obligatoires dans l'ordre spec 01 §3 | PASS |
| 4 | Budget mots (⭐⭐ → 700–1200) ≈ 880 | PASS |
| 5 | Mermaid ≤ 12 nœuds | PASS (N/A) |
| 6 | Blocs code typés (3 × ```diff) | PASS |
| 7 | Pas d'emoji hors étoile/coche/croix | PASS |
| 8 | Français + tutoiement + voix active + anglicismes italisés 1re occ. | PASS |
| 9 | Liens internes valides | WARNING (résolution post-promotion uniquement) |

---

## Failures

Aucun.

## Warnings (3, regroupés sur check 9)

### W1 — L11 — lien `./00-setup-posture.md`
Depuis `docs/_drafts/learning-path/` la cible n'existe pas ; elle existe à `docs/learning-path/00-setup-posture.md`. Résolution correcte uniquement après promotion du brouillon.

### W2 — L117 — lien `../ressources/glossaire-fr.md`
Depuis `_drafts/learning-path/` résout vers `docs/_drafts/ressources/glossaire-fr.md` (inexistant). Cible réelle `docs/ressources/glossaire-fr.md` — OK post-promotion.

### W3 — L119 — lien `./02-prompts-personnalises.md`
Module 02 non encore rédigé. Tolérable si publication coordonnée ; sinon annoter « à venir ».

---

## Metrics

- Mots corps : ≈ 880 (cible ⭐⭐ 700–1200)
- Sections : 7/7 dans l'ordre canonique
- Diffs git : 3 (instruction globale, instruction scopée applyTo, avant/après prompt)
- Anglicismes italisés 1re occurrence : prompt, instruction, frontmatter, applyTo
- Exercice : énoncé + 6 étapes + critère mesurable
- Validation : checklist binaire 3 items
- Antipatterns spec 01 §10 / spec 11 §10 : aucun détecté

---

## Décision pipeline

PASS round 1 → exit loop sans round 2. Le brouillon est éligible à la promotion vers `docs/learning-path/` après checkpoint humain (spec 12 §5) et passage du glossary keeper.
