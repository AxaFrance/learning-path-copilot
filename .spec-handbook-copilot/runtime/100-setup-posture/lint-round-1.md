# Lint — docs/_drafts/learning-path/00-setup-posture.md (Round 1)

**Verdict global** : FAIL
**Score** : 5 / 9 critères validés (2 FAIL bloquants, 2 WARNING)
**Références** : spec 01 (gabarit) + spec 11 (conventions)

---

## Per-criterion verdict

| # | Critère | Source | Verdict |
|---|---|---|---|
| 1 | Frontmatter Docusaurus complet | spec 01 §1 / spec 11 §8 | PASS |
| 2 | En-tête (Durée · Complexité · Pré-requis + blockquote) | spec 01 §2 | PASS |
| 3 | Sections obligatoires présentes & ordonnées | spec 01 §3 | FAIL |
| 4 | Budget mots conforme à la complexité | spec 01 §9 | WARNING |
| 5 | Mermaid ≤ 12 nœuds | spec 01 §6 / spec 11 §5 | PASS (N/A — aucun diagramme) |
| 6 | Diffs git progressifs dans le contenu pédagogique | spec 01 §3.3 + §5 | FAIL |
| 7 | Pas d'emoji hors étoile / coche / croix | spec 11 §3 | PASS |
| 8 | Français + tutoiement + voix active | spec 11 §1–§2 | PASS |
| 9 | Liens internes relatifs valides | spec 11 §7 | WARNING (1 cible inexistante) |

---

## Failures

### F1 — sections obligatoires manquantes (critère 3)
Spec 01 §3 ordre canonique : Objectif → Ce que tu vas apprendre → Contenu pédagogique → Exercice → Validation (« Tu es prêt si… ») → Pour aller plus loin → Module suivant (ligne finale).

Le brouillon présente : Objectifs → Prérequis → Concepts clés → Mise en pratique → Pièges & anti-patterns → Pour aller plus loin → Sources.

Manquent :
- « Ce que tu vas apprendre » (mini-sommaire numéroté 4–7 items)
- « Validation — Tu es prêt si… » avec checklist binaire `- [ ]` (3–5 items)
- « Module suivant » comme ligne finale autonome

Sections hors-template à supprimer ou fusionner : Prérequis, Pièges & anti-patterns, Sources.

Renommer : Objectifs → Objectif ; Concepts clés → Contenu pédagogique ; Mise en pratique → Exercice.

### F2 — zéro diff git progressif (critère 6)
Spec 01 §3.3 impose des diffs git dans « Contenu pédagogique ». Format : bloc ```diff avec chemin de fichier en commentaire, contexte ±1 ligne, une intention par diff.

Le brouillon ne contient aucun bloc code. Injecter au minimum 3 diffs :
1. `.vscode/extensions.json` ajoutant `GitHub.copilot`
2. `fizzbuzz.ts` montrant la suggestion inline
3. `fizzbuzz-agent.ts` + `fizzbuzz.test.ts` créés par l'agent

Chaque diff précédé d'une phrase d'intention et suivi d'un paragraphe d'observation.

---

## Warnings

### W1 — budget mots dépassé (critère 4)
Spec 01 §9 : module 1-étoile = 400–700 mots. Brouillon estimé à ~880 mots.

Pistes : compresser « Concepts clés », supprimer « Posture d'apprentissage », réduire l'exercice de 7 à 5 étapes.

### W2 — lien vers cible inexistante (critère 9)
`./10-copilot-cli.md` n'existe pas encore. Retirer le lien ou l'annoter « à venir » sans lien actif.

---

## Top 3 remédiations prioritaires

1. Restructurer le squelette dans l'ordre exact spec 01 §3 (les 7 headings canoniques ci-dessus).
2. Injecter au moins 3 blocs ```diff dans la section « Contenu pédagogique ».
3. Resserrer le texte sous 700 mots et corriger le lien `10-copilot-cli.md`.
